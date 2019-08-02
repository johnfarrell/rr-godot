using System.Collections.Generic;

using Godot;
using RosSharp.Urdf;


namespace RR_Godot.Core.Urdf
{
    public class UrdfImporter
    {
        Robot _robot;

        public UrdfNode _robotRoot;

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

        public bool Parse(string file_name)
        {
            try
            {
                _robot = new Robot(file_name);
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

        public Spatial GenerateSpatial(UrdfNode base_node)
        {
            // Create the empty spatial node
            Spatial rootSpat = new Spatial();
            rootSpat.Name = base_node._name;

            // Add children recursively
            foreach (var child in base_node.GetChildren())
            {
                // Create a Godot joint

                // Create a GLink
                GLink tempLink = child.CreateGLink();
            }

            return rootSpat;
        }
    }
}