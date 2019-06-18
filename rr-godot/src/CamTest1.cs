using Godot;
using System;

public class CamTest1 : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var scene = GD.Load<PackedScene>("res://theme/CamSensor.tscn");
        var node =  scene.Instance();
        Node container = node.GetChild(0);
        Camera cam = (Camera)container.GetChild(0);
        Vector3 loc = this.GetTranslation();
        loc.z = loc.z+10;
        cam = new Cam(" "," ",1,loc);
        GetChild(GetChildCount()-1).AddChild(node);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
