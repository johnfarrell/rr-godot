using Godot;
using System;

/// <summary>
/// GizmoMode
/// <para>Class that handles common actions of Gizmos</para>
/// <para>Extended by each gizmo mode to implement their own custom logic</para>
/// </summary>
public class GizmoMode : Spatial
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
    /// Holds a <ref>Mode</ref> enum object describing the type of gizmo
    /// </summary>
    protected static Mode HandleMode;

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

    /// <summary>
    /// Whether or not this gizmo is enabled on startup
    /// </summary>
    [Export]
    private bool EnabledByDefault = false;

    /// <summary>
    /// Available modes of this gizmo
    /// </summary>

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

    /// <summary>
    /// Handles when the mouse leaves the X handle of this gizmo
    /// <para>Unhighlights and sets <see cref="XHover" /> to false</para>
    /// </summary>
    public virtual void OnXHandleMouseExit()
    {
        XHover = false;
        UnhighlightHandle(Handles.X);
    }

    /// <summary>
    /// Handles when the mouse leaves the Y handle of this gizmo
    /// <para>Unhighlights and sets <see cref="YHover" /> to false</para>
    /// </summary>
    public virtual void OnYHandleMouseExit()
    {
        YHover = false;
        UnhighlightHandle(Handles.Y);
    }

    /// <summary>
    /// Handles when the mouse leaves the Z handle of this gizmo
    /// <para>Unhighlights and sets <see cref="ZHover" /> to false</para>
    /// </summary>
    public virtual void OnZHandleMouseExit()
    {
        ZHover = false;
        UnhighlightHandle(Handles.Z);
    }

    /// <summary>
    /// Handles when the mouse enters the X handle of this gizmo
    /// <para>Highlights and sets <see cref="XHover"/> to true</para>
    /// </summary>
    public virtual void OnXHandleMouseEnter()
    {
        XHover = true;
        HighlightHandle(HandleX);
    }

    /// <summary>
    /// Handles when the mouse leaves the Y handle of this gizmo
    /// <para>Highlights and sets <see cref="ZHover"/> to true</para>
    /// </summary>
    public virtual void OnYHandleMouseEnter()
    {
        YHover = true;
        HighlightHandle(HandleY);
    }

    /// <summary>
    /// Handles when the mouse leaves the Z handle of this gizmo
    /// <para>Highlights and sets <see cref="ZHover"/> to true</para>
    /// </summary>
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