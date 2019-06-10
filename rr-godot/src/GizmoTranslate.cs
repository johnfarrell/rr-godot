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

        HandleX.Connect("mouse_entered", this, "OnXHandleMouseEnter");
        HandleX.Connect("mouse_exited", this, "OnXHandleMouseExit");
        HandleX.Connect("input_event", this, "OnXHandleInputEvent");

        GD.Print(GetIncomingConnections());
    }

    public override void OnXHandleMouseEnter()
    {
        HighlightHandle(Handles.X);
    }

    public override void OnXHandleMouseExit()
    {
        UnhighlightHandle(Handles.X);
    }

    public override void OnXHandleInputEvent(InputEvent @event)
    {
        GD.Print(@event);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
