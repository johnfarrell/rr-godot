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

    private LineEdit XTransIn;
    private LineEdit YTransIn;
    private LineEdit ZTransIn;
    private LineEdit XRotIn;
    private LineEdit YRotIn;
    private LineEdit ZRotIn;

    private String lastVal;

    private LineEdit[] inputsList;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        XTransIn = (LineEdit)GetNode("MarginContainer/VBoxContainer/TranslateInputs/transXIn");
        YTransIn = (LineEdit)GetNode("MarginContainer/VBoxContainer/TranslateInputs/transYIn");
        ZTransIn = (LineEdit)GetNode("MarginContainer/VBoxContainer/TranslateInputs/transZIn");
        XRotIn = (LineEdit)GetNode("MarginContainer/VBoxContainer/RotateInputs/rotXIn");
        YRotIn = (LineEdit)GetNode("MarginContainer/VBoxContainer/RotateInputs/rotYIn");
        ZRotIn = (LineEdit)GetNode("MarginContainer/VBoxContainer/RotateInputs/rotZIn");

        inputsList = new LineEdit[] { XTransIn, YTransIn, ZTransIn, XRotIn, YRotIn, ZRotIn };

        foreach (var Input in inputsList)
        {
            Input.Connect("focus_entered", this, "storeLast");
        }

        // Connect Text entered signal to an input validator/signal passer
        XTransIn.Connect("text_entered", this, "sendXTrans");
        YTransIn.Connect("text_entered", this, "sendYTrans");
        ZTransIn.Connect("text_entered", this, "sendZTrans");
        XRotIn.Connect("text_entered", this, "sendXRot");
        YRotIn.Connect("text_entered", this, "sendYRot");
        ZRotIn.Connect("text_entered", this, "sendZRot");

        this.Connect("XTrans", GetNode("/root/main/env"), "test");
    }

    private void storeLast()
    {
        foreach (var input in inputsList)
        {
            if (input.HasFocus())
            {
                lastVal = input.Text;
            }
        }
    }

    private bool ValidateInput(string input)
    {
        if(input.IsValidFloat())
        {
            return true;
        }
        return false;
    }

    private void sendXTrans(string newVal)
    {
        if (ValidateInput(newVal))
        {
            this.EmitSignal("XTrans", newVal.ToFloat());
        }
        else
        {
            XTransIn.Text = lastVal;
        }

    }
    private void sendYTrans(string newVal)
    {
        if (ValidateInput(newVal))
        {
            this.EmitSignal("YTrans", newVal.ToFloat());
        }
        else
        {
            YTransIn.Text = lastVal;
        }
    }
    private void sendZTrans(string newVal)
    {
        if (ValidateInput(newVal))
        {
            this.EmitSignal("ZTrans", newVal.ToFloat());
        }
        else
        {
            ZTransIn.Text = lastVal;
        }
    }
    private void sendXRot(string newVal)
    {
        if (ValidateInput(newVal))
        {
            this.EmitSignal("XRot", newVal.ToFloat());
        }
        else
        {
            XRotIn.Text = lastVal;
        }
    }
    private void sendYRot(string newVal)
    {
        if (ValidateInput(newVal))
        {
            this.EmitSignal("YRotot", newVal.ToFloat());
        }
        else
        {
            YRotIn.Text = lastVal;
        }
    }
    private void sendZRot(string newVal)
    {
        if (ValidateInput(newVal))
        {
            this.EmitSignal("ZRot", newVal.ToFloat());
        }
        else
        {
            ZRotIn.Text = lastVal;
        }
    }
}
