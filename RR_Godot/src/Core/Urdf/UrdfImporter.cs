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
                // Returns a joint connected to a rigid body
                Generic6DOFJoint childJoint = GenerateSpatialRec(child);
                rootSpat.AddChild(childJoint);

                // Transform according to the child joint transformations
                childJoint.TranslateObjectLocal(new Vector3(
                    (float) child._joint.origin.Xyz[0],
                    (float) child._joint.origin.Xyz[2],
                    (float) child._joint.origin.Xyz[1]
                ));
                childJoint.RotateX((float) child._joint.origin.Rpy[0]);
                childJoint.RotateY((float) child._joint.origin.Rpy[2]);
                childJoint.RotateZ(-1.0F * (float) child._joint.origin.Rpy[1]);
            }

            return rootSpat;
        }

        public Generic6DOFJoint GenerateSpatialRec(UrdfNode base_node)
        {
            // Create the return joint
            Generic6DOFJoint finJoint = ConfigureJoint(base_node._joint);
            finJoint.Name = base_node._joint.name;

            // Create the return RigidBody
            GLink tempLink = base_node.CreateGLink();
            finJoint.AddChild(tempLink._rigidBody);
            

            foreach (var child in base_node.GetChildren())
            {
                Generic6DOFJoint childJoint = GenerateSpatialRec(child);
                tempLink._rigidBody.AddChild(childJoint);

                childJoint.TranslateObjectLocal(new Vector3(
                    (float) child._joint.origin.Xyz[0],
                    (float) child._joint.origin.Xyz[2],
                    (float) child._joint.origin.Xyz[1]
                ));
                childJoint.RotateX((float) child._joint.origin.Rpy[0]);
                childJoint.RotateY((float) child._joint.origin.Rpy[2]);
                childJoint.RotateZ(-1.0F * (float) child._joint.origin.Rpy[1]);
            }

            return finJoint;
        }

        private Generic6DOFJoint ConfigureJoint(RosSharp.Urdf.Joint base_joint)
        {
            Generic6DOFJoint genJoint = new Generic6DOFJoint();
            double[] j_axis;
            try 
            {
                j_axis = base_joint.axis.xyz;
            }
            catch
            {
                j_axis = new double[] {1.0, 0.0, 0.0};
            }

            // Limit all the axis
            genJoint.AngularLimitX__enabled = true;
            genJoint.AngularLimitY__enabled = true;
            genJoint.AngularLimitZ__enabled = true;

            

            UrdfPrint(base_joint.name + " AXIS: " + j_axis[0] + " " + j_axis[1] + " " + j_axis[2]);

            // Type comments taken from https://wiki.ros.org/urdf/XML/joint 
            switch (base_joint.type)
            {
                case "revolute":
                    // A hinge joint that rotates along the axis and has a
                    // limited range specified by the upper and lower limits.
                    if(j_axis[0] == 1.0)
                    {
                        genJoint.AngularLimitX__lowerAngle = (float) base_joint.limit.lower;
                        genJoint.AngularLimitX__upperAngle = (float) base_joint.limit.upper;
                    }
                    if(j_axis[1] == 1.0)
                    {
                        genJoint.AngularLimitY__lowerAngle = (float) base_joint.limit.lower;
                        genJoint.AngularLimitY__upperAngle = (float) base_joint.limit.upper;
                    }
                    if(j_axis[2] == 0.0)
                    {
                        genJoint.AngularLimitZ__lowerAngle = (float) base_joint.limit.lower;
                        genJoint.AngularLimitZ__upperAngle = (float) base_joint.limit.upper;
                    }
                    
                    break;
                case "continuous":
                    // a continuous hinge joint that rotates around the axis 
                    // and has no upper and lower limits.
                    GD.Print("c");
                    break;
                case "prismatic":
                    // a sliding joint that slides along the axis, and has a
                    // limited range specified by the upper and lower limits. 
                    GD.Print("p");
                    break;
                case "fixed":
                    // This is not really a joint because it cannot move.
                    // All degrees of freedom are locked. This type of joint 
                    // does not require the axis, calibration, dynamics, 
                    // limits or safety_controller. 
                    GD.Print("f");
                    break;
                case "floating":
                    // This joint allows motion for all 6 degrees of freedom. 
                    GD.Print("fl");
                    break;
                case "planar":
                    // This joint allows motion in a plane perpendicular to the axis. 
                    GD.Print("pl");
                    break;
                default:
                    GD.Print(base_joint.type);
                    break;
            }

            return genJoint;
        }
    }
}