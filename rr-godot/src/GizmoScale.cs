using Godot;
using System;

public class GizmoScale : GizmoMode
{
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        HandleMode = Mode.Scale;
        SetDefaults();
        GD.Print("GIZMOSCALE.CS: READY");

        Disable();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
