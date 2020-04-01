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
