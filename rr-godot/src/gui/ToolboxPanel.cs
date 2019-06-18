using Godot;
using System;

public class ToolboxPanel : Panel
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        
        MenuButton rendTypeButton = GetNode<MenuButton>("ToolboxContainer/RenderStyle");
        rendTypeButton.GetPopup().AddItem("Disabled");
        rendTypeButton.GetPopup().AddItem("Unshaded");
        rendTypeButton.GetPopup().AddItem("Overdraw");
        rendTypeButton.GetPopup().AddItem("Wireframe");


        rendTypeButton.GetPopup().Connect("id_pressed", GetNode("../DebugDraw/"), "toolbarChangeRendTypePressed");

        MenuButton cameraPerspectiveButton = GetNode<MenuButton>("ToolboxContainer/CameraPerspective");
        cameraPerspectiveButton.GetPopup().AddItem("Front");
        cameraPerspectiveButton.GetPopup().AddItem("Back");
        cameraPerspectiveButton.GetPopup().AddItem("Orthogonal On");
        cameraPerspectiveButton.GetPopup().AddItem("Orthogonal Off");

        cameraPerspectiveButton.GetPopup().Connect("id_pressed",GetNode<Control>("../DebugDraw/"),"toolbarChangePerspective");

        GD.Print("TOOLBOXPANEL.CS: READY");
    }

    // public override void _Process(float delta)
    // {
    //     // if(this.GetNode<MenuButton>("ToolboxContainer/AddMeshMenuButton").HasFocus()) {
    //     //     GD.Print("TOOLBOX PANEL FOCUS ");
    //     // }
    // }

    
}
