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
        GetNode<CollisionShape>("CollisionShape").Disabled = false;

        // Set collision masks to layer 2 so the gizmos won't collide with anything
        this.CollisionMask = 0b10;
        this.CollisionLayer = 0b10;

        // Set visual instance so the gizmo will be rendered over everything in the
        // gizmo viewport
        GetNode<MeshInstance>("Handle").Layers = 0b10;
        
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
            // GD.Print(this);
        }
    }

    public void HoverHighlight()
    {
        GetNode<MeshInstance>("Handle").MaterialOverride = highlightMat;
    }

    public void HoverUnhighlight()
    {
        GetNode<MeshInstance>("Handle").MaterialOverride = originalMat;
    }
}
