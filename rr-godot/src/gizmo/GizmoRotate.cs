using Godot;
using System;

public class GizmoRotate : Gizmo
{

    private int LatestAngle;
    private Vector3 LatestClickStart;

    Vector3 RotationStart;

    float AngleStart;

    private Vector2 ClickStart;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LatestAngle = 0;
        SetDefaults();
        GD.Print("GIZMOROTATE.CS: READY");
    }

    public override void InputEvent(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        Camera cam = (Camera) camera;
        if(shape_idx != 0)
        {
            LatestAngle = shape_idx;
            LatestClickStart = click_position;
        }
        if(@event is InputEventMouseButton)
        {
            GizmoPressed = @event.IsActionPressed("mouse_left_click");
            ClickStart = cam.UnprojectPosition(click_position);
            

            if(ActiveAxis != Axis.NONE)
            {
                EmitSignal("HandlePressedStateChanged");
            }
        }
        if(@event is InputEventMouseMotion && GizmoPressed && ActiveAxis != Axis.NONE)
        {   
            GD.Print(LatestAngle);
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

            Vector2 Origin2d = cam.UnprojectPosition(CurrentObject.GlobalTransform.origin);
            Vector2 MousePos = Event.Position;

            Vector2 StartVector = ClickStart - Origin2d;
            Vector2 CurrVector = MousePos - Origin2d;

            float AngleCurr = (float) (180 / Math.PI) * StartVector.AngleTo(CurrVector);


            Vector3 PreviousObjectRotation = CurrentObject.RotationDegrees;
            Vector3 CurrentObjectRotation = PreviousObjectRotation;

            if (ActiveAxis == Axis.X)
            {
                CurrentObjectRotation.x = AngleCurr;
            }
            else if (ActiveAxis == Axis.Y)
            {
                CurrentObjectRotation.y = -1 * AngleCurr;
            }
            else
            {
                CurrentObjectRotation.z = AngleCurr;
            }


            CurrentObject.RotationDegrees = CurrentObjectRotation;
            
        }
    }
}
