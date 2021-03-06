using Godot;
using System;

public class AppWindow : HSplitContainer
{
    int MaxSplitOffset = 100;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateSplitOffset();
        UpdateControlBoxPosition();

        GetNode("EnvironmentContainer").Connect("resized", this, "EnvironmentContainerResizeHandler");
        GetNode("/root/main").Connect("resized", this, "WindowResizeHandler");

        GD.Print("APPWINDOW.CS: READY");
    }

    /// <summary>
    /// Resizes the HSplitContainer split to be (1/4 viewport size - size of left menu) or
    /// MaxSplitOffset, whichever is smaller.
    /// </summary>
    private void UpdateSplitOffset()
    {
        Vector2 viewportDimensions = GetViewportRect().Size;
        SplitOffset = Math.Min(((int) viewportDimensions.x / 4) - 225, MaxSplitOffset);
    }

    /// <summary>
    /// Updates the position of the Toolbox menu for interacting with the environment.
    /// </summary>
    private void UpdateControlBoxPosition()
    {

        Panel cbNode = GetNode<Panel>("/root/main/UI/AppWindow/EnvironmentContainer/ToolboxPanelFixed");
        Control envContainer = cbNode.GetParent<Control>();

        Vector2 ButtonRowSize = cbNode.GetNode<HBoxContainer>("ToolboxContainer").RectSize;
        float ButtonRowMarginTop = cbNode.GetNode<HBoxContainer>("ToolboxContainer").MarginTop;
        float ButtonRowMarginLeft = cbNode.GetNode<HBoxContainer>("ToolboxContainer").MarginLeft;
        
        cbNode.SetSize(ButtonRowSize + new Vector2(ButtonRowMarginLeft, ButtonRowMarginTop));

        // Get the necessary size and position values
        Vector2 initialCbPos = cbNode.RectPosition;
        Vector2 cbSize = cbNode.RectSize;
        Vector2 viewportDimensions = envContainer.RectSize;

        // Calculate the new x value to align the horizontal midpoints of the control box
        // and environment container.
        float newX = ((viewportDimensions.x) / (float) 2.0) - (cbSize.x / (float) 2.0);

        cbNode.SetPosition(new Vector2(newX, initialCbPos.y));
    }
    
    /// <summary>
    /// Signal handler for resized() signal sent by program root spacial node.
    /// </summary>
    public void WindowResizeHandler()
    {
        UpdateSplitOffset();
        UpdateControlBoxPosition();
    }

    /// <summary>
    /// Signal handler for resized() signal sent by the environment container node.
    /// </summary>
    public void EnvironmentContainerResizeHandler()
    {
        UpdateControlBoxPosition();
    }
}
