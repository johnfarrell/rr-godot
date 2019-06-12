using Godot;
using System;

public class LIDAR : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    float _minAng;
    float _maxAng;
    float _resolution;
    int _beamNum;
    float _beamMax;
    float dataSampleSize;
    bool interpolation;
    Godot.Collections.Array pointCloud;
    File resultPCD;
    // Called when the node enters the scene tree for the first time.
    
    //Empty constructor to silence godot errors
    private LIDAR()
    {
        
    }
    public LIDAR(float minAng,float maxAng, float resolution,int beamNum,float beamMax)
    {
        this._minAng=minAng;
        this._maxAng = maxAng;
        this._resolution=resolution;
        this._beamNum=beamNum;
        this._beamMax=beamMax;
        this.dataSampleSize=this._beamNum*this._resolution;
        if(resolution<1)
        {
            interpolation=true;
        }
        else{
            interpolation=false;
        }

    }
    
    public override void _Ready()
    {
        pointCloud = new Godot.Collections.Array();
        resultPCD.Open("user://pc.PCD", (int)File.ModeFlags.Write);
    }

    public override void _PhysicsProcess(float delta)
    {
        
    }

    //drives, writes to file, closes file, handles interpolation logic
    public void LIDAR_DRIVER()
    {
        
    }
    //responsible for camera panning
    private void camMovement()
    {

    }
    //responsible for actually taking the raycast and returning the collision data
    private Vector3 imaging()
    {
        return new Vector3(0,0,0);
    }

    //divides the fov of the LIDAR sensor into sectors in order to take the correct number of points as per
    //user specifications, returns number of data samples needed to be produced, ie, how often we need to raycast
    private int gridding()
    {
        return 0;
    }
    //handles interpolation
    private void interp()
    {

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
