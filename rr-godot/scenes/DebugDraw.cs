using Godot;
using System;

public class DebugDraw : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public enum DebugDrawType
    {
        Disable = 0, Unshaded = 1, Overdraw = 2, Wireframe = 3
    }
    private DebugDrawType currentDrawType = DebugDrawType.Disable;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }


    private void toolbarChangeRendTypePressed(int id)
    {
        currentDrawType = (DebugDrawType) id;
        switch(currentDrawType)
        {
            case DebugDrawType.Disable:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Disabled;
                GD.Print(currentDrawType);
                
                break;

            case DebugDrawType.Overdraw:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Overdraw;
                GD.Print(currentDrawType);
                
                break;

            case DebugDrawType.Unshaded:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Unshaded;
                GD.Print(currentDrawType);
                
                break;
            case DebugDrawType.Wireframe:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Wireframe;
                GD.Print(currentDrawType);

                break;
            default:
                GD.Print("Unrecognized Menu Item");
                break;
        }
        
        

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
