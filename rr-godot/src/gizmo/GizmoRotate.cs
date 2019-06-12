using Godot;
using System;

public class GizmoRotate : Gizmo
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetDefaults();
        GD.Print("GIZMOROTATE.CS: READY");
    }

    public override void _Input(InputEvent @event)
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

            Vector3 PreviousObjectRotation = CurrentObject.RotationDegrees;
            Vector3 CurrentObjectRotation = PreviousObjectRotation;
            
            Vector2 MouseMoveDelta = Event.Relative * new Vector2((float) 0.1, (float) 0.1);

            CurrentObjectRotation += CurrentObject.GlobalTransform.basis.x * MouseMoveDelta.x;
            CurrentObjectRotation += CurrentObject.GlobalTransform.basis.y * -MouseMoveDelta.y;

            if (ActiveAxis == Axis.X)
            {
                CurrentObjectRotation.y = PreviousObjectRotation.y;
                CurrentObjectRotation.z = PreviousObjectRotation.z;
            }
            else if (ActiveAxis == Axis.Y)
            {
                CurrentObjectRotation.x = PreviousObjectRotation.x;
                CurrentObjectRotation.z = PreviousObjectRotation.z;
            }
            else
            {
                CurrentObjectRotation.x = PreviousObjectRotation.x;
                CurrentObjectRotation.y = PreviousObjectRotation.y;
            }


            CurrentObject.RotationDegrees = CurrentObjectRotation;
            
        }
    }
}
