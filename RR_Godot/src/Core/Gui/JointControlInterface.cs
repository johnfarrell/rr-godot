using Godot;
using System;

public class JointControlInterface : Panel
{
    private PackedScene detailScene;

    private VBoxContainer jointList;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        detailScene = (PackedScene) ResourceLoader.Load("res://Godot/scenes/JointControllerDetail.tscn");
        jointList = (VBoxContainer) GetNode("Content/JointContainer/List");
        
    }

    public void AddJoint(Generic6DOFJoint joint)
    {
        VBoxContainer jointDetail = (VBoxContainer) detailScene.Instance();
        jointList.AddChild(jointDetail);
    }
    
    public void RemoveJoint(int index)
    {
        var child = jointList.GetChild(index);
        jointList.RemoveChild(child);
        child.Dispose();
    }

}
