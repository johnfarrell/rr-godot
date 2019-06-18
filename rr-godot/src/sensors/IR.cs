using Godot;
using System;

public class IR : Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    float fov = 0.10F;
    Array distances;

    

    float maxDist;
    float minDist;
    string connection;
    int seq;
    string type = "IR";
    File saveData;
    string local = "c://Users/John Parent/Documents";
    
    public IR()
    {
        this.saveData = new File();
    }
    public IR(string connection)
    {
        this.connection = connection;
        this.saveData = new File();
        
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //an rr connection will be established here given by the user in the contructor when they create an instance of this IR sensor
        //for now, and json will be written to a local repository so we can verify information.
        this.saveData = new File();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
    //     this.SetCurrent(true);
    //     saveData.Open("user://savegame.save", (int)File.ModeFlags.Write);





    //     saveData.Close();
    //     this.SetCurrent(false);
    // }

    public override void _PhysicsProcess(float delta)
    {
        GD.Print(delta);
        saveData.Open("c://Users/John Parent/Dropbox/a/saveData.json", (int)File.ModeFlags.Write);
        var dir = -GlobalTransform.basis.z;
        
        var ray = (RayCast)GetNode("RayCast");
        ray.CastTo = dir*100;
        ray.Enabled = true;
        
        if(ray.Enabled && ray.IsColliding())
        {
            saveData.StoreLine(JSON.Print(ray.GetCollisionPoint()));

        }
       
        //code for projecting the simulation of the IR sensor
        // ImmediateGeometry g = new ImmediateGeometry();
        // this.AddChild(g);
        // ImmediateGeometry t = (ImmediateGeometry)this.GetChild(0);
        // t.Begin(Mesh.PrimitiveType.Lines,null);
        // t.AddVertex(dir);
        // t.AddVertex(rn);
        // t.End();
        saveData.Close();
        ray.Enabled = false;
    }
    public override void _ExitTree()
    {
        var ray = (RayCast)GetNode("RayCast");
        ray.Enabled = false;
        
    }

    
}
