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
        mat.ParamsPointSize = 8;

        m.MaterialOverride = mat;
        m.Mesh = mesh;
        m.Name = "Custom Mesh";

        m.CreateConvexCollision();

        StaticBody temp = new StaticBody();
        temp.Name = "Mesh Static Body";

        // Get the collision shape and reparent it to the StaticBody
        CollisionShape collision = (CollisionShape) m.GetChild(0).GetChild(0);
        collision.Name = "Custom Mesh Collision";

        m.GetChild(0).RemoveChild(collision);
        m.RemoveChild(m.GetChild(0));

        temp.AddChild(collision);
        temp.AddChild(m);

        GD.Print("Adding mesh to environment...");
        RootView.GetNode("main/env").AddChild(temp);
    }
}
