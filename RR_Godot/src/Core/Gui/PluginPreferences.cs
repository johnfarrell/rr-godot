using Godot;
using System;
using System.Linq;
using RR_Godot.Core;
using RR_Godot.Core.Plugins;

public class PluginPreferences : Panel
{
    private ItemList PluginItemList;

    private FileDialog PluginFolderSelector;

    private Global GlobalSettings;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PluginItemList = GetNode<ItemList>("VBoxContainer/ItemList");
        GlobalSettings = GetNode<Global>("/root/Global");
        PluginFolderSelector = GetNode<FileDialog>("FileDialog");

        PluginItemList.Connect("item_selected", this, "OnItemListItemPressed");
        
        // Disable the load and refresh buttons because we need to think through them more
        Button LoadButton = GetNode<Button>("VBoxContainer/SearchBar/HBoxContainer/LoadButton");
        LoadButton.Disabled = true;
        LoadButton.Connect("pressed", this, "SelectPluginFolder");

        Button RefreshButton = GetNode<Button>("VBoxContainer/SearchBar/HBoxContainer/RefreshButton");
        RefreshButton.Disabled = false;
        RefreshButton.Connect("pressed", this, "RefreshPluginList");

        this.Connect("item_rect_changed", this, "OnRectSizeChanged");

        PluginItemList.MaxColumns = 2;
        PluginItemList.SameColumnWidth = true;

        PluginItemList.FixedColumnWidth = (int) PluginItemList.RectSize.x / PluginItemList.MaxColumns;
        

        CreatePluginListHeader();
        RefreshPluginList(true);
    }

    public void OnRectSizeChanged()
    {
        GD.Print("---");
        GD.Print(PluginItemList.FixedColumnWidth);
        GD.Print(PluginItemList.RectSize.x);
        GD.Print(PluginItemList.MaxColumns);
        PluginItemList.FixedColumnWidth = (int) PluginItemList.RectSize.x / PluginItemList.MaxColumns;
    }

    public void SelectPluginFolder()
    {
        PluginFolderSelector.PopupCentered();
    }

    public void OnItemListItemPressed(int index)
    {
        int MaxCol = PluginItemList.MaxColumns;
        int rowIndex = (index / MaxCol) * MaxCol;

        var itemCount = PluginItemList.GetItemCount();

        for(var i = 2; i < itemCount; ++i)
        {
            PluginItemList.SetItemCustomBgColor(i, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }

        Color selectColor = new Color(0.6f, 0.6f, 0.6f);
        for(var i = 0; i < MaxCol; ++i)
        {
            PluginItemList.SetItemCustomBgColor(rowIndex + i, selectColor);
        }
    }

    public void CreatePluginListHeader()
    {
        PluginItemList.AddItem("Plugin Name");
        PluginItemList.AddItem("Enabled (y/n)");

        PluginItemList.SetItemSelectable(0, false);
        PluginItemList.SetItemSelectable(1, false);

        float headerColorVal = 0.3f;
        Color HeaderColor = new Color(headerColorVal, headerColorVal, headerColorVal);
        PluginItemList.SetItemCustomBgColor(0, HeaderColor);
        PluginItemList.SetItemCustomBgColor(1, HeaderColor);
    }

    /// <summary>
    /// Helper because Godot signals don't like connecting to functions
    /// that have arguments.
    /// </summary>
    public void RefreshPluginList()
    {
        RefreshPluginList(false);
    }

    public void RefreshPluginList(bool StartupCall)
    {
        PluginItemList.Clear();
        CreatePluginListHeader();

        // Used so we don't go through the plugin directory twice on startup
        if(!StartupCall)
        {
            GlobalSettings.CheckForNewPlugins();
        }

        string[] EnabledPluginNames = (string[]) GlobalSettings.UserConfig.GetValue("plugins", "enabled_plugins");
        foreach (string PluginName in EnabledPluginNames)
        {
            PluginItemList.AddItem(PluginName);
            PluginItemList.AddItem("y");
        }

        string[] DisabledPluginNames = (string[]) GlobalSettings.UserConfig.GetValue("plugins", "disabled_plugins");
        foreach (string PluginName in DisabledPluginNames)
        {
            PluginItemList.AddItem(PluginName);
            PluginItemList.AddItem("n");
        }

        // string[] InactivePluginNames = GlobalSettings.UserConfig.GetInactivePlugins();
        // foreach (string PluginName in InactivePluginNames)
        // {
        //     PluginItemList.AddItem(PluginName);
        //     PluginItemList.AddItem("i");
        // }
    }
}