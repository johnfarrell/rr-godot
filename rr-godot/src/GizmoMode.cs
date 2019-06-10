using Godot;
using System;

public class GizmoMode : Spatial
{
    // Class to hold the materials for each handle and the highlight material
    class Materials
    {
        public static Material Highlight = (Material) GD.Load("res://theme/gizmo_HandleHighlight.tres");
        public static Material XHandle = (Material) GD.Load("res://theme/gizmo_XHandleMat.tres");
        public static Material YHandle = (Material) GD.Load("res://theme/gizmo_YHandleMat.tres");
        public static Material ZHandle = (Material) GD.Load("res://theme/gizmo_ZHandleMat.tres");
    }

    protected StaticBody HandleX { get; set; }

    protected StaticBody HandleY { get; set; }

    protected StaticBody HandleZ { get; set; }

    protected static Mode HandleMode;

    protected bool XHover = false;
    protected bool YHover = false;
    protected bool ZHover = false;

    [Export]
    private bool EnabledByDefault = false;

    protected enum Mode
    {
        Translate = 0,
        Rotate = 1,
        Scale = 2
    }

    public enum Handles
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    /// <summary>
    /// Sets the MaterialOverride of the specified <paramref name="handle"/>
    /// mesh instance to the highlight material.
    /// </summary>
    /// <param name="handle">
    /// Handles enum describing what handle to highlight
    /// </param>
    public void HighlightHandle(StaticBody handle)
    {
        handle.GetNode<MeshInstance>("Handle").MaterialOverride = Materials.Highlight;
    }

    /// <summary>
    /// Similar to HighlightHandle, this sets the Material override of the specified handle to the default
    /// material for the handle.
    /// </summary>
    /// <param name="handle">
    /// Handles enum describing what handle to reset
    /// </param>
    public void UnhighlightHandle(Handles handle)
    {
        switch(handle)
        {
            case Handles.X:
                HandleX.GetNode<MeshInstance>("Handle").MaterialOverride = Materials.XHandle;
                break;
            case Handles.Y:
                HandleY.GetNode<MeshInstance>("Handle").MaterialOverride = Materials.YHandle;
                break;
            case Handles.Z:
                HandleZ.GetNode<MeshInstance>("Handle").MaterialOverride = Materials.ZHandle;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Disables drawing and collision checking for the gizmo.
    /// </summary>
    public void Disable()
    {   
        // Disable collision checking on the handles
        HandleX.GetNode<CollisionShape>("CollisionShape").Disabled = true;
        HandleY.GetNode<CollisionShape>("CollisionShape").Disabled = true;
        HandleZ.GetNode<CollisionShape>("CollisionShape").Disabled = true;

        // Set the handles to invisible.
        HandleX.Visible = false;
        HandleY.Visible = false;
        HandleZ.Visible = false;
    }

    public virtual void OnXHandleMouseExit()
    {
        XHover = false;
        UnhighlightHandle(Handles.X);
    }

    public virtual void OnYHandleMouseExit()
    {
        YHover = false;
        UnhighlightHandle(Handles.Y);
    }

    public virtual void OnZHandleMouseExit()
    {
        ZHover = false;
        UnhighlightHandle(Handles.Z);
    }

    public virtual void OnXHandleMouseEnter()
    {
        XHover = true;
        HighlightHandle(HandleX);
    }

    public virtual void OnYHandleMouseEnter()
    {
        YHover = true;
        HighlightHandle(HandleY);
    }

    public virtual void OnZHandleMouseEnter()
    {
        ZHover = true;
        HighlightHandle(HandleZ);
    }

    /// <summary>
    /// Populates the handle variables with the relevant static bodies and
    /// sets the collision masking and visual masking.
    /// </summary>        
    public void SetDefaults()
    {
        GD.Print(HandleMode);
        HandleX = GetNode<StaticBody>("HandleX");
        HandleY = GetNode<StaticBody>("HandleY");
        HandleZ = GetNode<StaticBody>("HandleZ");

        // Set collision layers to layer 2 so the handles don't
        // collide with anything.
        HandleX.CollisionMask = 0b10;
        HandleX.CollisionLayer = 0b10;

        HandleY.CollisionMask = 0b10;
        HandleY.CollisionLayer = 0b10;

        HandleZ.CollisionMask = 0b10;
        HandleZ.CollisionLayer = 0b10;

        // Set visual layers to draw handles over whole environment.
        HandleX.GetNode<MeshInstance>("Handle").Layers = 0b10;
        HandleY.GetNode<MeshInstance>("Handle").Layers = 0b10;
        HandleZ.GetNode<MeshInstance>("Handle").Layers = 0b10;

        // Connect mouse enter/exit signals
        HandleX.Connect("mouse_entered", this, "OnXHandleMouseEnter");
        HandleY.Connect("mouse_entered", this, "OnYHandleMouseEnter");
        HandleZ.Connect("mouse_entered", this, "OnZHandleMouseEnter");

        HandleX.Connect("mouse_exited", this, "OnXHandleMouseExit");
        HandleY.Connect("mouse_exited", this, "OnYHandleMouseExit");
        HandleZ.Connect("mouse_exited", this, "OnZHandleMouseExit");

        // if(!EnabledByDefault)
        // {
        //     Disable();
        // }
    }
}