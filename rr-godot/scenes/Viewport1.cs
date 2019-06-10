using Godot;
using System;

public class Viewport1 : ViewportContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateControlBoxPosition();
        

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void UpdateControlBoxPosition()
    {
        Panel cbNode = GetNode<Panel>("/root/main/AppWindow/EnvironmentContainer/4WayViewport/VerticalSplit/HSplit1/Viewport1/ToolboxPanel");
        ViewportContainer envContainer = cbNode.GetParent<ViewportContainer>();

        // Get the necessary size and position values
        Vector2 initialCbPos = cbNode.RectPosition;
        Vector2 cbSize = cbNode.RectSize;
        Vector2 viewportDimensions = envContainer.RectSize;

        // Calculate the new x value to align the horizontal midpoints of the control box
        // and environment container.
        float newX = ((viewportDimensions.x) / (float) 2.0) - (cbSize.x / (float) 2.0);

        cbNode.SetPosition(new Vector2(newX, initialCbPos.y));
    }
}
