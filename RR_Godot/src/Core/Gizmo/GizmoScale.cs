using Godot;
using System;
using RR_Godot.Core.Gizmo;

public class GizmoScale : Gizmo
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetDefaults();
        GD.Print("GIZMOSCALE.CS: READY");
    }

    /// <summary>
    /// Handler for when the Translate gizmo is selected. Disables this gizmo
    /// </summary>
    /// <param name="trans">Whether or not to disable the gizmo</param>
    public void TurnOffOnTrans(bool trans)
    {
        if(trans)
        {
            this.Disable();
        }
    }

    public override void InputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        GD.Print("GIZMO SCALE INPUT EVENT");
        
        if(@event is InputEventMouseButton)
        {
            GizmoPressed = @event.IsActionPressed("mouse_left_click");

            if(ActiveAxis != Axis.NONE)
            {
                EmitSignal("HandlePressed");
            }
            if(@event.IsActionReleased("mouse_left_click"))
            {
                EmitSignal("HandleUnpressed");
            }
        }
        if(@event is InputEventMouseMotion && GizmoPressed && ActiveAxis != Axis.NONE)
        {   
            InputEventMouseMotion Event = (InputEventMouseMotion) @event;

            Godot.Spatial tempObj = GetObject();
            if(tempObj == null)
            {
                return;
            }
            if(CurrentObject == null)
            {
                CurrentObject = tempObj;
            }
            else if(CurrentObject.Name != tempObj.Name)
            {
                CurrentObject = tempObj;
            }
            if(CurrentObject.Name == "env")
            {
                return;
            }

            Vector3 PreviousObjectPosition = CurrentObject.Scale;
            Vector3 CurrentObjectPosition = PreviousObjectPosition;
            
            Vector2 MouseMoveDelta = Event.Relative * new Vector2((float) 0.01, (float) 0.01);

            CurrentObjectPosition += EditorViewport.GetCamera().GlobalTransform.basis.y * -MouseMoveDelta.y;
            CurrentObjectPosition += EditorViewport.GetCamera().GlobalTransform.basis.x * MouseMoveDelta.x;

            if (ActiveAxis == Axis.X)
            {
                CurrentObjectPosition.y = PreviousObjectPosition.y;
                CurrentObjectPosition.z = PreviousObjectPosition.z;
            }
            else if (ActiveAxis == Axis.Y)
            {
                CurrentObjectPosition.x = PreviousObjectPosition.x;
                CurrentObjectPosition.z = PreviousObjectPosition.z;
            }
            else
            {
                CurrentObjectPosition.x = PreviousObjectPosition.x;
                CurrentObjectPosition.y = PreviousObjectPosition.y;
            }

            CurrentObject.Scale = CurrentObjectPosition;
        }
    }
}