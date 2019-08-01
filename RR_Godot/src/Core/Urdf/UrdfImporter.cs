using System.Collections.Generic;

using Godot;
using RosSharp.Urdf;

namespace RR_Godot.Core.Urdf
{
    public class UrdfImporter
    {
        Robot _robot;

        private void UrdfPrint(string msg, int indent = 0)
        {
            string form_msg = " U\t";
            form_msg = form_msg.PadRight(indent * 2, ' ');
            form_msg += msg;
            GD.Print(form_msg);
        }

        private bool HandleJoints(List<RosSharp.Urdf.Joint> joint_list)
        {
            UrdfPrint("Parsing joints...");
            foreach (var joint in joint_list)
            {
                UrdfPrint("\tFound Joint: " + joint.name);
            }
            return false;
        }

        private bool HandleLinks(List<RosSharp.Urdf.Link> link_list)
        {
            UrdfPrint("Parsing links...");
            foreach (var link in link_list)
            {
                UrdfPrint("\tFound Link: " + link.name);

                UrdfPrint("\t  parsing link visuals...");
                foreach (var vis in link.visuals)
                {
                    try
                    {
                        UrdfPrint("\t\tVisual: " + vis.geometry.mesh.filename);
                    }
                    catch
                    {
                        // GD.Print("No visual data");
                    }
                }
            }
            return false;
        }

        private bool HandleMaterials(List<Link.Visual.Material> mat_list)
        {
            UrdfPrint("Parsing materials...");
            foreach (var mat in mat_list)
            {
                UrdfPrint("\tFound Material: " + mat.name);
            }

            return false;
        }

        /// <summary>
        /// <para>PrintTree</para>
        /// Recursively prints the tree structure of
        /// the Urdf file
        /// </summary>
        /// <param name="root">
        /// Root UrdfNode of the tree
        /// </param>
        /// <param name="level">
        /// Current level of recursion.
        /// </param>
        public void PrintTree(UrdfNode root, int level = 0)
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
        public UrdfNode CreateNodeTree()
        {
            // Create the root node
            UrdfNode rootNode = new UrdfNode(null, _robot.root, null);
            rootNode._isRoot = true;
            rootNode._name = _robot.root.name;

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

        public bool Parse(string file_name)
        {
            UrdfPrint("Parsing file: " + file_name);
            _robot = new Robot(file_name);

            UrdfPrint("Robot name: " + _robot.name);

            int child_count = _robot.root.joints.Count;
            string status = "Root " + _robot.root.name + " has " + child_count + " children";
            UrdfPrint(status);
            UrdfNode root = CreateNodeTree();

            PrintTree(root, 2);

            return false;
        }
    }
}