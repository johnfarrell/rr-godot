using Godot;
using System;

public class Viewport1 : ViewportContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Panel toolbox;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //UpdateControlBoxPosition();
        toolbox = GetNode<Panel>("/root/main/AppWindow/EnvironmentContainer/4WayViewport/VerticalSplit/HSplit1/Viewport1/ToolboxPanel/");
        toolbox.Hide();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
    //     toolboxInToggle();
    // }




    public void _on_Viewport1_mouse_exited()
    {
        toolbox.Hide();
    }



    public void _on_Viewport1_mouse_entered()
    {
        toolbox.Show();
    }
}
