// ------- UrdfImporter.cs ------
// Author: John Farrell
//          john@johnjfarrell.com
// 
// This file contains all the necessary logic
// for importing URDF files and creating a structure
// in Godot that accurately represents the robot
// described in the URDF file.
// To successfully import custom models, the necessary folder
// containing the models must be placed into the user://models file.
// In a windows environment, this can be accessed at
//      C:\Users\[user]\AppData\Roaming\rr-godot\models
// For example, to import the Baxter URDF file, you need to place the
// entire baxter_description folder in the models directory.

using System.Collections.Generic;

using Godot;
using System;
using RosSharp.Urdf;


/// <summary>
/// Class <c>UrdfImporter</c> handles the import logic of URDF files.
/// </summary>
public class UrdfImporter : Node
{
    /// <summary>
    /// <para>UrdfPrint</para>
    /// Helper function to format debug messages for Urdf handling.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="indent">Indent level of the line. Multiplied by 2.</param>
    private void UrdfPrint(string msg, int indent = 0)
    {
        string form_msg = " U\t";
        form_msg = form_msg.PadRight(indent * 2, ' ');
        form_msg += msg;
        GD.Print(form_msg);
    }

    /// <summary>
    /// Parses a urdf file and creates a fully structured and configured Godot object for insertion
    /// into the scene tree.
    /// </summary>
    /// <param name="file_name">Absolute path of the URDF file.</param>
    /// <returns>
    /// Godot.StaticBody containing the entire robot heirarchy and configuration.
    /// </returns>
    public StaticBody Load(string file_name, Node parent)
    {
        UrdfPrint("Generating RosSharp object...");
        RosSharp.Urdf.Robot bot_struct = new Robot(file_name);

        // Create base
        UrdfPrint("Creating base...");
        StaticBody base_node = generate_base(bot_struct.root, bot_struct.name);
        UrdfPrint("Created base");

        List<Godot.Spatial> links = create_links(bot_struct.links);
        UrdfPrint("Loaded links");
        List<Godot.Joint> joints = create_joints(bot_struct.joints);
        UrdfPrint("Loaded joints");

        // Add the base_node to the SceneTree so Godot recognizes
        // node paths and transforms
        parent.AddChild(base_node);

        List<RosSharp.Urdf.Joint> base_joints = bot_struct.root.joints;

        foreach (var rosJoint in base_joints)
        {
            var godotJoint = joints.Find(x => x.Name == rosJoint.name);
            // var childLink = links.Find(x => x.Name == rosJoint.ChildLink.name);
            base_node.AddChild(godotJoint);
            // godotJoint.AddChild(childLink);
        }

        foreach (var child in base_node.GetChildren())
        {
            if(child is Godot.Joint)
            {
                Godot.Joint cJoint = (Godot.Joint) child;
                var refJoint = base_joints.Find(x => x.name == cJoint.Name);
                BuildTreeStructure(refJoint, cJoint, links, joints);
                
                ConnectJoints(cJoint.GetChildren(), links, bot_struct.joints);
            }
        }
        
        return base_node;
    }


    /// <summary>
    /// Connects all the children joints in a list of links.
    /// </summary>
    /// <param name="childList">Children of a main base node in a robot structure.</param>
    /// <param name="allLinks">A list of all the RigidBody links in the structure.</param>
    /// <param name="refJointList">A list of all the RosSharp joint descriptions.</param>
    private void ConnectJoints(Godot.Collections.Array childList,
        List<Godot.Spatial> allLinks,
        List<RosSharp.Urdf.Joint> refJointList)
    {
        foreach (var link in childList)
        {
            if( !(link is Godot.RigidBody))
            {
                continue;
            }
            Godot.RigidBody tempLink = (Godot.RigidBody)link;
            GD.Print(String.Format("Connecting link {0} joints", tempLink.Name));
            foreach (var child in tempLink.GetChildren())
            {
                if( !(child is Godot.Joint))
                {
                    continue;
                }
                Godot.Joint childJoint = (Godot.Joint)child;
                RosSharp.Urdf.Joint sJoint = refJointList.Find(x => x.name == childJoint.Name);
                Godot.RigidBody nodeB = (Godot.RigidBody)allLinks.Find(x => x.Name == sJoint.ChildLink.name);

                nodeB.GlobalTransform = childJoint.GlobalTransform;

                childJoint.Nodes__nodeA = tempLink.GetPath();
                childJoint.Nodes__nodeB = nodeB.GetPath();
            }
        }
    }

    private void BuildTreeStructure(
        RosSharp.Urdf.Joint sourceJoint, 
        Godot.Joint baseJoint,
        List<Godot.Spatial> linkList,
        List<Godot.Joint> jointList)
    {
        Godot.Spatial tempLink = linkList.Find(x => x.Name == sourceJoint.ChildLink.name);
        baseJoint.AddChild(tempLink);
        foreach (var linkJoint in sourceJoint.ChildLink.joints)
        {
            Godot.Joint tJoint = jointList.Find(x => x.Name == linkJoint.name);
            tempLink.AddChild(tJoint);
            // Transform linkTrans = tempLink.GlobalTransform;
            // var joint_offset = new Vector3(
            //     (float)linkJoint.origin.Xyz[0],
            //     (float)linkJoint.origin.Xyz[2],
            //     -1F * (float)linkJoint.origin.Xyz[1]
            // );
            // linkTrans = linkTrans.Translated(joint_offset);
            // tJoint.GlobalTransform = linkTrans;

            tJoint.TranslateObjectLocal(new Vector3(
                (float)linkJoint.origin.Xyz[0],
                (float)linkJoint.origin.Xyz[2],
                -1.0F * (float)linkJoint.origin.Xyz[1]
            ));
            
            // tJoint.RotateObjectLocal(new Vector3(1,0,0), (float)linkJoint.origin.Rpy[0]);
            // tJoint.RotateObjectLocal(new Vector3(0,1,0), (float)linkJoint.origin.Rpy[2]);
            // tJoint.RotateObjectLocal(new Vector3(0,0,1), -1F * (float)linkJoint.origin.Rpy[1]);

            tJoint.RotateX((float)linkJoint.origin.Rpy[0]);
            tJoint.RotateY((float)linkJoint.origin.Rpy[2]);
            tJoint.RotateZ(-1.0F * (float)linkJoint.origin.Rpy[1]);

            BuildTreeStructure(linkJoint, baseJoint, linkList, jointList);
        }
        
    }

    private List<Godot.Spatial> create_links(List<RosSharp.Urdf.Link> source)
    {
        List<Godot.Spatial> links =  new List<Godot.Spatial>();
        
        foreach (RosSharp.Urdf.Link slink in source)
        {
            RigidBody link = new RigidBody();
            link.Name = slink.name;
            link.CanSleep = false;
            link.ContinuousCd = true;

            if(slink.inertial != null)
            {
                link.Mass = (float)slink.inertial.mass;
            }

            UrdfPrint(String.Format("Generating {0} geometries", link.Name));
            MeshInstance link_mesh = create_visual_geometry(slink.visuals);
            CollisionShape link_col = create_collision_geometry(slink.collisions);       

            if(slink.collisions.Count >= 1) {
                link_col.Name = String.Format("{0}_col", link.Name);
                link.AddChild(link_col);
                link_col.TranslateObjectLocal(
                    new Vector3((float)slink.collisions[0].origin.Xyz[0],
                                (float)slink.collisions[0].origin.Xyz[2],
                                (float)slink.collisions[0].origin.Xyz[1])
                );
                link_col.RotateObjectLocal(
                    new Vector3(1, 0, 0),
                    (float)slink.collisions[0].origin.Rpy[0]
                );
                link_col.RotateObjectLocal(
                    new Vector3(0, 1, 0),
                    (float)slink.collisions[0].origin.Rpy[2]
                );
                link_col.RotateObjectLocal(
                    new Vector3(0, 0, 1),
                    -1F * (float)slink.collisions[0].origin.Rpy[1]
                );
            }
            if(slink.visuals.Count >= 1) {
                link_mesh.Name = String.Format("{0}_mesh", link.Name);
                link.AddChild(link_mesh);
                link_mesh.TranslateObjectLocal(
                    new Vector3((float)slink.visuals[0].origin.Xyz[0],
                                (float)slink.visuals[0].origin.Xyz[2],
                                (float)slink.visuals[0].origin.Xyz[1])
                );
                link_mesh.RotateObjectLocal(
                    new Vector3(1, 0, 0),
                    (float)slink.visuals[0].origin.Rpy[0]
                );
                link_mesh.RotateObjectLocal(
                    new Vector3(0, 1, 0),
                    (float)slink.visuals[0].origin.Rpy[2]
                );
                link_mesh.RotateObjectLocal(
                    new Vector3(0, 0, 1),
                    -1F * (float)slink.visuals[0].origin.Rpy[1]
                );
            }
            
            links.Add(link);
        }
        return links;
    }

    private List<Godot.Joint> create_joints(List<RosSharp.Urdf.Joint> source)
    {
        List<Godot.Joint> joints = new List<Godot.Joint>();

        foreach (RosSharp.Urdf.Joint sjoint in source)
        {
            switch (sjoint.type)
            {
                case "revolute":
                    // A hinge joint that rotates along the axis and has a
                    // limited range specified by the upper and lower limits.
                    joints.Add(configure_revolute(sjoint));
                    break;
                case "continuous":
                    // a continuous hinge joint that rotates around the axis 
                    // and has no upper and lower limits.
                    joints.Add(configure_continuous(sjoint));
                    break;
                case "prismatic":
                    // a sliding joint that slides along the axis, and has a
                    // limited range specified by the upper and lower limits.
                    joints.Add(configure_prismatic(sjoint));
                    break;
                case "fixed":
                    // This is not really a joint because it cannot move.
                    // All degrees of freedom are locked. This type of joint 
                    // does not require the axis, calibration, dynamics, 
                    // limits or safety_controller.
                    joints.Add(configure_fixed(sjoint));
                    break;
                case "floating":
                    // This joint allows motion for all 6 degrees of freedom.
                    joints.Add(configure_floating(sjoint));
                    break;
                case "planar":
                    // This joint allows motion in a plane perpendicular to the axis.
                    joints.Add(configure_planar(sjoint));
                    break;
                default:
                    break;
            }
        }

        return joints;
    }

    private HingeJoint configure_revolute(RosSharp.Urdf.Joint source)
    {
        HingeJoint joint = configure_continuous(source);

        joint.AngularLimit__enable = true;
        joint.AngularLimit__upper = (float)source.limit.upper * (180F / (float)Math.PI);
        joint.AngularLimit__lower = (float)source.limit.lower * (180F / (float)Math.PI);

        return joint;
    }

    private HingeJoint configure_continuous(RosSharp.Urdf.Joint source)
    {
        HingeJoint joint = new HingeJoint();
        joint.Name = source.name;

        joint.Motor__enable = true;
        joint.Motor__targetVelocity = 0F;
        // joint.Motor__maxImpulse = 1024F;

        joint.Params__bias = 0.99F;

        joint.AngularLimit__bias = 0.99F;
        joint.AngularLimit__softness = 0.01F;
        joint.AngularLimit__relaxation = 0.01F;

        MeshInstance joint_ind = generate_joint_mesh(
            String.Format("{0}_indicator", joint.Name)
        );

        joint.AddChild(joint_ind);
        return joint;
    }

    private SliderJoint configure_prismatic(RosSharp.Urdf.Joint source)
    {
        throw new NotImplementedException();
    }

    private HingeJoint configure_fixed(RosSharp.Urdf.Joint source)
    {
        HingeJoint joint = new HingeJoint();
        joint.Name = source.name;

        joint.Motor__enable = false;

        joint.Params__bias = 0.99F;
        joint.AngularLimit__enable = true;
        joint.AngularLimit__upper = 0F;
        joint.AngularLimit__lower = 0F;
        joint.AngularLimit__bias = 0.99F;
        joint.AngularLimit__softness = 0.9F;
        joint.AngularLimit__relaxation = 1;

        MeshInstance joint_ind = generate_joint_mesh(
            String.Format("{0}_indicator", joint.Name)
        );

        joint.AddChild(joint_ind);
        return joint;
    }

    private Generic6DOFJoint configure_fixed_gen(RosSharp.Urdf.Joint source)
    {
        Generic6DOFJoint joint = new Generic6DOFJoint();
        joint.Name = source.name;

        joint.LinearLimitX__enabled = true;
        joint.LinearLimitY__enabled = true;
        joint.LinearLimitZ__enabled = true;
        joint.AngularLimitX__enabled = true;
        joint.AngularLimitY__enabled = true;
        joint.AngularLimitZ__enabled = true;

        joint.LinearLimitX__softness = 16F;
        joint.LinearLimitY__softness = 16F;
        joint.LinearLimitZ__softness = 16F;
        joint.AngularLimitX__softness = 16F;
        joint.AngularLimitY__softness = 16F;
        joint.AngularLimitZ__softness = 16F;

        joint.LinearLimitX__restitution = 0.01F;
        joint.LinearLimitY__restitution = 0.01F;
        joint.LinearLimitZ__restitution = 0.01F;
        joint.AngularLimitX__restitution = 0.01F;
        joint.AngularLimitY__restitution = 0.01F;
        joint.AngularLimitZ__restitution = 0.01F;

        joint.LinearLimitX__damping = 16F;
        joint.LinearLimitY__damping = 16F;
        joint.LinearLimitZ__damping = 16F;
        joint.AngularLimitX__damping = 16F;
        joint.AngularLimitY__damping = 16F;
        joint.AngularLimitZ__damping = 16F;

        joint.LinearLimitX__upperDistance = 0F;
        joint.LinearLimitX__lowerDistance = 0F;
        joint.LinearLimitY__upperDistance = 0F;
        joint.LinearLimitY__lowerDistance = 0F;
        joint.LinearLimitZ__upperDistance = 0F;
        joint.LinearLimitZ__lowerDistance = 0F;
        joint.AngularLimitX__upperAngle = 0F;
        joint.AngularLimitX__lowerAngle = 0F;
        joint.AngularLimitY__upperAngle = 0F;
        joint.AngularLimitY__lowerAngle = 0F;
        joint.AngularLimitZ__upperAngle = 0F;
        joint.AngularLimitZ__lowerAngle = 0F;


        MeshInstance joint_ind = generate_joint_mesh(
            String.Format("{0}_indicator", joint.Name)
        );

        joint.AddChild(joint_ind);
        return joint;
    }

    private Generic6DOFJoint configure_floating(RosSharp.Urdf.Joint source)
    {
        throw new NotImplementedException();
    }

    private Generic6DOFJoint configure_planar(RosSharp.Urdf.Joint source)
    {
        throw new NotImplementedException();
    }

    private MeshInstance generate_joint_mesh(string name)
    {
        MeshInstance j_mesh = new MeshInstance();
        j_mesh.Name = name;

        SphereMesh m_mesh = new SphereMesh();
        m_mesh.Radius = 0.05F;
        m_mesh.Height = 0.1F;
        m_mesh.RadialSegments = 5;
        m_mesh.Rings = 5;

        // Create material
        SpatialMaterial m_mat = new SpatialMaterial();
        m_mat.AlbedoColor = new Color(0.26F, 0.32F, 0.92F);
        m_mat.EmissionEnabled = true;
        m_mat.Emission = new Color(0.4F, 0.3F, 1.0F);
        m_mat.EmissionEnergy = 3.0F;

        j_mesh.Mesh = m_mesh;
        j_mesh.MaterialOverride = m_mat;

        return j_mesh;
    }

    /// <summary>
    /// Creates the base node of the robot for the world tree.
    /// </summary>
    /// <param name="link">Urdf definition of the link.</param>
    /// <param name="bot_name">Name of the robot.</param>
    /// <returns>
    /// A Godot.StaticBody node with configured Godot.MeshInstance and Godot.CollisionShape children.
    /// </returns>
    private StaticBody generate_base(RosSharp.Urdf.Link link, string bot_name)
    {
        StaticBody node = new StaticBody();
        node.Name = String.Format("{0}-{1}", link.name, bot_name);
        PhysicsMaterial mat = new PhysicsMaterial();
        node.PhysicsMaterialOverride = mat;
        
        MeshInstance node_mesh = create_visual_geometry(link.visuals);
        if(node_mesh != null)
        {
            node_mesh.Name = String.Format("{0}_mesh", node.Name);
            node.AddChild(node_mesh);
        }
        CollisionShape node_col = create_collision_geometry(link.collisions);
        if(node_col != null)
        {
            node_col.Name = String.Format("{0}_col", node.Name);
            node.AddChild(node_col);
        }
        return node;
    }

    /// <summary>
    /// Generates a Godot.SpatialMaterial based off a Urdf material definition.
    /// </summary>
    /// <param name="source_mat">Urdf material</param>
    /// <returns>Fully configured Godot.SpatialMaterial.</returns>
    private SpatialMaterial create_material(RosSharp.Urdf.Link.Visual.Material source_mat)
    {
        if(source_mat == null)
        {
            return null;
        }

        SpatialMaterial ret_mat = new SpatialMaterial();
        var matColor = new Godot.Color();
        matColor.r = (float)source_mat.color.rgba[0];
        matColor.g = (float)source_mat.color.rgba[1];
        matColor.b = (float)source_mat.color.rgba[2];
        matColor.a = (float)source_mat.color.rgba[3];
        ret_mat.AlbedoColor = matColor;

        return ret_mat;
    }

    /// <summary>
    /// Creates a fully configured Godot.MeshInstance based off a Urdf visual list.
    /// </summary>
    /// <param name="visuals">List of URDF visuals.</param>
    /// <returns>
    /// Godot.MeshInstance
    /// </returns>
    private MeshInstance create_visual_geometry(
        List<RosSharp.Urdf.Link.Visual> visuals)
    {
        if (visuals.Count < 1)
        {
            return null;
        }

        Link.Visual workingVis = visuals[0];
        SpatialMaterial mat = create_material(workingVis.material);
        if(workingVis.geometry.box != null)
        {
            return create_box_mesh(workingVis.geometry.box, mat);
        }
        if(workingVis.geometry.cylinder != null)
        {
            return create_cylinder_mesh(workingVis.geometry.cylinder, mat);
        }
        if(workingVis.geometry.sphere != null)
        {
            return create_sphere_mesh(workingVis.geometry.sphere, mat);
        }
        if(workingVis.geometry.mesh != null)
        {
            return create_custom_mesh(workingVis.geometry.mesh, mat);
        }
        return null;
    }

    /// <summary>
    /// Helper function to translate URDF box geometry into a Godot terms
    /// </summary>
    /// <param name="source">URDF Box geometry object</param>
    /// <param name="mat">Material for the mesh to use</param>
    /// <returns></returns>
    private MeshInstance create_box_mesh(Link.Geometry.Box source, SpatialMaterial mat)
    {
        MeshInstance ret_val = new MeshInstance();

        CubeMesh cmesh = new CubeMesh();
        // Axii 2 and 1 are switched due to the different
        // coordinate structures Godot and URDF uses
        cmesh.Size = new Vector3(
            (float)source.size[0],
            (float)source.size[2],
            (float)source.size[1]
        );

        cmesh.Material = mat;
        ret_val.Mesh = cmesh;
        return ret_val;
    }

    private MeshInstance create_cylinder_mesh(Link.Geometry.Cylinder source, SpatialMaterial mat)
    {
        MeshInstance ret_val = new MeshInstance();

        CylinderMesh cmesh = new CylinderMesh();
        cmesh.TopRadius = (float)source.radius;
        cmesh.BottomRadius = (float)source.radius;
        cmesh.Height = (float)source.length;

        cmesh.Material = mat;
        ret_val.Mesh = cmesh;
        return ret_val;
    }

    private MeshInstance create_sphere_mesh(Link.Geometry.Sphere source, SpatialMaterial mat)
    {
        MeshInstance ret_val = new MeshInstance();

        SphereMesh cmesh = new SphereMesh();
        cmesh.Radius = (float)source.radius;
        cmesh.Height = (float)(source.radius * 2.0);

        cmesh.Material = mat;
        ret_val.Mesh = cmesh;
        return ret_val;
    }

    private MeshInstance create_custom_mesh(Link.Geometry.Mesh source, SpatialMaterial mat)
    {
        
        string filename = GetFullPath(source.filename);
        MeshMaker mm = new MeshMaker();
        
        return (MeshInstance) mm.CreateMesh(filename);
    }

    private string GetFullPath(string file)
    {
        string[] splitPath = file.Split('/');

        GD.Print(String.Format("Path: {0}", file));
        // Path is a ROS package path, have to convert
        // to an absolute path
        string packPath = OS.GetUserDataDir() + "/models";
        if (splitPath[0] == "package:")
        {
            for (var i = 2; i < splitPath.Length; ++i)
            {
                packPath += "/";
                packPath += splitPath[i];
            }
        }
        return packPath;
    }

    private CollisionShape create_collision_geometry(
        List<RosSharp.Urdf.Link.Collision> collisions)
    {
        if (collisions.Count < 1)
        {
            return null;
        }

        Link.Collision workingCol = collisions[0];
        if(workingCol.geometry.box != null)
        {
            return create_box_collision(workingCol.geometry.box);
        }
        if(workingCol.geometry.cylinder != null)
        {
            return create_cylinder_collision(workingCol.geometry.cylinder);
        }
        if(workingCol.geometry.sphere != null)
        {
            return create_sphere_collision(workingCol.geometry.sphere);
        }
        if(workingCol.geometry.mesh != null)
        {
            return create_custom_collision(workingCol.geometry.mesh);
        }
        return null;
    }

    private CollisionShape create_box_collision(Link.Geometry.Box source)
    {
        CollisionShape ret_val = new CollisionShape();

        BoxShape cshape = new BoxShape();
        // We divide the given sizes by two because the
        // BoxShape extents are given in half sizes
        cshape.Extents = new Vector3(
            (float)source.size[0] / 2F,
            (float)source.size[2] / 2F,
            (float)source.size[1] / 2F
        );

        ret_val.Shape = cshape;
        return ret_val;
    }

    private CollisionShape create_cylinder_collision(Link.Geometry.Cylinder source)
    {
        CollisionShape ret_val = new CollisionShape();

        CylinderShape cshape = new CylinderShape();
        cshape.Radius = (float)source.radius;
        cshape.Height = (float)source.length;

        ret_val.Shape = cshape;
        return ret_val;
    }

    private CollisionShape create_sphere_collision(Link.Geometry.Sphere source)
    {
        CollisionShape ret_val = new CollisionShape();

        SphereShape cshape = new SphereShape();
        cshape.Radius = (float)source.radius;
        
        ret_val.Shape = cshape;
        return ret_val;
    }

    private CollisionShape create_custom_collision(Link.Geometry.Mesh source)
    {
        CollisionShape ret_val = new CollisionShape();

        string filename = GetFullPath(source.filename);
        MeshMaker mm = new MeshMaker();

        MeshInstance temp_mesh = (MeshInstance)mm.CreateMesh(filename);
        Shape cshape = temp_mesh.Mesh.CreateTrimeshShape();

        ret_val.Shape = cshape;
        return ret_val;
    }
}
