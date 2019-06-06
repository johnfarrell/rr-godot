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

        // GD.Print(GetNode("../Viewport/env").GetChild(0).GetParent().GetScript().ToString());

        // Connect the pressed signals to the environment
        // addMeshButton.GetPopup().Connect("id_pressed", GetNode("../Viewport/env/").GetChild(0).GetParent(), "toolbarAddMeshItemPressed");

        MenuButton manipTypeButton = GetNode<MenuButton>("ToolboxContainer/ManipulationType");
        manipTypeButton.GetPopup().AddCheckItem("Translate");
        manipTypeButton.GetPopup().AddCheckItem("Rotate");
        manipTypeButton.GetPopup().AddCheckItem("Scale");

        manipTypeButton.GetPopup().Connect("id_pressed", GetNode("../Viewport/env/"), "toolbarChangeManipTypePressed");
        manipTypeButton.GetPopup().Connect("id_pressed", this, "UpdateManipType");

        GD.Print("TOOLBOXPANEL.CS: READY");
    }

    public void UpdateManipType(int id)
    {
        GD.Print(id);
    }
}
