using Godot;
using System;

public class TransformInspector : VBoxContainer
{
    [Signal]
    public delegate void XTrans(float newX);
    [Signal]
    public delegate void YTrans(float newY);
    [Signal]
    public delegate void ZTrans(float newZ);

    [Signal]
    public delegate void XRot(float newX);
    [Signal]
    public delegate void YRot(float newY);
    [Signal]
    public delegate void ZRot(float newZ);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode("MarginContainer/VBoxContainer/TranslateInputs/transXIn").Connect("text_entered", this, "sendXTrans");
        GetNode("MarginContainer/VBoxContainer/TranslateInputs/transYIn").Connect("text_entered", this, "sendYTrans");
        GetNode("MarginContainer/VBoxContainer/TranslateInputs/transZIn").Connect("text_entered", this, "sendZTrans");

        GetNode("MarginContainer/VBoxContainer/RotateInputs/rotXIn").Connect("text_entered", this, "sendXRot");
        GetNode("MarginContainer/VBoxContainer/RotateInputs/rotYIn").Connect("text_entered", this, "sendYRot");
        GetNode("MarginContainer/VBoxContainer/RotateInputs/rotZIn").Connect("text_entered", this, "sendZRot");
    }

    private void sendXTrans(string newVal)
    {
        this.EmitSignal("XTrans", newVal.ToFloat());
    }
    private void sendYTrans(string newVal)
    {
        this.EmitSignal("YTrans", newVal.ToFloat());
    }
    private void sendZTrans(string newVal)
    {
        this.EmitSignal("ZTrans", newVal.ToFloat());
    }
    private void sendXRot(string newVal)
    {
        this.EmitSignal("XRot", newVal.ToFloat());
    }
    private void sendYRot(string newVal)
    {
        this.EmitSignal("YRot", newVal.ToFloat());
    }
    private void sendZRot(string newVal)
    {
        this.EmitSignal("ZRot", newVal.ToFloat());
    }
}
