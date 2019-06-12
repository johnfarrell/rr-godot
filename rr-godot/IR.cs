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
    
        saveData.Open("user://savegame.save", (int)File.ModeFlags.Write);
        var dir = -GlobalTransform.basis.z;
        Vector3 ro = this.ProjectRayOrigin(new Vector2(dir.x,dir.y));
        Vector3 rn = ro + this.ProjectRayNormal(new Vector2(dir.x,dir.y))*maxDist;
        
        var spaceState = GetWorld().DirectSpaceState;
        
        
        Array[] execptions = {};
        execptions.SetValue(this,0);

        //this ray needs to be pointed in the direction of the camera
        var result = spaceState.IntersectRay(dir, rn,new Godot.Collections.Array{this} );

        
        saveData.StoreLine(JSON.Print(result["position"]));
        

        saveData.Close();
        this.SetCurrent(false);
    }
    public override void _ExitTree()
    {
        this.SetCurrent(false);
        
    }

    
}
