using Godot;

public class MeshAdder : Spatial
{
    Viewport RootView;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MainLoop loop = Engine.GetMainLoop();
        SceneTree mainTree = (SceneTree) loop;
        RootView = mainTree.Root;
    }

    public void AddMesh(ArrayMesh mesh)
    {
        if(RootView == null)
        {
            MainLoop loop = Engine.GetMainLoop();
            SceneTree mainTree = (SceneTree) loop;
            RootView = mainTree.Root;
        }
        
        MeshInstance m = new MeshInstance();
        SpatialMaterial mat = new SpatialMaterial();
        mat.VertexColorUseAsAlbedo = true;
        mat.FlagsUsePointSize = true;
        mat.ParamsPointSize = 4;

        m.MaterialOverride = mat;
        m.Mesh = mesh;
        m.Name = "Custom Mesh";

        GD.Print("Adding mesh to environment...");
        RootView.GetNode("main/env").AddChild(m);
    }
}
