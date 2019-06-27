using Godot;
using System;
using RR_Godot.Core.Gizmo;

public class GizmoRotate : Gizmo
{

    private int LatestAngle;
    private Vector3 LatestClickStart;

    Vector3 RotationStart;

    Transform StartTrans;

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
        GD.Print("GIZMO SCALE INPUT EVENT");
        Godot.Camera cam = (Godot.Camera) camera;
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
                EmitSignal("HandlePressed");
            }
            if(@event.IsActionReleased("mouse_left_click"))
            {
                StartTrans = CurrentObject.GlobalTransform;
                EmitSignal("HandleUnpressed");
            }
        }
        if(@event is InputEventMouseMotion && GizmoPressed && ActiveAxis != Axis.NONE)
        {   
            GD.Print(LatestAngle);
            InputEventMouseMotion Event = (InputEventMouseMotion) @event;

            Godot.Spatial tempObj = GetObject();
            if(tempObj == null)
            {
                return;
            }
            if(CurrentObject == null)
            {
                CurrentObject = tempObj;
                StartTrans = CurrentObject.GlobalTransform;
            }
            else if(CurrentObject.Name != tempObj.Name)
            {
                CurrentObject = tempObj;
                StartTrans = CurrentObject.GlobalTransform;
            }
            if(CurrentObject.Name == "env")
            {
                return;
            }

            Vector2 Origin2d = cam.UnprojectPosition(CurrentObject.GlobalTransform.origin);
            Vector2 MousePos = Event.Position;

            Vector2 StartVector = ClickStart - Origin2d;
            Vector2 CurrVector = MousePos - Origin2d;

            // float AngleCurr = (float) (180 / Math.PI) * StartVector.AngleTo(CurrVector);
            float AngleCurr = StartVector.AngleTo(CurrVector);


            Vector3 PreviousObjectRotation = CurrentObject.RotationDegrees;
            Vector3 CurrentObjectRotation = PreviousObjectRotation;

            Transform rotTrans = StartTrans;

            Vector3 rotation = rotTrans.basis.GetEuler();

            if (ActiveAxis == Axis.X)
            {
                rotTrans.basis = StartTrans.basis.Rotated(new Vector3(1, 0, 0), AngleCurr);
                
            }
            else if (ActiveAxis == Axis.Y)
            {
                rotation.y = -1 * AngleCurr;
                rotTrans.basis = StartTrans.basis.Rotated(new Vector3(0, 1, 0), -1 *AngleCurr);
            }
            else
            {
                rotation.z = AngleCurr;
                rotTrans.basis = StartTrans.basis.Rotated(new Vector3(0, 0, 1), AngleCurr);
            }

            // rotTrans.basis = new Basis(rotation);
            CurrentObject.GlobalTransform = rotTrans;
            
        }
    }
}