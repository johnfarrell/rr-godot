using Godot;
using System;

/// <summary>
/// GizmoMode
/// <para>Class that handles common actions of Gizmos</para>
/// <para>Extended by each gizmo mode to implement their own custom logic</para>
/// </summary>
public class Gizmo : Spatial
{
    /// <summary>
    /// Class to hold the material locations for each handle along with the highlight
    /// material
    /// </summary>
    class Materials
    {
        public static Material Highlight = (Material) GD.Load("res://theme/gizmo_HandleHighlight.tres");
        public static Material XHandle = (Material) GD.Load("res://theme/gizmo_XHandleMat.tres");
        public static Material YHandle = (Material) GD.Load("res://theme/gizmo_YHandleMat.tres");
        public static Material ZHandle = (Material) GD.Load("res://theme/gizmo_ZHandleMat.tres");
    }

    [Signal]
    public delegate void HandlePressedStateChanged();

    public Viewport EditorViewport;

    /// <summary>
    /// Node of the X Handle for this gizmo
    /// </summary>
    protected StaticBody HandleX { get; set; }
    /// <summary>
    /// Node of the Y Handle for this gizmo
    /// </summary>
    protected StaticBody HandleY { get; set; }
    /// <summary>
    /// Node of the Z Handle for this gizmo
    /// </summary>
    protected StaticBody HandleZ { get; set; }

    /// <summary>
    /// Whether or not the X handle is currently hovered over by the user
    /// </summary>
    protected bool XHover = false;
    /// <summary>
    /// Whether or not the Y handle is currently hovered over by the user
    /// </summary>
    protected bool YHover = false;
    /// <summary>
    /// Whether or not the Z handle is currently hovered over by the user
    /// </summary>
    protected bool ZHover = false;

    protected bool GizmoPressed = false;
    protected Axis ActiveAxis = Axis.NONE;

    /// <summary>
    /// Holds the node of the object that is selected
    /// </summary>
    protected Spatial CurrentObject { get; set; }

    /// <summary>
    /// Whether or not this gizmo is enabled on startup
    /// </summary>
    [Export]
    private bool EnabledByDefault = false;

    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2,
        NONE = 3
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
    public void UnhighlightHandle(Axis handle)
    {
        switch(handle)
        {
            case Axis.X:
                HandleX.GetNode<MeshInstance>("Handle").MaterialOverride = Materials.XHandle;
                break;
            case Axis.Y:
                HandleY.GetNode<MeshInstance>("Handle").MaterialOverride = Materials.YHandle;
                break;
            case Axis.Z:
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
        ToggleEnabled(false);
    }

    public void ToggleEnabled(bool Enable)
    {
        HandleX.GetNode<CollisionShape>("CollisionShape").Disabled = !Enable;
        HandleY.GetNode<CollisionShape>("CollisionShape").Disabled = !Enable;
        HandleZ.GetNode<CollisionShape>("CollisionShape").Disabled = !Enable;

        // Set the handles to invisible.
        HandleX.Visible = Enable;
        HandleY.Visible = Enable;
        HandleZ.Visible = Enable;
    }

    /// <summary>
    /// Handles when the mouse leaves the X handle of this gizmo
    /// <para>Unhighlights and sets <see cref="XHover" /> to false</para>
    /// </summary>
    public virtual void OnXHandleMouseExit()
    {
        XHover = false;
        ActiveAxis = Axis.NONE;
        UnhighlightHandle(Axis.X);
    }

    /// <summary>
    /// Handles when the mouse leaves the Y handle of this gizmo
    /// <para>Unhighlights and sets <see cref="YHover" /> to false</para>
    /// </summary>
    public virtual void OnYHandleMouseExit()
    {
        YHover = false;
        ActiveAxis = Axis.NONE;
        UnhighlightHandle(Axis.Y);
    }

    /// <summary>
    /// Handles when the mouse leaves the Z handle of this gizmo
    /// <para>Unhighlights and sets <see cref="ZHover" /> to false</para>
    /// </summary>
    public virtual void OnZHandleMouseExit()
    {
        ZHover = false;
        ActiveAxis = Axis.NONE;
        UnhighlightHandle(Axis.Z);
    }

    /// <summary>
    /// Handles when the mouse enters the X handle of this gizmo
    /// <para>Highlights and sets <see cref="XHover"/> to true</para>
    /// </summary>
    public virtual void OnXHandleMouseEnter()
    {
        XHover = true;
        ActiveAxis = Axis.X;
        HighlightHandle(HandleX);
    }

    /// <summary>
    /// Handles when the mouse enters the Y handle of this gizmo
    /// <para>Highlights and sets <see cref="ZHover"/> to true</para>
    /// </summary>
    public virtual void OnYHandleMouseEnter()
    {
        YHover = true;
        ActiveAxis = Axis.Y;
        HighlightHandle(HandleY);
    }

    /// <summary>
    /// Handles when the mouse enter the Z handle of this gizmo
    /// <para>Highlights and sets <see cref="ZHover"/> to true</para>
    /// </summary>
    public virtual void OnZHandleMouseEnter()
    {
        ZHover = true;
        ActiveAxis = Axis.Z;
        HighlightHandle(HandleZ);
    }

    /// <summary>
    /// Gets the object the gizmo is set to manipulate.
    /// <para>Call GetObject().Type to get what type of Node it is</para>
    /// </summary>
    public Spatial GetObject()
    {
        Node env = GetNode<Spatial>("/root/main/env");
        Node marker = env.FindNode("SelectedObject", true, false);

        if(marker == null)
        {
            return null;
        }
    
        return (Spatial) marker.GetParent();
    }

    public virtual void ChangeManipType(int id) { }

    public virtual void InputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) { }

    /// <summary>
    /// Populates the handle variables with the relevant static bodies and
    /// sets the collision masking and visual masking.
    /// </summary>        
    public void SetDefaults()
    {
        EditorViewport = GetNode<Viewport>("/root/main/UI/AppWindow/EnvironmentContainer/4way/HSplitContainer/ViewportContainer/Viewport");

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

        HandleX.Connect("input_event", this, "InputEvent");
        HandleY.Connect("input_event", this, "InputEvent");
        HandleZ.Connect("input_event", this, "InputEvent");

        if(!EnabledByDefault)
        {
            Disable();
        }
    }
}