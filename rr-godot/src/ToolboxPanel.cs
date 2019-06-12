using Godot;
using System;

public class ToolboxPanel : Panel
{

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
        addMeshButton.GetPopup().Connect("id_pressed", GetNode("/root/main/env"), "toolbarAddMeshItemPressed");

        MenuButton manipTypeButton = GetNode<MenuButton>("ToolboxContainer/ManipulationType");
        manipTypeButton.GetPopup().AddCheckItem("Translate");
        manipTypeButton.GetPopup().AddCheckItem("Rotate");
        manipTypeButton.GetPopup().AddCheckItem("Scale");

        manipTypeButton.GetPopup().SetItemChecked(0, true);

        manipTypeButton.GetPopup().Connect("id_pressed", GetNode("/root/main/env"), "toolbarChangeManipTypePressed");
        manipTypeButton.GetPopup().Connect("id_pressed", this, "UpdateManipType");


        MenuButton rendTypeButton = GetNode<MenuButton>("ToolboxContainer/RenderStyle");
        rendTypeButton.GetPopup().AddItem("Disabled");
        rendTypeButton.GetPopup().AddItem("Unshaded");
        rendTypeButton.GetPopup().AddItem("Overdraw");
        rendTypeButton.GetPopup().AddItem("Wireframe");

        rendTypeButton.GetPopup().Connect("id_pressed", GetNode("/root/main/env"), "toolbarChangeRendTypePressed");

        GD.Print("TOOLBOXPANEL.CS: READY");
    }

    public override void _Process(float delta)
    {
        if(this.GetNode<MenuButton>("ToolboxContainer/AddMeshMenuButton").HasFocus()) {
            GD.Print("TOOLBOX PANEL FOCUS ");
        }
    }

    public void UpdateManipType(int id)
    {
        for(var i = 0; i < 3; ++i)
        {
            if(i != id)
            {
                GetNode<MenuButton>("ToolboxContainer/ManipulationType").GetPopup().SetItemChecked(i, false);
            }
            else{
                GetNode<MenuButton>("ToolboxContainer/ManipulationType").GetPopup().SetItemChecked(i, true);
            }
        }
    }
}
