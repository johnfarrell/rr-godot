using Godot;
using System;

public class ToolboxPanelFixed : Panel
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        // Add the AddMesh pop-up menu items
        MenuButton addMeshButton = GetNode<MenuButton>("ToolboxContainer/AddMeshMenuButton");
        addMeshButton.GetPopup().AddItem("Cube");
        addMeshButton.GetPopup().AddItem("Sphere");
        addMeshButton.GetPopup().AddItem("Cylinder");
        addMeshButton.GetPopup().AddItem("Prism");
        addMeshButton.GetPopup().AddItem("Capsule");

        // Connect the pressed signals to the environment
        addMeshButton.GetPopup().Connect("id_pressed", GetNode("/root/main/env/"), "toolbarAddMeshItemPressed");

        Button ModeTranslateButton = GetNode<Button>("ToolboxContainer/ModeTranslate");
        ModeTranslateButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Translate"), "ManipToggled");
        ModeTranslateButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Scale"), "TurnOffOnTrans");
        ModeTranslateButton.Connect("toggled", this, "ToggleScaleOnTrans");
    
        Button ModeRotateButton = GetNode<Button>("ToolboxContainer/ModeRotate");
        ModeRotateButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Rotate"), "ManipToggled");

        Button ModeScaleButton = GetNode<Button>("ToolboxContainer/ModeScale");
        ModeScaleButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Scale"), "ManipToggled");
        ModeScaleButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Translate"), "TurnOffOnScale");
        ModeScaleButton.Connect("toggled", this, "ToggleTransOnScale");


        

        GD.Print("TOOLBOXPANELFIXED_________________________________.CS: READY");
    }

    public override void _Process(float delta)
    {
        if(this.GetNode<MenuButton>("ToolboxContainer/AddMeshMenuButton").HasFocus()) {
            GD.Print("TOOLBOX PANEL FOCUS ");
        }
    }

    public void ToggleScaleOnTrans(bool Toggled)
    {   
        if(Toggled)
        {
            GetNode<Button>("ToolboxContainer/ModeScale").Pressed = false;
        }
    }

    public void ToggleTransOnScale(bool Toggled)
    {
        if(Toggled)
        {
            GetNode<Button>("ToolboxContainer/ModeTranslate").Pressed = false;
        }
    }
}
