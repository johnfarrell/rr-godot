using Godot;
using System;

public class AxisLineGenerator : ImmediateGeometry
{
    [Export]
    Color XAxisHighlight = Godot.Colors.Red;

    [Export]
    Color YAxisHighlight = Godot.Colors.Green;

    [Export]
    Color ZAxisHighlight = Godot.Colors.Blue;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Begin(Godot.Mesh.PrimitiveType.Lines);

        this.SetColor(XAxisHighlight);
        this.AddVertex(new Vector3(-1000, 0, 0));
        this.AddVertex(new Vector3(1000, 0, 0));
        
        this.SetColor(YAxisHighlight);
        this.AddVertex(new Vector3(0, -1000, 0));
        this.AddVertex(new Vector3(0, 1000, 0));
        
        this.SetColor(ZAxisHighlight);
        this.AddVertex(new Vector3(0, 0, -1000));
        this.AddVertex(new Vector3(0, 0, 1000));
        
        this.End();
    }
}
