using Godot;
using System;

public class JointControlManager : Panel
{
    private PackedScene GenDetailScene;
    private PackedScene HingeDetailScene;

    private bool MoveRequested;

    private float RequestedVel;

    private VBoxContainer jointList;

    private Godot.Joint ActiveJoint;

    [Signal]
    public delegate void MotorTargetChanged(float target, string joint_name);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GenDetailScene = (PackedScene)ResourceLoader.Load("res://Godot/scenes/Gen6DoFJointControllerDetail.tscn");
        HingeDetailScene = (PackedScene)ResourceLoader.Load("res://Godot/scenes/HingeJointControllerDetail.tscn");
        jointList = (VBoxContainer)GetNode("Content/JointContainer/List");
    }

    /// <summary>
    /// <para>AddJointDetail</para>
    /// Adds a joint detail to the joint inspector if there isn't currently a
    /// detail, or replaces the current joint detail with a new one.
    /// </summary>
    /// <param name="item">Godot.Joint object of the joint to add to the list.</param>
    public void AddJointDetail(Joint item)
    {
        if (JointAdded(item.Name))
        {
            return;
        }
        if (item.IsClass("Generic6DOFJoint"))
        {
            AddJoint((Generic6DOFJoint)item);
            GD.Print("Added GenJoint");
        }
        else if (item.IsClass("HingeJoint"))
        {
            AddJoint((Godot.HingeJoint)item);
            GD.Print("Added HingeJoint");
        }
    }

    // Determines whether or not a joint detail
    // of the name 'JointName' is already in
    // the detail list
    private bool JointAdded(string JointName)
    {
        Node joint = jointList.FindNode(JointName);
        return (joint != null);
    }


    // Adds and configures a Gen6DoFJointControllerDetail scene to the
    // detail list, replacing any current detail
    private void AddJoint(Godot.Generic6DOFJoint joint)
    {
        if (jointList.GetChildCount() == 1)
        {
            RemoveJoint(0);
        }
        try
        {
            Gen6DoFJointControllerDetail jointDetail = (Gen6DoFJointControllerDetail)GenDetailScene.Instance();
            jointDetail.Configure(joint);
            jointDetail.Name = joint.Name;
            jointList.AddChild(jointDetail);
            ActiveJoint = joint;
        }
        catch (Exception e)
        {
            GD.Print("Error adding joint...\n" + e.Message);
        }

    }

    // Adds a HingeJointControllerDetail scene to the detail list,
    // replacing the current detail if there is one.
    private void AddJoint(Godot.HingeJoint joint)
    {
        if (jointList.GetChildCount() == 1)
        {
            RemoveJoint(0);
        }
        try
        {
            HingeJointControllerDetail jointDetail = (HingeJointControllerDetail)HingeDetailScene.Instance();
            jointDetail.Configure(joint);
            jointDetail.Name = joint.Name;
            jointList.AddChild(jointDetail);
            jointDetail.Connect("TargetVelChanged", this, "HingeJointVelChanged");
            ActiveJoint = joint;
        }
        catch (Exception e)
        {
            GD.Print("Error adding joint...\n" + e.Message);
        }
    }

    public void HingeJointVelChanged(float vel)
    {
        EmitSignal("MotorTargetChanged", vel, ActiveJoint.Name);
    }

    

    private void RemoveJoint(int index)
    {
        var child = jointList.GetChild(index);
        jointList.RemoveChild(child);
        child.Dispose();
    }
}
