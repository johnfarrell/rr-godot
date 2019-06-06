using Godot;
using System;

public class GizmoHandleHover : StaticBody
{
    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2,
        ALL = 3
    }

    [Export]
    Axis handleAxis = Axis.ALL; 
    Material originalMat;
    Material highlightMat;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<CollisionShape>("CollisionShape").Disabled = true;
        
        highlightMat = (Material) GD.Load("res://theme/gizmo_HandleHighlight.tres");
        originalMat = GetNode<MeshInstance>("Handle").MaterialOverride;
        this.Connect("mouse_entered", this, "HoverHighlight");
        this.Connect("mouse_exited", this, "HoverUnhighlight");

        GD.Print("GIZMOHANDLEHOVER.CS: READY");
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton && @event.IsAction("mouse_left_click"))
        {
            // GD.Print("GIZMOHANDLEHOVER.CS: " + this.GetParent().Name + " - " + handleAxis);
        }
    }

    public void HoverHighlight()
    {
        GD.Print("here");
        GetNode<MeshInstance>("Handle").MaterialOverride = highlightMat;
    }

    public void HoverUnhighlight()
    {
        GD.Print("hifoe");
        GetNode<MeshInstance>("Handle").MaterialOverride = originalMat;
    }
}
