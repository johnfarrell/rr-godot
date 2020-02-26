using Godot;
using System;

public class ObjectInspector : Panel
{
    private LineEdit[] inputBoxes;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        inputBoxes = new LineEdit[6];
        var i = 0;
        foreach (var child in GetNode("VBoxContainer/TransformMenu/MarginContainer/VBoxContainer/TranslateInputs").GetChildren())
        {
            if(child.GetType().Equals(typeof(LineEdit)))
            {
                inputBoxes[i] = (LineEdit) child;
                ++i;
            }
        }
        foreach (var child in GetNode("VBoxContainer/TransformMenu/MarginContainer/VBoxContainer/RotateInputs").GetChildren())
        {
            if(child.GetType().Equals(typeof(LineEdit)))
            {
                inputBoxes[i] = (LineEdit) child;
                ++i;
            }
        }
    }

    public void ToggleInputDisabled()
    {
        
        foreach (var input in inputBoxes)
        {
            input.Editable = !input.Editable;
            if(input.FocusMode != FocusModeEnum.None)
            {
                input.FocusMode = FocusModeEnum.None;
            }
            else
            {
                input.FocusMode = FocusModeEnum.All;
            }
        }
    }
}
