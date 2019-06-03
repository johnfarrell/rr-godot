using Godot;
using System;

public class AxisLineGenerator : ImmediateGeometry
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Begin(Godot.Mesh.PrimitiveType.Lines);

        this.SetColor(Godot.Colors.Red);
        this.AddVertex(new Vector3(-1000, 0, 0));
        this.AddVertex(new Vector3(1000, 0, 0));
        
        this.SetColor(Godot.Colors.Green);
        this.AddVertex(new Vector3(0, -1000, 0));
        this.AddVertex(new Vector3(0, 1000, 0));
        
        this.SetColor(Godot.Colors.Blue);
        this.AddVertex(new Vector3(0, 0, -1000));
        this.AddVertex(new Vector3(0, 0, 1000));
        
        this.End();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
