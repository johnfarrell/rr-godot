using Godot;
using System;

public class Viewport2 : ViewportContainer
{
   private Panel toolbox;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        //UpdateControlBoxPosition();
        toolbox = GetNode<Panel>("/root/main/UI/AppWindow/EnvironmentContainer/4WayViewport/VerticalSplit/HSplit1/Viewport2/ToolboxPanel/");
        toolbox.Hide();
    }
    
    /// <summary>
    /// When the mouse exits the borders of this viewport, the toolbox contained in the viewport is hidden.
    /// </summary>
    public void _on_Viewport2_mouse_exited()
    {
       if(toolbox.GetLocalMousePosition().x > toolbox.GetEnd().x || toolbox.GetLocalMousePosition().y >toolbox.GetEnd().y)
        {
           toolbox.Hide();
        }
    }

    /// <summary>
    /// When the mouse enters the borders of the viewport, the toolbox is displayed.
    /// </summary>
    public void _on_Viewport2_mouse_entered()
    {
        toolbox.Show();
    }
}
