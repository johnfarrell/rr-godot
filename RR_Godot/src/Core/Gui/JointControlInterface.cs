using Godot;
using System;

public class JointControlInterface : Panel
{
    private PackedScene GenDetailScene;
    private PackedScene HingeDetailScene;

    private VBoxContainer jointList;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GenDetailScene = (PackedScene) ResourceLoader.Load("res://Godot/scenes/Gen6DoFJointControllerDetail.tscn");
        HingeDetailScene = (PackedScene) ResourceLoader.Load("res://Godot/scenes/HingeJointControllerDetail.tscn");
        jointList = (VBoxContainer) GetNode("Content/JointContainer/List");
    }

    public void TreeObjSelected(Joint item)
    {
        if(JointAdded(item.Name))
        {
            return;
        }
        if(item.IsClass("Generic6DOFJoint"))
        {
            AddJoint((Generic6DOFJoint) item);
            GD.Print("Added GenJoint");
        }
        else if(item.IsClass("HingeJoint"))
        {
            AddJoint((HingeJoint) item);
            GD.Print("Added HingeJoint");
        }
    }

    private bool JointAdded(string JointName)
    {
        Node joint = jointList.GetNode(JointName);
        
        return (joint != null);
    }

    public void AddJoint(Generic6DOFJoint joint)
    {
        try
        {
            Gen6DoFJointControllerDetail jointDetail = (Gen6DoFJointControllerDetail) GenDetailScene.Instance();
            jointDetail._Ready();
            jointDetail.Configure(joint);
            jointDetail.Name = joint.Name;
            jointList.AddChild(jointDetail);
        }
        catch (Exception e)
        {
            GD.Print("Error adding joint...\n" + e.Message);
        }
        
    }

    public void AddJoint(HingeJoint joint)
    {
        try
        {
            HingeJointControllerDetail jointDetail = (HingeJointControllerDetail) HingeDetailScene.Instance();
            jointDetail._Ready();
            jointDetail.Configure(joint);
            jointDetail.Name = joint.Name;
            jointList.AddChild(jointDetail);
        }
        catch (Exception e)
        {
            GD.Print("Error adding joint...\n" + e.Message);
        }
        
    }
    
    public void RemoveJoint(int index)
    {
        var child = jointList.GetChild(index);
        jointList.RemoveChild(child);
        child.Dispose();
    }

}
