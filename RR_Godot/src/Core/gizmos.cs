using Godot;

public class gizmos : Spatial
{
    [Export]
    Godot.Camera mainCam;

    [Export]
    public NodePath CamPath;

    public override void _Ready()
    {
        // TODO: Find some way to make this dynamic instead of a static path
        var node = GetNode<Godot.Camera>("/root/main/UI/AppWindow/EnvironmentContainer/4WayViewport/VerticalSplit/HSplit1/Viewport1/Viewport/Camera/CameraObj");
        this.mainCam = node;
        GD.Print(node.GetPath());
        GD.Print("GIZMOS.CS: READY");
    }

    public override void _Process(float delta)
    {
        var distance = this.GetChild<Spatial>(0).GlobalTransform.origin.DistanceTo(mainCam.GlobalTransform.origin);

        for(var x = 0; x < this.GetChildCount(); ++x) {
            this.GetChild<Spatial>(x).Scale = new Vector3(distance / 3, distance / 3, distance / 3); 
        }
    }
}