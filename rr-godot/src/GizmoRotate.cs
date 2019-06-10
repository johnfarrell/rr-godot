using Godot;
using System;

public class GizmoRotate : GizmoMode
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        HandleMode = Mode.Rotate;
        SetDefaults();

        GD.Print("GIZMOROTATE.CS: READY");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
