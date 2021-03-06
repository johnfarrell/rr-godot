using Godot;
using System;

public class HingeJointControllerDetail : VBoxContainer
{
    private Button hideBtn;
    private MarginContainer content;
    private Control spacer;

    [Signal]
    public delegate void TargetVelChanged(float newTarget);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hideBtn = (Button)GetNode("Button");
        content = (MarginContainer)GetNode("Content");
        spacer = (Control)GetNode("Spacer");

        hideBtn.Connect("pressed", this, "ToggleHidden");

        content.GetNode("VBoxContainer/Motor/TargVelInput").Connect("text_entered", this, "VelEntered");
    }

    public void Configure(Godot.HingeJoint joint)
    {
        hideBtn = (Button)GetNode("Button");
        content = (MarginContainer)GetNode("Content");
        spacer = (Control)GetNode("Spacer");

        hideBtn.Text = joint.Name;
        float upperLimDeg = joint.GetParam(Godot.HingeJoint.Param.LimitUpper);
        float lowerLimDeg = joint.GetParam(Godot.HingeJoint.Param.LimitLower);

        ((Label)content.GetNode("VBoxContainer/AngLimits/UpperVal")).Text =
            upperLimDeg.ToString();
        ((Label)content.GetNode("VBoxContainer/AngLimits/LowerVal")).Text =
            lowerLimDeg.ToString();
        ((LineEdit)content.GetNode("VBoxContainer/Motor/TargVelInput")).Text =
            joint.GetParam(Godot.HingeJoint.Param.MotorTargetVelocity).ToString();
        ((Label)content.GetNode("VBoxContainer/Motor/MaxImpulseVal")).Text =
            joint.GetParam(Godot.HingeJoint.Param.MotorMaxImpulse).ToString();
    }

    public void VelEntered(String newVal)
    {
        if (newVal.IsValidFloat())
        {
            EmitSignal("TargetVelChanged", newVal.ToFloat());
        }
        ((LineEdit)content.GetNode("VBoxContainer/Motor/TargVelInput")).ReleaseFocus();
    }

    private void ToggleHidden()
    {
        content.Visible = !content.Visible;
        spacer.Visible = !spacer.Visible;
    }
}
