using System.Collections.Generic;

using Godot;
using System;
using RosSharp.Urdf;
using RR_Godot.Core.Urdf;


public class UrdfImporter : Node
{
    Robot _robot;

    public UrdfNode _robotRoot;

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
    /// <para>PrintTree</para>
    /// Prints the tree structure of
    /// the Urdf file.
    /// </summary>
    /// <param name="root">
    /// Root UrdfNode of the tree.
    /// </param>
    public void PrintTree(UrdfNode root)
    {
        int count = 1;

        foreach (var childNode in root.GetChildren())
        {
            string status = "(" + count + ") - Node " + childNode._name + ": " + childNode.GetNumChildren();
            UrdfPrint(status);

            PrintTree(childNode, 1);
            count += 1;
        }
    }


    private void PrintTree(UrdfNode root, int level)
    {
        int count = 1;

        foreach (var childNode in root.GetChildren())
        {
            string status = "(" + count + ") - Node " + childNode._name + ": " + childNode.GetNumChildren();
            UrdfPrint(status, level);

            PrintTree(childNode, level + 1);
            count += 1;
        }
    }

    /// <summary>
    /// <para>CreateNodeTree</para>
    /// Creates a tree representation
    /// of the URDF file using custom
    /// UrdfNode objects.
    /// </summary>
    /// <returns>
    /// UrdfNode of the root node in the
    /// tree.
    /// </returns>
    private UrdfNode CreateNodeTree(Robot bot)
    {
        // Create the root node
        UrdfNode rootNode = new UrdfNode(null, bot.root, null);
        rootNode._isRoot = true;
        rootNode._name = bot.root.name;

        PopulateChildren(rootNode);

        return rootNode;
    }

    /// <summary>
    /// <para>PopulateChildren</para>
    /// Recursively builds a n-tree of UrdfNode objects
    /// from the parsed Urdf file.
    /// </summary>
    /// <param name="base_node">
    /// Base node to build the tree off.
    /// </param>
    private void PopulateChildren(UrdfNode base_node)
    {
        // Go through each joint and get the connected
        // link, creating a fully specified UrdfNode 
        // object to add as a child to base_node
        foreach (var joint in base_node._link.joints)
        {
            Link joint_link = joint.ChildLink;
            UrdfNode temp = new UrdfNode(base_node, joint_link, joint);
            temp._name = joint.child;

            temp._offsetXyz = joint.origin.Xyz;
            temp._offsetRpy = joint.origin.Rpy;

            base_node.AddChild(temp);

            PopulateChildren(temp);
        }
    }

    private void PrintRobotInfo(Robot bot)
    {
        if (bot != null)
        {
            var botName = bot.name;
            var fileName = bot.filename;

            UrdfPrint("Parsed robot " + botName + " from file: " + fileName);
        }
    }

    /// <summary>
    /// Parses a urdf file and creates a fully structured and configured Godot object for insertion
    /// into the scene tree.
    /// </summary>
    /// <param name="file_name">Absolute path of the URDF file.</param>
    /// <returns>
    /// Godot.StaticBody containing the entire robot heirarchy and configuration.
    /// </returns>
    public StaticBody Parse(string file_name)
    {
        RosSharp.Urdf.Robot bot_struct = new Robot(file_name);

        // Create base
        StaticBody base_node = generate_base(bot_struct.root, bot_struct.name);

        // List<Godot.Spatial> links = create_links(bot_struct.links);
        // List<Godot.Joint> joints = create_joints(bot_struct.joints);
        return base_node;
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

    /// <summary>
    /// <para>Parse</para>
    /// Parses a Urdf file into class RosSharp.Urdf.Robot member.
    /// </summary>
    /// <param name="file_name">Name of the Urdf file.</param>
    /// <returns>
    /// <para>True if the file was succesfully parsed.</para>
    /// <para>False if there was an error while parsing.<para>
    /// </returns>
    public bool Parse2(string file_name)
    {
        try
        {
            _robot = new Robot(file_name);
            GD.Print("Robot link and joint count:");
            GD.Print(_robot.links.Count);
            GD.Print(_robot.joints.Count);
            _robotRoot = CreateNodeTree(_robot);
        }
        catch
        {
            UrdfPrint("Error parsing Urdf file.\tRobot not parsed!");
            return false;
        }
        PrintRobotInfo(_robot);

        return true;
    }

    /// <summary>
    /// <para>GenerateSpatial</para>
    /// <para>
    /// Generates a tree of Godot objects capable of
    /// being inserted into a Godot SceneTree.
    /// </para>
    /// <para>
    /// This tree accurately represents Urdf Joints and Links
    /// in Godot terms.
    /// </para>
    /// </summary>
    /// <param name="base_node">UrdfNode containing the root Urdf component.</param>
    /// <returns>A Godot.Spatial containing the root of the Godot tree.</returns>
    public StaticBody GenerateSpatial(UrdfNode base_node)
    {
        // Create the empty spatial node
        StaticBody rootSpat = new StaticBody();
        rootSpat.Name = base_node._name;

        // Add children recursively
        foreach (var child in base_node.GetChildren())
        {
            // Returns a joint connected to a rigid body
            Godot.Joint childJoint = GenerateSpatialRec(child);
            rootSpat.AddChild(childJoint);

            // Transform according to the child joint transformations
            // Godot's 3D scene has X forward, Y up, and Z right, while
            // Urdf uses X forward, Y right, Z up. 
            // This is why the indices below aren't in order, it translates
            // the Urdf coordinates into Godot coordinates.
            childJoint.TranslateObjectLocal(new Vector3(
                (float)child._joint.origin.Xyz[0],
                (float)child._joint.origin.Xyz[2],
                (float)child._joint.origin.Xyz[1]
            ));
            childJoint.RotateX((float)child._joint.origin.Rpy[0]);
            childJoint.RotateY((float)child._joint.origin.Rpy[2]);
            childJoint.RotateZ(-1.0F * (float)child._joint.origin.Rpy[1]);
        }

        return rootSpat;
    }

    /// <summary>
    /// <para>GenerateSpatialRec</para>
    /// Recursive component to generate the Godot SceneTree
    /// structure of the URDF file, complete with joints and
    /// collision shapes.
    /// </summary>
    /// <param name="base_node">
    /// UrdfNode containing the node to generate off of.
    /// </param>
    /// <returns>
    /// A Godot.Generic6DOFJoint that represents the start of the Godot 
    /// representation of the URDF tree structure.
    /// </returns>
    private Godot.Joint GenerateSpatialRec(UrdfNode base_node)
    {
        // Create the return joint
        Godot.Joint finJoint = ConfigureJoint(base_node._joint);
        finJoint.Name = base_node._joint.name;

        // Create the return RigidBody
        RigidBody tempLink = base_node.CreateLink();
        

        foreach (var child in base_node.GetChildren())
        {
            // This is the same as GenerateSpatial(), so look at that
            // function for the explanation.
            Godot.Joint childJoint = GenerateSpatialRec(child);
            tempLink.AddChild(childJoint);
            childJoint.SetOwner(tempLink);

            childJoint.TranslateObjectLocal(new Vector3(
                (float)child._joint.origin.Xyz[0],
                (float)child._joint.origin.Xyz[2],
                -1.0F * (float)child._joint.origin.Xyz[1]
            ));

            try
            {
                // childJoint.RotateX((float)child._joint.axis.xyz[0]);
                // childJoint.RotateY((float)child._joint.axis.xyz[2]);
                // childJoint.RotateZ(-1.0F * (float)child._joint.axis.xyz[1]);
            }
            catch
            {
                GD.Print("Axis not specified, continuing...");
            }
            

            childJoint.RotateX((float)child._joint.origin.Rpy[0]);
            childJoint.RotateY((float)child._joint.origin.Rpy[2]);
            childJoint.RotateZ(-1.0F * (float)child._joint.origin.Rpy[1]);

            GD.Print(childJoint.Transform.ToString());
        }
        finJoint.AddChild(tempLink);

        return finJoint;
    }

    /// <summary>
    /// <para>ConnectJoints</para>
    /// Iterates through a Spatial->Joint->Link tree setting the joint
    /// endpoints to the necessary links. 
    /// <para>The tree is generated by <see cref="GenerateSpatial" /></para>
    /// </summary>
    /// <param name="root">Spatial node containing the root of the tree.</param>
    public void ConnectJoints(Spatial root)
    {
        Queue<Spatial> nodeQueue = new Queue<Spatial>();

        foreach (var child in root.GetChildren())
        {
            nodeQueue.Enqueue((Spatial)child);
        }

        Spatial curr = nodeQueue.Dequeue();
        while (curr != null)
        {
            if (!curr.GetType().Equals(typeof(Godot.PinJoint)) &&
                !curr.GetType().Equals(typeof(Godot.HingeJoint)) &&
                !curr.GetType().Equals(typeof(Generic6DOFJoint)))
            {
                // If the current object isn't a joint, its a link so you need
                // to add all of the children to the queue.
                // Some of this will be a meshinstance/collisionshape, but those
                // will get filtered out.
                foreach (var child in curr.GetChildren())
                {
                    nodeQueue.Enqueue((Spatial)child);
                }
                if (nodeQueue.Count == 0)
                {
                    break;
                }
                curr = nodeQueue.Dequeue();
                continue;
            }
            Godot.Joint tempJoint = (Godot.Joint)curr;

            // We have a joint, set the endpoints

            // A joints parent will always be a RigidBody.
            NodePath parentPath = curr.GetParent().GetPath();
            // A joint will always have only one child which is a RigidBody,
            // this is what we want the second endpoint to be.
            NodePath childPath = curr.GetChild(0).GetPath();

            tempJoint.SetOwner(curr.GetParent());

            tempJoint.SetNodeA(parentPath);
            tempJoint.SetNodeB(childPath);

            // tempJoint.Nodes__nodeA = parentPath;
            // tempJoint.Nodes__nodeB = childPath;

            nodeQueue.Enqueue((Spatial)curr.GetChild(0));

            if (nodeQueue.Count == 0)
            {
                break;
            }
            curr = nodeQueue.Dequeue();
        }
    }

    // TODO
    // * Support Urdf axis that aren't directly aligned with the
    //    X, Y, or Z axis
    /// <summary>
    /// <para>ConfigureJoint</para>
    /// Configures the Godot Generic6DOFJoint
    /// properties to match what the Urdf joint
    /// contains.
    /// </summary>
    /// <param name="base_joint">Urdf Joint specifications.</param>
    /// <returns>Godot joint matching specs.</returns>
    private Godot.Joint ConfigureJoint(RosSharp.Urdf.Joint base_joint)
    {
        // The Urdf joint axis specifies the axis of rotation for revolute joints,
        // axis of translation for prismatic joints, and the surface normal
        // for planar joints.
        // If it's not specified accessing it will error out, so we need to
        // manually specify the default (X-axis).
        double[] j_axis;
        
        try
        {
            j_axis = base_joint.axis.xyz;
        }
        catch
        {
            j_axis = new double[] { 1.0, 0.0, 0.0 };
        }
        GD.Print(base_joint.name + " axis: " + j_axis[0] + " " + j_axis[1] + " " + j_axis[2]);

        JointMaker mkr = new JointMaker();


        // Type comments taken from https://wiki.ros.org/urdf/XML/joint 
        switch (base_joint.type)
        {
            case "revolute":
                // A hinge joint that rotates along the axis and has a
                // limited range specified by the upper and lower limits.
                // HingeJoint revJoint = mkr.CreateHingeJoint();
                // // HingeJoint revJoint = new HingeJoint();

                // // revJoint.rot/
                // GD.Print("setting hingejoint flags");

                // revJoint.SetFlag(HingeJoint.Flag.UseLimit, true);
                // revJoint.SetFlag(HingeJoint.Flag.EnableMotor, true);
                // revJoint.SetParam(HingeJoint.Param.MotorTargetVelocity, 0F);
                // revJoint.SetParam(HingeJoint.Param.MotorMaxImpulse, 1024F);
                // revJoint.SetParam(HingeJoint.Param.Bias, 1F);
                // revJoint.SetParam(HingeJoint.Param.LimitLower, (float)base_joint.limit.lower * (180F / (float)Math.PI));
                // revJoint.SetParam(HingeJoint.Param.LimitUpper, (float)base_joint.limit.upper * (180F / (float)Math.PI));

                // return revJoint;
            case "continuous":
                // a continuous hinge joint that rotates around the axis 
                // and has no upper and lower limits.
                HingeJoint contJoint = new HingeJoint();

                contJoint.SetFlag(HingeJoint.Flag.UseLimit, false);
                contJoint.SetFlag(HingeJoint.Flag.EnableMotor, true);
                contJoint.SetParam(HingeJoint.Param.MotorTargetVelocity, 0F);

                return contJoint;
            case "prismatic":
                // a sliding joint that slides along the axis, and has a
                // limited range specified by the upper and lower limits.

                SliderJoint slideJoint = new SliderJoint();

                slideJoint.SetParam(SliderJoint.Param.LinearLimitLower, (float)base_joint.limit.lower);
                slideJoint.SetParam(SliderJoint.Param.LinearLimitUpper, (float)base_joint.limit.upper); 
                
                return slideJoint;
            case "fixed":
                // This is not really a joint because it cannot move.
                // All degrees of freedom are locked. This type of joint 
                // does not require the axis, calibration, dynamics, 
                // limits or safety_controller.

                Generic6DOFJoint pinJoint = new Generic6DOFJoint();

                pinJoint.SetFlagX(Generic6DOFJoint.Flag.EnableAngularLimit, true);
                pinJoint.SetFlagY(Generic6DOFJoint.Flag.EnableAngularLimit, true);
                pinJoint.SetFlagZ(Generic6DOFJoint.Flag.EnableAngularLimit, true);
                pinJoint.SetFlagX(Generic6DOFJoint.Flag.EnableLinearLimit, true);
                pinJoint.SetFlagY(Generic6DOFJoint.Flag.EnableLinearLimit, true);
                pinJoint.SetFlagZ(Generic6DOFJoint.Flag.EnableLinearLimit, true);

                return pinJoint;
            case "floating":
                // This joint allows motion for all 6 degrees of freedom.
                Generic6DOFJoint genJoint = new Generic6DOFJoint();

                genJoint.SetFlagX(Generic6DOFJoint.Flag.EnableAngularLimit, false);
                genJoint.SetFlagY(Generic6DOFJoint.Flag.EnableAngularLimit, false);
                genJoint.SetFlagZ(Generic6DOFJoint.Flag.EnableAngularLimit, false);
                genJoint.SetFlagX(Generic6DOFJoint.Flag.EnableLinearLimit, false);
                genJoint.SetFlagY(Generic6DOFJoint.Flag.EnableLinearLimit, false);
                genJoint.SetFlagZ(Generic6DOFJoint.Flag.EnableLinearLimit, false);

                return genJoint;
            case "planar":
                // This joint allows motion in a plane perpendicular to the axis.

                Generic6DOFJoint planJoint = new Generic6DOFJoint();
                if (j_axis[0] == 1.0)
                {
                    planJoint.SetParamY(
                        Generic6DOFJoint.Param.LinearUpperLimit,
                        (float)base_joint.limit.upper
                    );
                    planJoint.SetParamY(
                        Generic6DOFJoint.Param.LinearLowerLimit,
                        (float)base_joint.limit.lower
                    );
                    planJoint.SetParamZ(
                        Generic6DOFJoint.Param.LinearUpperLimit,
                        (float)base_joint.limit.upper
                    );
                    planJoint.SetParamZ(
                        Generic6DOFJoint.Param.LinearLowerLimit,
                        (float)base_joint.limit.lower
                    );
                }
                if (j_axis[1] == 1.0)
                {
                    planJoint.SetParamY(
                        Generic6DOFJoint.Param.LinearUpperLimit,
                        (float)base_joint.limit.upper
                    );
                    planJoint.SetParamY(
                        Generic6DOFJoint.Param.LinearLowerLimit,
                        (float)base_joint.limit.lower
                    );
                    planJoint.SetParamX(
                        Generic6DOFJoint.Param.LinearUpperLimit,
                        (float)base_joint.limit.upper
                    );
                    planJoint.SetParamX(
                        Generic6DOFJoint.Param.LinearLowerLimit,
                        (float)base_joint.limit.lower
                    );
                }
                if (j_axis[2] == 1.0)
                {
                    planJoint.SetParamX(
                        Generic6DOFJoint.Param.LinearUpperLimit,
                        (float)base_joint.limit.upper
                    );
                    planJoint.SetParamX(
                        Generic6DOFJoint.Param.LinearLowerLimit,
                        (float)base_joint.limit.lower
                    );
                    planJoint.SetParamZ(
                        Generic6DOFJoint.Param.LinearUpperLimit,
                        (float)base_joint.limit.upper
                    );
                    planJoint.SetParamZ(
                        Generic6DOFJoint.Param.LinearLowerLimit,
                        (float)base_joint.limit.lower
                    );
                }

                return planJoint;
            default:
                GD.Print("testing: " + base_joint.type);
                break;
        }

        return null;
    }
}
