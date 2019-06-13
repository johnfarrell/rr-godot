using Godot;
using System;

public class gizmos : Control
{
    [Export]
    Godot.Camera mainCam;

    [Export]
    public NodePath CamPath;

    public override void _Ready()
    {
        // TODO: Find some way to make this dynamic instead of a static path
        mainCam = GetNode<Godot.Camera>(CamPath);
        
        GD.Print("GIZMOS.CS: READY");
    }

    public override void _Process(float delta)
    {
        var distance = this.GetChild<Spatial>(0).GlobalTransform.origin.DistanceTo(mainCam.GlobalTransform.origin);

        for(var x = 0; x < this.GetChildCount(); ++x) {
            this.GetChild<Spatial>(x).Scale = new Vector3(distance / 4, distance / 4, distance / 4); 
        }
    }
}
