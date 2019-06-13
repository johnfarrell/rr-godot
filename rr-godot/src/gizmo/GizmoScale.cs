using Godot;
using System;

public class GizmoScale : Gizmo
{
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetDefaults();
        GD.Print("GIZMOSCALE.CS: READY");
    }

    public override void ChangeManipType(int id)
    {
        if(id == 2)
        {
            GD.Print("Changing to Scale");
            ToggleEnabled(true);
        }
        else
        {
            ToggleEnabled(false);
        }
    }

    public override void InputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if(@event is InputEventMouseButton)
        {
            GizmoPressed = @event.IsActionPressed("mouse_left_click");

            if(ActiveAxis != Axis.NONE)
            {
                EmitSignal("HandlePressedStateChanged");
            }
        }
        if(@event is InputEventMouseMotion && GizmoPressed && ActiveAxis != Axis.NONE)
        {   
            InputEventMouseMotion Event = (InputEventMouseMotion) @event;

            Spatial tempObj = GetObject();
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