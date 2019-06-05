using Godot;
using System;

public class TitleBar : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private bool Following = false;
    private Vector2 DragStart = new Vector2();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("gui_input", this, "TitleBarGUIInputHandler");
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

    /// <summary>
    /// Handles quitting the program safely
    /// </summary>
    public void QuitButtonPressedHandler()
    {
        GetTree().Quit();
    }

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
