using Godot;
using System;

public class gizmos : Spatial
{
    private Camera mainCam;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mainCam = GetNode<Camera>("../Camera/CameraObj");
    }

    public override void _Input(InputEvent @event)
    {
        // GetNode("/root")._Input(@event);
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
    
    var fov = mainCam.Fov;

    float distance = this.GlobalTransform.origin.DistanceTo(mainCam.GlobalTransform.origin);

    this.Scale = new Vector3(distance / 4, distance / 4, distance / 4);
 }
}
