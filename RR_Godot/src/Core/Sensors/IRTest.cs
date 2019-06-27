using Godot;
using System;

namespace RR_Godot.Core.Sensors
{
    public class IRTest : Spatial
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            var scene = GD.Load<PackedScene>("res://theme/LIDAR.tscn");
            var node =  scene.Instance();
            Vector3 loc = this.Translation;
            loc.x = loc.x+1;
            loc.y = loc.y+1;
            node = new LIDAR(-0.53529248,0.18622663,1,64,100,loc);
            GetChild(GetChildCount()-1).AddChild(node);
            
        }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    }
}