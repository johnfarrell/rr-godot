using Godot;
using System;

public class JointControllerDetail : VBoxContainer
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
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hideBtn = (Button) GetNode("Button");
        content = (MarginContainer) GetNode("Content");

        hideBtn.Connect("pressed", this, "ToggleHidden");

        // Connect a lot of signals for the jog buttons
        GetNode("Content/VBoxContainer/JogLin/PosX").Connect("pressed", this, "jogLinPosXPressed");
        GetNode("Content/VBoxContainer/JogLin/NegX").Connect("pressed", this, "jogLinNegXPressed");
        GetNode("Content/VBoxContainer/JogLin/PosY").Connect("pressed", this, "jogLinPosYPressed");
        GetNode("Content/VBoxContainer/JogLin/NegY").Connect("pressed", this, "jogLinNegYPressed");
        GetNode("Content/VBoxContainer/JogLin/PosZ").Connect("pressed", this, "jogLinPosZPressed");
        GetNode("Content/VBoxContainer/JogLin/NegZ").Connect("pressed", this, "jogLinNegZPressed");

        GetNode("Content/VBoxContainer/JogAng/PosX").Connect("pressed", this, "jogAngPosXPressed");
        GetNode("Content/VBoxContainer/JogAng/NegX").Connect("pressed", this, "jogAngNegXPressed");
        GetNode("Content/VBoxContainer/JogAng/PosY").Connect("pressed", this, "jogAngPosYPressed");
        GetNode("Content/VBoxContainer/JogAng/NegY").Connect("pressed", this, "jogAngNegYPressed");
        GetNode("Content/VBoxContainer/JogAng/PosZ").Connect("pressed", this, "jogAngPosZPressed");
        GetNode("Content/VBoxContainer/JogAng/NegZ").Connect("pressed", this, "jogAngNegZPressed");
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
    }
}
