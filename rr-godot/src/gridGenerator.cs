using Godot;
using System;

public class gridGenerator : ImmediateGeometry
{
    [Export]
    float gridStep = 1;

    [Export]
    int gridHighlightStep = 10;

    [Export]
    int gridSize = 30;

    [Export]
    Color mainLine = new Color((float) 0.2, (float) 0.2, (float) 0.2);

    [Export]
    Color highlightLine = new Color((float) 0.4, (float) 0.4, (float) 0.4);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {   
        
        this.Begin(Godot.Mesh.PrimitiveType.Lines, null);
        // Draw the grid lines
        for(float i = gridSize * -1; i <= gridSize; i += gridStep)
        {
            // Skip zero to avoid double-drawing X-Z lines
            if(i == 0) { 
                continue; 
            }
            // Highlight every 'gridHighlightStep' gridlines
            if(i % gridHighlightStep == 0) {
                this.SetColor(highlightLine);
            }
            else { 
                this.SetColor(mainLine);
            }

            this.AddVertex(new Vector3(i, 0, gridSize * -1));
            this.AddVertex(new Vector3(i, 0, gridSize));

            this.AddVertex(new Vector3(gridSize * -1, 0, i));
            this.AddVertex(new Vector3(gridSize, 0, i));
        }
        this.End();
        GD.Print("GRIDGENERATOR.CS: READY");
    }
}
