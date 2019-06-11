using Godot;
using System;

public class GizmoTranslate : GizmoMode
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        HandleMode = Mode.Translate;
        SetDefaults();

        GD.Print("GIZMOTRANSLATE.CS: READY");

        HandleX.Connect("input_event", this, "OnXHandleInputEvent");
        HandleY.Connect("input_event", this, "OnYHandleInputEvent");
        HandleZ.Connect("input_event", this, "OnZHandleInputEvent");
    }

    public void OnXHandleInputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if(@event is InputEventMouseButton)
        {
            GD.Print("X PRESS");
        }
    }

    public void OnYHandleInputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if(@event is InputEventMouseButton)
        {
            GD.Print("Y PRESS");
        }
    }

    public void OnZHandleInputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if(@event is InputEventMouseButton)
        {
            GD.Print("Z PRESS");
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
