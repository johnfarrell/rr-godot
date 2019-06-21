using Godot;
using System;

public class PreferencesWindow : WindowDialog
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Panel BackgroundShadow;

    private ItemList SettingsList;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Save the shadowbox panel for hiding/showing later
        BackgroundShadow = GetNode<Panel>("/root/main/UI/ShadowBox");
        SettingsList = GetNode<ItemList>("VBoxContainer/Panel/HSplitContainer/ItemList");

        SettingsList.AddItem("General");
        SettingsList.AddItem("Plugins");
        
        GetNode("../TitleBar").Connect("PreferencesPressed", this, "OnAboutToShow");
        GetNode("VBoxContainer/CloseButton").Connect("pressed", this, "ClosePopup");
        this.Connect("popup_hide", this, "OnPopupHide");

        this.GetCloseButton().Connect("pressed", this, "OnPopupClose");
    }

    public void OnAboutToShow()
    {
        BackgroundShadow.Visible = true;
    }

    public void OnPopupClose()
    {
        BackgroundShadow.Visible = false;
    }

    public void ClosePopup()
    {
        this.Visible = false;
        OnPopupClose();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
