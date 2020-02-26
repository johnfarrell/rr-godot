using Godot;
using System;

public class Gen6DoFJointControllerDetail : VBoxContainer
{
    public enum JogAxis
    {
        PosX = 0,
        NegX = 1,
        PosY = 2,
        NegY = 3,
        PosZ = 4,
        NegZ = 5,
        None = 6
    }

    [Signal]
    public delegate void LinearAxisJogged(JogAxis axis);
    [Signal]
    public delegate void AngularAxisJogged(JogAxis axis);

    private Button hideBtn;
    private MarginContainer content;
    private Control spacer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hideBtn = (Button)GetNode("Button");
        content = (MarginContainer)GetNode("Content");
        spacer = (Control)GetNode("Spacer");

        hideBtn.Connect("pressed", this, "ToggleHidden");

        // Connect a lot of signals for the jog buttons
        content.GetNode("VBoxContainer/JogLin/PosX").Connect("pressed", this, "jogLinPosXPressed");
        content.GetNode("VBoxContainer/JogLin/NegX").Connect("pressed", this, "jogLinNegXPressed");
        content.GetNode("VBoxContainer/JogLin/PosY").Connect("pressed", this, "jogLinPosYPressed");
        content.GetNode("VBoxContainer/JogLin/NegY").Connect("pressed", this, "jogLinNegYPressed");
        content.GetNode("VBoxContainer/JogLin/PosZ").Connect("pressed", this, "jogLinPosZPressed");
        content.GetNode("VBoxContainer/JogLin/NegZ").Connect("pressed", this, "jogLinNegZPressed");

        content.GetNode("VBoxContainer/JogAng/PosX").Connect("pressed", this, "jogAngPosXPressed");
        content.GetNode("VBoxContainer/JogAng/NegX").Connect("pressed", this, "jogAngNegXPressed");
        content.GetNode("VBoxContainer/JogAng/PosY").Connect("pressed", this, "jogAngPosYPressed");
        content.GetNode("VBoxContainer/JogAng/NegY").Connect("pressed", this, "jogAngNegYPressed");
        content.GetNode("VBoxContainer/JogAng/PosZ").Connect("pressed", this, "jogAngPosZPressed");
        content.GetNode("VBoxContainer/JogAng/NegZ").Connect("pressed", this, "jogAngNegZPressed");
    }

    public void Configure(Generic6DOFJoint joint)
    {
        hideBtn = (Button)GetNode("Button");
        content = (MarginContainer)GetNode("Content");
        spacer = (Control)GetNode("Spacer");
        hideBtn.Text = joint.Name;
    }

    private void jogLinPosXPressed()
    {
        EmitSignal("LinearAxisJogged", JogAxis.PosX);
    }
    private void jogLinNegXPressed()
    {
        EmitSignal("LinearAxisJogged", JogAxis.NegX);
    }
    private void jogLinPosYPressed()
    {
        EmitSignal("LinearAxisJogged", JogAxis.PosY);
    }
    private void jogLinNegYPressed()
    {
        EmitSignal("LinearAxisJogged", JogAxis.NegY);
    }
    private void jogLinPosZPressed()
    {
        EmitSignal("LinearAxisJogged", JogAxis.PosZ);
    }
    private void jogLinNegZPressed()
    {
        EmitSignal("LinearAxisJogged", JogAxis.NegZ);
    }

    private void jogAngPosXPressed()
    {
        EmitSignal("AngularAxisJogged", JogAxis.PosX);
    }
    private void jogAngNegXPressed()
    {
        EmitSignal("AngularAxisJogged", JogAxis.NegX);
    }
    private void jogAngPosYPressed()
    {
        EmitSignal("AngularAxisJogged", JogAxis.PosY);
    }
    private void jogAngNegYPressed()
    {
        EmitSignal("AngularAxisJogged", JogAxis.NegY);
    }
    private void jogAngPosZPressed()
    {
        EmitSignal("AngularAxisJogged", JogAxis.PosZ);
    }
    private void jogAngNegZPressed()
    {
        EmitSignal("AngularAxisJogged", JogAxis.NegZ);
    }

    private void ToggleHidden()
    {
        content.Visible = !content.Visible;
        spacer.Visible = !spacer.Visible;
    }
}
