// ------ MeshAdder.cs -----
// Author: John Farrell
//          john@johnjfarrell.com
//
// MeshAdder is used to add the robot arms
// created using the 'Create' button.
// Currently a god class, need to seperate out
// the functionalities. I'm keeping the arm generation
// code here temporarily for legacy purposes. This was
// the first working version of robot generation
// programmatically, so it is a good reference to have for
// the future.

using Godot;
using System;
using System.Collections.Generic;

public class MeshAdder : Spatial
{
    // Flag that's set by a signal
    // in order to know when to build a bot
    // during the physics process
    bool build_robot = false;

    [Signal]
    public delegate void ArmAdded(Spatial rootObj);
    RandomNumberGenerator rand;

    Viewport RootView;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rand = new RandomNumberGenerator();
        MainLoop loop = Engine.GetMainLoop();
        SceneTree mainTree = (SceneTree)loop;
        RootView = mainTree.Root;
    }

    // Function that is called from a signal outside.
    // Currently sent by the 'create' button click event.
    public void Generate2DoFArm()
    {
        build_robot = true;
    }

    // This is where the magic happens for robot creation.
    // Pretty straight flow. Create all the joints/links, place them in
    // the environment, then deal with transforms and joint
    // connections
    private void GenerateNDofArmHelper(int N)
    {
        // Create base
        StaticBody base_node = generate_static(String.Format("{0}_dof_base", N));

        // Create N-1 Rigid Bodies
        List<Spatial> links = create_links(N - 1);
        print_list(links);

        // Create N Joints
        List<Joint> joints = create_joints(N);
        print_list(joints);

        Spatial env_node = RootView.GetNode<Spatial>("main/env");
        env_node.AddChild(base_node);
        
        base_node.GlobalTranslate(new Vector3(rand.Randfn() * 3F, 0F, rand.Randfn() * 3F));
        base_node.GlobalRotate(new Vector3(0F, 1F, 0F), rand.RandfRange(-3.14F, 3.14F));

        Joint base_joint = joints[0];
        
        // Add base joint
        base_node.AddChild(base_joint);

        for(int i = 1; i < N; i++)
        {
            Spatial link = links[i-1];
            Joint joint = joints[i];

            link.AddChild(joint);
            base_joint.AddChild(link);
        }
        base_joint.AddChild(links[N-1]);

        construct_transforms(base_node, base_joint);
        connect_joints(joints, links, base_node, base_joint, N);

        EmitSignal("ArmAdded", base_node);
    }

    private void connect_joints(
        List<Joint> joints, List<Spatial> links,
        Spatial base_link, Joint base_joint,
        int N)
    {
        GD.Print("Connecting all joints....");
        
        base_joint.Nodes__nodeA = base_link.GetPath();
        base_joint.Nodes__nodeB = links[0].GetPath();

        for(int i = 1; i < N; i++)
        {
            joints[i].Nodes__nodeA = links[i-1].GetPath();
            joints[i].Nodes__nodeB = links[i].GetPath();
        }
    }

    private void construct_transforms(Spatial base_node, Joint base_joint)
    {
        GD.Print(String.Format("Constructing spatial tree for {0}...", base_node.Name));
        // Vector3 base_node_dims = get_dimensions(base_node);

        Vector3 base_joint_offset = new Vector3(
            0F,
            .2F,
            0F
        );
        
        Transform base_j_trans = base_joint.Transform;
        base_j_trans = base_j_trans.Translated(base_joint_offset);
        base_joint.Transform = base_j_trans;

        GD.Print(String.Format("Using base joint {0}", base_joint.Name));

        Vector3 link_offset = new Vector3(
            0F, .4F, 0F
        );
        Vector3 total_link_offset = new Vector3(0F, 0F, 0F);
        foreach(Spatial child in base_joint.GetChildren())
        {
            if(child is Godot.RigidBody)
            {
                GD.Print(String.Format("Placing link {0}...", child.Name));
                Transform link_trans = child.Transform;
                link_trans = link_trans.Translated(link_offset + (2 * total_link_offset));
                child.Transform = link_trans;

                total_link_offset += link_offset;

                foreach(Spatial grandchild in child.GetChildren())
                {
                    if(grandchild is Godot.Joint)
                    {
                        GD.Print(String.Format("Placing joint {0}...", grandchild.Name));
                        grandchild.TranslateObjectLocal(link_offset);
                    }
                }
            }
        }
    }

    private List<Joint> create_joints(int N)
    {
        List<Joint> joints = new List<Joint>();

        for (int i = 0; i < N; i++)
        {
            HingeJoint joint = new HingeJoint();
            joint.Name = String.Format("j{0}", i);

            joint.Motor__enable = true;
            joint.Motor__targetVelocity = 1F;
            joint.Motor__maxImpulse = 1024F;
            joint.AngularLimit__softness = 0.1F;
            joint.AngularLimit__bias = 0.99F;
            joint.AngularLimit__relaxation = 0.1F;
            joint.Params__bias = 0.99F;

            MeshInstance joint_ind = generate_joint_mesh(String.Format("{0}_indicator", joint.Name));

            joint.AddChild(joint_ind);

            joints.Add(joint);
        }

        return joints;
    }

    private List<Spatial> create_links(int N)
    {
        List<Spatial> links = new List<Spatial>();
        PhysicsMaterial link_phys_mat = new PhysicsMaterial();

        SpatialMaterial link_mesh_mat = new SpatialMaterial();
        link_mesh_mat.AlbedoColor = new Color(0.5F, 0.5F, 0.5F, 1F);
        for (int i = 0; i <= N; i++)
        {
            RigidBody link = new RigidBody();
            link.Name = String.Format("l{0}", i);
            link.CanSleep = false;
            link.ContinuousCd = true;

            Vector3 link_dimensions = new Vector3(0.2F, 0.8F, 0.1F);

            MeshInstance link_mesh = create_link_mesh(
                String.Format("{0}_mesh", link.Name),
                link_dimensions,
                link_mesh_mat);
            CollisionShape link_col = create_link_collision(
                String.Format("{0}_col", link.Name),
                link_dimensions);

            link.AddChild(link_mesh);
            link.AddChild(link_col);

            links.Add(link);
        }
        return links;
    }

    private void print_list(List<Spatial> list)
    {
        foreach (Spatial link in list)
        {
            GD.Print(String.Format("{0} - {1}", link.Name, link.GetType().ToString()));
            foreach (Spatial child in link.GetChildren())
            {
                GD.Print(String.Format("\t{0} - {1}", child.Name, child.GetType().ToString()));
            }
        }
    }

    private void print_list(List<Joint> list)
    {
        foreach (Spatial link in list)
        {
            GD.Print(String.Format("{0} - {1}", link.Name, link.GetType().ToString()));
            foreach (Spatial child in link.GetChildren())
            {
                GD.Print(String.Format("\t{0} - {1}", child.Name, child.GetType().ToString()));
            }
        }
    }

    private MeshInstance create_link_mesh(
        string mesh_name,
        Vector3 mesh_shape,
        SpatialMaterial mesh_mat)
    {
        MeshInstance ret_mesh = new MeshInstance();
        ret_mesh.Name = mesh_name;

        CubeMesh cmesh = new CubeMesh();
        cmesh.Size = mesh_shape;
        cmesh.Material = mesh_mat;

        ret_mesh.Mesh = cmesh;

        return ret_mesh;
    }

    private CollisionShape create_link_collision(
        string collision_name,
        Vector3 col_shape)
    {
        CollisionShape ret_col = new CollisionShape();
        ret_col.Name = collision_name;

        BoxShape cshape = new BoxShape();
        cshape.Extents = col_shape / 2;

        ret_col.Shape = cshape;

        return ret_col;
    }

    // Creates the static body that acts as the
    // base of the robot.
    private StaticBody generate_static(string name)
    {
        StaticBody node = new StaticBody();
        node.Name = name;
        PhysicsMaterial mat = new PhysicsMaterial();
        node.PhysicsMaterialOverride = mat;

        Vector3 stat_size = new Vector3(0.7F, 0.2F, 0.7F);

        MeshInstance node_mesh = create_link_mesh(
            String.Format("{0}_mesh", name),
            stat_size,
            new SpatialMaterial()
        );
        CollisionShape node_col = create_link_collision(
            String.Format("{0}_col", name),
            stat_size
        );

        node.AddChild(node_mesh);
        node.AddChild(node_col);
        return node;
    }

    private MeshInstance generate_joint_mesh(string joint_name)
    {
        MeshInstance j_mesh = generate_joint_mesh();
        j_mesh.Name = joint_name + "_mesh";

        return j_mesh;
    }

    private MeshInstance generate_joint_mesh()
    {
        MeshInstance j_mesh = new MeshInstance();

        // Create sphere
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

    public override void _PhysicsProcess(float delta)
    {
        if (build_robot)
        {
            build_robot = false;
            GenerateNDofArmHelper(rand.RandiRange(1,3));
        }
    }
}
