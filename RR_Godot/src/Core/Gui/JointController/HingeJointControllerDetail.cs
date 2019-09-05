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
        hideBtn = (Button) GetNode("Button");
        content = (MarginContainer) GetNode("Content");
        spacer = (Control) GetNode("Spacer");

        hideBtn.Connect("pressed", this, "ToggleHidden");

        content.GetNode("VBoxContainer/Motor/TargVelInput").Connect("text_changed", this, "VelChanged");
    }

    public void Configure(HingeJoint joint)
    {
        hideBtn.Text = joint.Name;
        ((Label) content.GetNode("VBoxContainer/AngLimits/UpperVal")).Text = 
            joint.GetParam(HingeJoint.Param.LimitUpper).ToString();
        ((Label) content.GetNode("VBoxContainer/AngLimits/LowerVal")).Text = 
            joint.GetParam(HingeJoint.Param.LimitLower).ToString();
        ((LineEdit) content.GetNode("VBoxContainer/Motor/TargVelInput")).Text =
            joint.GetParam(HingeJoint.Param.MotorTargetVelocity).ToString();
    }

    public void VelChanged(String newVal)
    {
        if(VerifyInput(newVal))
        {
            GD.Print("Velocity changed!");
            EmitSignal("TargetVelChanged", newVal.ToFloat());
        }
    }

    private bool VerifyInput(string input)
    {
        return input.IsValidFloat();
    }

    private void ToggleHidden()
    {
        content.Visible = !content.Visible;
        spacer.Visible = !spacer.Visible;
    }
}
