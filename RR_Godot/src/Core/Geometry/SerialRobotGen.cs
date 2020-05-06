// ------ SerialRobotGen.cs ------
// Author: John Farrell
//          john@johnjfarrell.com
//
// This class is used to generate basic N-DoF arms
// with basic links. This is what is called when
// the 'Create' button is clicked in the toolbar.
// Not very practical at all, but used as a first
// draft of generating robots programmatically.
// I'm keeping it in here for legacy purposes and
// references, as it is essentially a simpler version
// of the UrdfImporter class. They have the same basic
// creation logic which will probably be useful
// in the future.

using Godot;
using System;
using System.Collections.Generic;

namespace RR_Godot.Core.Geometry
{
    public static class SerialRobotGen
    {
        static int joint_count;
        static int link_count;
        static string bot_name;

        static RandomNumberGenerator rand;

        private static void DEBUG_PRINT(string msg)
        {
            GD.Print(String.Format("{0} GEN : {1}", bot_name, msg));
        }

        private static void configure(int dof)
        {
            rand = new RandomNumberGenerator();
            joint_count = dof;
            link_count = dof - 1;
            bot_name = String.Format("{0}_dof_arm", joint_count);
        }

        private static void configure(int dof, string name)
        {
            configure(dof);
            bot_name = name;
        }

        private static StaticBody generate_base(Vector3 dims)
        {
            string base_name = String.Format("{0}_base", bot_name);

            DEBUG_PRINT("Generating static base...");

            StaticBody node = new StaticBody();
            node.Name = base_name;

            PhysicsMaterial mat = new PhysicsMaterial();
            node.PhysicsMaterialOverride = mat;

            MeshInstance base_mesh = create_link_mesh(
                String.Format("{0}_mesh", base_name),
                dims,
                new SpatialMaterial()
            );
            CollisionShape base_col = create_link_collision(
                String.Format("{0}_col", base_name),
                dims
            );

            node.AddChild(base_mesh);
            node.AddChild(base_col);

            DEBUG_PRINT("Finished generating base.");

            return node;
        }

        private static MeshInstance create_link_mesh(
            string name,
            Vector3 dims,
            SpatialMaterial mat)
        {
            MeshInstance link_mesh = new MeshInstance();
            link_mesh.Name = name;

            CubeMesh cmesh = new CubeMesh();
            cmesh.Size = dims;
            cmesh.Material = mat;

            link_mesh.Mesh = cmesh;
            return link_mesh;
        }

        private static CollisionShape create_link_collision(
            string name,
            Vector3 dims)
        {
            CollisionShape link_col = new CollisionShape();
            link_col.Name = name;

            BoxShape cshape = new BoxShape();
            cshape.Extents = dims / 2;

            link_col.Shape = cshape;

            return link_col;
        }

        private static List<Spatial> generate_links()
        {
            List<Spatial> links = new List<Spatial>();

            PhysicsMaterial phys_mat = new PhysicsMaterial();
            SpatialMaterial spat_mat = new SpatialMaterial();
            spat_mat.AlbedoColor = new Color(0.5F, 0.5F, 0.5F, 1F);

            DEBUG_PRINT("Generating links...");
            for (int i = 0; i <= link_count; i++)
            {
                RigidBody link = new RigidBody();
                link.Name = String.Format("l{0}", i);
                link.CanSleep = false;
                link.ContinuousCd = true;

                Vector3 link_dimensions = new Vector3(0.2F, 0.8F, 0.1F);

                MeshInstance link_mesh = create_link_mesh(
                    String.Format("{0}_mesh", link.Name),
                    link_dimensions,
                    spat_mat
                );
                CollisionShape link_col = create_link_collision(
                    String.Format("{0}_col", link.Name),
                    link_dimensions
                );

                link.AddChild(link_mesh);
                link.AddChild(link_col);

                links.Add(link);
            }
            DEBUG_PRINT("Finished link generation.");

            return links;
        }

        private static List<Joint> generate_joints()
        {
            List<Joint> joints = new List<Joint>();
            DEBUG_PRINT("Generating joints...");
            for (int i = 0; i < joint_count; i++)
            {
                HingeJoint joint = new HingeJoint();
                joint.Name = String.Format("j{0}", i);

                joint.Motor__enable = true;
                joint.Motor__targetVelocity = 1F;
                joint.Motor__maxImpulse = 1024F;
                
                joint.AngularLimit__bias = 0.99F;
                joint.AngularLimit__softness = 0.1F;
                joint.AngularLimit__relaxation = 0.1F;
                joint.Params__bias = 0.99F;

                MeshInstance joint_indicator = generate_joint_mesh(
                    String.Format("{0}_indicator", joint.Name)
                );

                joint.AddChild(joint_indicator);

                joints.Add(joint);
            }
            DEBUG_PRINT("Finished joint generation.");

            return joints;
        }

        private static MeshInstance generate_joint_mesh(string name)
        {
            MeshInstance j_mesh = new MeshInstance();

            SphereMesh m_mesh = new SphereMesh();
            m_mesh.Radius = 0.05F;
            m_mesh.Height = 0.1F;
            m_mesh.RadialSegments = 5;
            m_mesh.Rings = 5;

            SpatialMaterial m_mat = new SpatialMaterial();
            m_mat.AlbedoColor = new Color(0.26F, 0.32F, 0.92F);
            m_mat.EmissionEnabled = true;
            m_mat.Emission = new Color(0.4F, 0.3F, 0.1F);
            m_mat.EmissionEnergy = 3F;

            j_mesh.Mesh = m_mesh;
            j_mesh.MaterialOverride = m_mat;

            return j_mesh;
        }


        public static void generate_generic(
            int dof,
            Spatial parent,
            Vector3 origin)
        {
            configure(dof);
            // Create Static base
            StaticBody base_node = generate_base(new Vector3(0.7F, 0.2F, 0.7F));

            List<Spatial> links = generate_links();
            List<Joint> joints = generate_joints();

            parent.AddChild(base_node);
            base_node.GlobalTranslate(origin);
            base_node.GlobalRotate(new Vector3(0F, 1F, 0F), rand.RandfRange(-3.14F, 3.14F));

            Joint base_joint = joints[0];
            base_node.AddChild(base_joint);

            for(int i = 1; i < dof; i++)
            {
                Spatial link = links[i-1];
                Joint joint = joints[i];

                link.AddChild(joint);
                base_joint.AddChild(link);
            }
            base_joint.AddChild(links[dof-1]);

            transform_all(base_node, base_joint);
            // connect_joints(joints, links, base_node, base_joint, dof);
        }

        private static void transform_all(Spatial base_node, Joint base_joint)
        {

        }
    
    }
}