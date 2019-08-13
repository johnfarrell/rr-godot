using Godot;
using System;

public class VelocityInspector : VBoxContainer
{
    // References to labels
    private Label linX;
    private Label linY;
    private Label linZ;
    private Label angX;
    private Label angY;
    private Label angZ;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        linX = (Label) GetNode("MarginContainer/VBoxContainer/Linear/X");
        linY = (Label) GetNode("MarginContainer/VBoxContainer/Linear/Y");
        linZ = (Label) GetNode("MarginContainer/VBoxContainer/Linear/Z");
        angX = (Label) GetNode("MarginContainer/VBoxContainer/Angular/X");
        angY = (Label) GetNode("MarginContainer/VBoxContainer/Angular/Y");
        angZ = (Label) GetNode("MarginContainer/VBoxContainer/Angular/Z");
    }

    public void UpdateLinX(double val)
    {
        val = Math.Round(val, 6);
        linX.Text = val.ToString();
    }

    public void UpdateLinY(double val)
    {
        val = Math.Round(val, 6);
        linY.Text = val.ToString();
    }
    public void UpdateLinZ(double val)
    {
        val = Math.Round(val, 6);
        linZ.Text = val.ToString();
    }
    public void UpdateAngX(double val)
    {
        val = Math.Round(val, 6);
        angX.Text = val.ToString();
    }
    public void UpdateAngY(double val)
    {
        val = Math.Round(val, 6);
        angY.Text = val.ToString();
    }
    public void UpdateAngZ(double val)
    {
        val = Math.Round(val, 6);
        angZ.Text = val.ToString();
    }
}
