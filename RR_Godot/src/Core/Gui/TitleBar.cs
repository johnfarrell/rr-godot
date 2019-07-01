using Godot;
using System;

public class TitleBar : Control
{
    MenuButton FileButton;
    MenuButton EditButton;
    MenuButton ViewButton;
    MenuButton HelpButton;

    [Signal]
    public delegate void PreferencesPressed();

    WindowDialog PreferencesWindow;

    FileDialog ImportWindow;

    private bool Following = false;
    private Vector2 DragStart = new Vector2();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Set up the menu bar items
        FileButton = GetNode<MenuButton>("MenuBar/btnFile");
        PopupMenu FileButtonPopup = FileButton.GetPopup();
        EditButton = GetNode<MenuButton>("MenuBar/btnEdit");
        PopupMenu EditButtonPopup = EditButton.GetPopup();
        ViewButton = GetNode<MenuButton>("MenuBar/btnView");
        HelpButton = GetNode<MenuButton>("MenuBar/btnHelp");

        // Populate file
        FileButtonPopup.AddItem("New");
        FileButtonPopup.AddItem("Open");
        FileButtonPopup.AddItem("Import");
        FileButtonPopup.AddSeparator();
        FileButtonPopup.AddItem("Save");
        FileButtonPopup.AddItem("Save As");
        FileButtonPopup.AddSeparator();
        FileButtonPopup.AddItem("Preferences");
        FileButtonPopup.AddSeparator();
        FileButtonPopup.AddItem("Exit");
        FileButtonPopup.Connect("id_pressed", this, "FileButtonPressed");

        PreferencesWindow = GetNode<WindowDialog>("/root/main/UI/PreferencesWindow");
        ImportWindow = GetNode<FileDialog>("/root/main/UI/ImportWindow");
        // Populate edit

        // Create Insert shape submenu
        PopupMenu AddShapeMenu = new PopupMenu();
        AddShapeMenu.Name = "Shape";
        AddShapeMenu.AddItem("Square");
        AddShapeMenu.AddItem("Sphere");
        AddShapeMenu.AddItem("Cylinder");
        AddShapeMenu.AddItem("Prism");
        AddShapeMenu.AddItem("Capsule");
        EditButtonPopup.AddChild(AddShapeMenu);

        EditButtonPopup.AddSubmenuItem("Add Shape", "Shape");

        
        this.Connect("gui_input", this, "TitleBarGUIInputHandler");
        GD.Print("TITLEBAR.CS: READY");
    }

    public void FileButtonPressed(int id)
    {   
        GD.Print(id);
        switch (id)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                ImportWindow.PopupCentered();
                break;
            case 4:
                break;
            case 7:
                PreferencesWindow.PopupCentered();
                EmitSignal("PreferencesPressed");
                break;
            case 8:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Handles input events in the tile bar. Mainly used for dragging the window
    /// while it's minimized.
    /// </summary>
    /// <param name="@event">Godot InputEvent containing event params</param>
    public void TitleBarGUIInputHandler(InputEvent @event)
    {
        
        if(@event is InputEventMouseButton)
        {
            GD.Print(@event);
            InputEventMouseButton eventArgs = (InputEventMouseButton) @event;
            if(eventArgs.ButtonIndex == 1)
            {
                if(eventArgs.Pressed)
                {
                    Following = true;
                    DragStart = GetLocalMousePosition();
                }
                else
                {
                    Following = false;
                    DragStart = new Vector2();
                }
            }

        }
        if(@event is InputEventMouseMotion && Following)
        {
            OS.WindowPosition += GetGlobalMousePosition() - DragStart;
        }
    }

    public override void _Notification(int what)
    {
        switch(what)
        {
            case MainLoop.NotificationWmMouseExit:
                // Used to handle edge case where user tries to move
                // the window too fast.
                if(Following)
                {
                    OS.WindowPosition += GetGlobalMousePosition() - DragStart;
                }
                break;
            default:
                break;
        }
    }   

    // Quitting is handled in /src/common/Global.cs to allow
    // saving settings on quit

    /// <summary>
    /// Handles maximizing the program
    /// </summary>
    public void MaximizeButtonPressedHandler()
    {
        OS.WindowMinimized = false;
        OS.WindowMaximized = !OS.WindowMaximized;
    }

    /// <summary>
    /// Handles minimizing the program
    /// </summary>
    public void MinimizeButtonPressedHandler()
    {
        OS.WindowMinimized = true;
    }
}