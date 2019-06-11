using Godot;
using System;

public class gizmos : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Camera mainCam;

    bool softfocus = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // TODO: Find some way to make this dynamic instead of a static path
        // mainCam = GetNode<Camera>("/root/main/AppWindow/EnvironmentContainer/Viewport/env/Camera");
        mainCam = GetNode<Camera>("/root/main/UI/AppWindow/EnvironmentContainer/4way/HSplitContainer/ViewportContainer/Viewport/Camera");
        // mainCam = GetNode<Camera>("../Camera");

        Connect("mouse_entered", this, "OnMouseEnter");
        Connect("mouse_exited", this, "OnMouseExit");
        
        GD.Print("GIZMOS.CS: READY");
    }

    public void OnMouseEnter()
    {
        softfocus = true;
    }

    public void OnMouseExit()
    {
        softfocus = false;
    }



    public override void _GuiInput(InputEvent @event)
    {
        GD.Print("EVENT: " + @event);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        if(softfocus) 
        {
            GD.Print("SOFTFOCUS");
        }

        // for(var x = 0; x < gizmo.GetChildCount(); ++x) {
            // this.GetChild<Spatial>(0).GlobalTranslate(new Vector3(0, 0, 0) - gizmo.GetChild<Spatial>(0).GlobalTransform.origin);
        // }
        var distance = this.GetChild<Spatial>(0).GlobalTransform.origin.DistanceTo(mainCam.GlobalTransform.origin);

        // this.GlobalScale(new Vector3(distance / 4, distance / 4, distance / 4));

        for(var x = 0; x < this.GetChildCount(); ++x) {
            this.GetChild<Spatial>(x).Scale = new Vector3(distance / 4, distance / 4, distance / 4); 
        }
        // this.Scale = new Vector3(distance / 4, distance / 4, distance / 4); 

        // this.LookAt(new Vector3(0, 0, -1), new Vector3(0, 1, 0));
    }
}
