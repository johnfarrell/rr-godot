using System.Collections.Generic;

using Godot;
using RosSharp.Urdf;


namespace RR_Godot.Core.Urdf
{
    public class UrdfImporter
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
        /// <para>Parse</para>
        /// Parses a Urdf file into class RosSharp.Urdf.Robot member.
        /// </summary>
        /// <param name="file_name">Name of the Urdf file.</param>
        /// <returns>
        /// <para>True if the file was succesfully parsed.</para>
        /// <para>False if there was an error while parsing.<para>
        /// </returns>
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
        public RigidBody GenerateSpatial(UrdfNode base_node)
        {
            // Create the empty spatial node
            RigidBody rootSpat = new RigidBody();
            rootSpat.Name = base_node._name;

            // Add children recursively
            foreach (var child in base_node.GetChildren())
            {
                // Returns a joint connected to a rigid body
                Generic6DOFJoint childJoint = GenerateSpatialRec(child);
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
        private Generic6DOFJoint GenerateSpatialRec(UrdfNode base_node)
        {
            // Create the return joint
            Generic6DOFJoint finJoint = ConfigureJoint(base_node._joint);
            finJoint.Name = base_node._joint.name;

            // Create the return RigidBody
            RigidBody tempLink = base_node.CreateLink();
            finJoint.AddChild(tempLink);

            foreach (var child in base_node.GetChildren())
            {
                // This is the same as GenerateSpatial(), so look at that
                // function for the explanation.
                Generic6DOFJoint childJoint = GenerateSpatialRec(child);
                tempLink.AddChild(childJoint);

                childJoint.TranslateObjectLocal(new Vector3(
                    (float)child._joint.origin.Xyz[0],
                    (float)child._joint.origin.Xyz[2],
                    -1.0F * (float)child._joint.origin.Xyz[1]
                ));
                childJoint.RotateX((float)child._joint.origin.Rpy[0]);
                childJoint.RotateY((float)child._joint.origin.Rpy[2]);
                childJoint.RotateZ(-1.0F * (float)child._joint.origin.Rpy[1]);
            }

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
                if (curr.GetType() != typeof(Godot.Generic6DOFJoint))
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

                Generic6DOFJoint tempJoint = (Generic6DOFJoint)curr;

                // We have a joint, set the endpoints

                // A joints parent will always be a RigidBody.
                NodePath parentPath = curr.GetParent().GetPath();
                // A joint will always have only one child which is a RigidBody,
                // this is what we want the second endpoint to be.
                NodePath childPath = curr.GetChild(0).GetPath();

                tempJoint.SetNodeA(parentPath);
                tempJoint.SetNodeB(childPath);

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
        private Generic6DOFJoint ConfigureJoint(RosSharp.Urdf.Joint base_joint)
        {
            Generic6DOFJoint genJoint = new Generic6DOFJoint();

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

            // Limit all the axis, has the effect of making it a fixed joint.
            // All the limits will be equal, making it unable to move.
            // Doing this allows us to set limits only where we need to.
            genJoint.AngularLimitX__enabled = true;
            genJoint.AngularLimitY__enabled = true;
            genJoint.AngularLimitZ__enabled = true;
            genJoint.LinearLimitX__enabled = true;
            genJoint.LinearLimitY__enabled = true;
            genJoint.LinearLimitZ__enabled = true;

            UrdfPrint(base_joint.name + " AXIS: " + j_axis[0] + " " + j_axis[1] + " " + j_axis[2]);

            // Type comments taken from https://wiki.ros.org/urdf/XML/joint 
            switch (base_joint.type)
            {
                case "revolute":
                    // A hinge joint that rotates along the axis and has a
                    // limited range specified by the upper and lower limits.
                    if (j_axis[0] == 1.0)
                    {
                        genJoint.AngularLimitX__lowerAngle = (float)base_joint.limit.lower;
                        genJoint.AngularLimitX__upperAngle = (float)base_joint.limit.upper;
                    }
                    if (j_axis[1] == 1.0)
                    {
                        genJoint.AngularLimitZ__lowerAngle = (float)base_joint.limit.lower;
                        genJoint.AngularLimitZ__upperAngle = (float)base_joint.limit.upper;
                    }
                    if (j_axis[2] == 0.0)
                    {
                        genJoint.AngularLimitY__lowerAngle = (float)base_joint.limit.lower;
                        genJoint.AngularLimitY__upperAngle = (float)base_joint.limit.upper;
                    }
                    break;
                case "continuous":
                    // a continuous hinge joint that rotates around the axis 
                    // and has no upper and lower limits.
                    if (j_axis[0] == 1.0)
                    {
                        genJoint.AngularLimitX__enabled = false;
                    }
                    if (j_axis[1] == 1.0)
                    {
                        genJoint.AngularLimitZ__enabled = false;
                    }
                    if (j_axis[2] == 1.0)
                    {
                        genJoint.AngularLimitY__enabled = false;
                    }
                    break;
                case "prismatic":
                    // a sliding joint that slides along the axis, and has a
                    // limited range specified by the upper and lower limits. 
                    if (j_axis[0] == 1.0)
                    {
                        genJoint.LinearLimitX__lowerDistance = (float)base_joint.limit.lower;
                        genJoint.LinearLimitX__upperDistance = (float)base_joint.limit.upper;
                    }
                    if (j_axis[1] == 1.0)
                    {
                        genJoint.LinearLimitZ__lowerDistance = (float)base_joint.limit.lower;
                        genJoint.LinearLimitZ__upperDistance = (float)base_joint.limit.upper;
                    }
                    if (j_axis[2] == 1.0)
                    {
                        genJoint.LinearLimitY__lowerDistance = (float)base_joint.limit.lower;
                        genJoint.LinearLimitY__upperDistance = (float)base_joint.limit.upper;
                    }
                    break;
                case "fixed":
                    // This is not really a joint because it cannot move.
                    // All degrees of freedom are locked. This type of joint 
                    // does not require the axis, calibration, dynamics, 
                    // limits or safety_controller. 
                    break;
                case "floating":
                    // This joint allows motion for all 6 degrees of freedom.
                    genJoint.AngularLimitX__enabled = false;
                    genJoint.AngularLimitY__enabled = false;
                    genJoint.AngularLimitZ__enabled = false;
                    genJoint.LinearLimitX__enabled = false;
                    genJoint.LinearLimitY__enabled = false;
                    genJoint.LinearLimitZ__enabled = false;
                    break;
                case "planar":
                    // This joint allows motion in a plane perpendicular to the axis.
                    if (j_axis[0] == 1.0)
                    {
                        genJoint.LinearLimitY__upperDistance = (float)base_joint.limit.upper;
                        genJoint.LinearLimitY__lowerDistance = (float)base_joint.limit.lower;
                        genJoint.LinearLimitZ__upperDistance = (float)base_joint.limit.upper;
                        genJoint.LinearLimitZ__lowerDistance = (float)base_joint.limit.lower;
                    }
                    if (j_axis[1] == 1.0)
                    {
                        genJoint.LinearLimitX__upperDistance = (float)base_joint.limit.upper;
                        genJoint.LinearLimitX__lowerDistance = (float)base_joint.limit.lower;
                        genJoint.LinearLimitY__upperDistance = (float)base_joint.limit.upper;
                        genJoint.LinearLimitY__lowerDistance = (float)base_joint.limit.lower;
                    }
                    if (j_axis[2] == 1.0)
                    {
                        genJoint.LinearLimitX__upperDistance = (float)base_joint.limit.upper;
                        genJoint.LinearLimitX__lowerDistance = (float)base_joint.limit.lower;
                        genJoint.LinearLimitZ__upperDistance = (float)base_joint.limit.upper;
                        genJoint.LinearLimitZ__lowerDistance = (float)base_joint.limit.lower;
                    }
                    break;
                default:
                    GD.Print(base_joint.type);
                    break;
            }

            return genJoint;
        }
    }
}