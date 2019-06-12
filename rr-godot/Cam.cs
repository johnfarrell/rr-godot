using Godot;
using System;

public class Cam : Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    String connect;
    float maxView;
    float minView;
    float fov;
    String docLoc;
    int sequence;
    
    public Cam(String connection,String loc,int seq)
    {
        this.connect = connection;
        this.docLoc = loc;
        this.sequence = seq;
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //hookup to RR will be needed here but does not exist at the present
        //for now we will substitute for a connection to a hardcoded local host, and in the interest of storage, we will store fewer images than optimal

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if((int)delta%10==0)
        {
            this.SetCurrent(true);
            var capture = this.GetViewport().GetTexture().GetData();
            capture.SavePng("c://Users/John Parent/AppData/Local/Temp/Godot");
            this.SetCurrent(false);
        }


    }
}
