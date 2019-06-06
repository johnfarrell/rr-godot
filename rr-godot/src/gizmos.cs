using Godot;
using System;

public class gizmos : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Camera mainCam;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        mainCam = GetNode<Camera>("../Camera/CameraObj");

        GD.Print("GIZMOS.CS: READY");
    }
    public void _on_HandleX_input_event(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        GD.Print(@event);
    }

    public void OnHandleXMouseEnter()
    {
        GD.Print("ENTER");
    }

    public void OnHandleXMouseExit()
    {
        GD.Print("EXIT");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var distance = this.GlobalTransform.origin.DistanceTo(mainCam.GlobalTransform.origin);

        this.Scale = new Vector3(distance / 4, distance / 4, distance / 4);
    }
}
