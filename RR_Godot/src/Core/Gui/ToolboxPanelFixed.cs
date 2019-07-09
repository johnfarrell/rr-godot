using Godot;
using System;


public class ToolboxPanelFixed : Panel
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        /// Add the AddMesh pop-up menu items
        MenuButton addMeshButton = GetNode<MenuButton>("ToolboxContainer/AddMeshMenuButton");
        addMeshButton.GetPopup().AddItem("Cube");
        addMeshButton.GetPopup().AddItem("Sphere");
        addMeshButton.GetPopup().AddItem("Cylinder");
        addMeshButton.GetPopup().AddItem("Prism");
        addMeshButton.GetPopup().AddItem("Capsule");
        
        /// Connect the pressed signals to the environment
        addMeshButton.GetPopup().Connect("id_pressed", GetNode("/root/main/env/"), "toolbarAddMeshItemPressed");

        /// Addthe translate button to the fixed menu
        Button ModeTranslateButton = GetNode<Button>("ToolboxContainer/ModeTranslate");
        ModeTranslateButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Translate"), "ManipToggled");
        ModeTranslateButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Scale"), "TurnOffOnTrans");
        ModeTranslateButton.Connect("toggled", this, "ToggleScaleOnTrans");

        ///Connect and create the rotate gizmo button on the fixed menu
        Button ModeRotateButton = GetNode<Button>("ToolboxContainer/ModeRotate");
        ModeRotateButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Rotate"), "ManipToggled");

        ///Connect and create the scaling button on the fixed menu
        Button ModeScaleButton = GetNode<Button>("ToolboxContainer/ModeScale");
        ModeScaleButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Scale"), "ManipToggled");
        ModeScaleButton.Connect("toggled", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/gizmos/Translate"), "TurnOffOnScale");
        ModeScaleButton.Connect("toggled", this, "ToggleTransOnScale");


        

        GD.Print("TOOLBOXPANELFIXED.CS: READY");
    }
    /// <summary>
    /// Proccess is called whenever the scene changes, <paramref name = "delta"> depends on framerate
    /// Proccess is used here to update the fixed menu focus
    /// </summary>
    /// <param name = "delta">delta</param>
    public override void _Process(float delta)
    {
        if(this.GetNode<MenuButton>("ToolboxContainer/AddMeshMenuButton").HasFocus()) {
            GD.Print("TOOLBOX PANEL FOCUS ");
        }
    }
    /// <summary>
    /// Handles the highlighting when the scale mode toggle is selected
    /// </summary>
    /// <param name = "Toggled">Toggled: bool whether or not the toggle is selected</param>
    public void ToggleScaleOnTrans(bool Toggled)
    {   
        if(Toggled)
        {
            GetNode<Button>("ToolboxContainer/ModeScale").Pressed = false;
        }
    }

    /// <summary>
    /// Handles the highlighting when the translate mode toggle is selected
    /// </summary>
    /// <param name = "Toggled">Toggled: bool whether or not the toggle is selected</param>
    public void ToggleTransOnScale(bool Toggled)
    {
        if(Toggled)
        {
            GetNode<Button>("ToolboxContainer/ModeTranslate").Pressed = false;
        }
    }
}