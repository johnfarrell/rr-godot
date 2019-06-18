using Godot;
using System;

public class LIDAR : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    double _minAng;
    double _maxAng;
    float _resolution;
    int _beamNum;
    float _beamMax;
    float dataSampleSize;
    bool interpolation;
    int refresh;
    float maxLR;
    Godot.Collections.Array pointCloud;
    Godot.Collections.Array distCloud;
    File resultPCD;
    // Called when the node enters the scene tree for the first time.
    
    //Empty constructor to silence godot errors
    private LIDAR()
    {
        
    }
    /// <summary>
    /// Creates a LIDAR object to be instantiated on a model by a model parser
    /// </summary>
    /// <param name = "minAng", name = "maxAng">minAng and maxAng define the yz limits of the LIDAR's scanning<param>
    /// <param name = "pos">pos: Vector3 coordinate of the LIDAR sensor</param>
    public LIDAR(double minAng,double maxAng, float resolution,int beamNum,float beamMax,Vector3 pos)
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
        
        this.SetTranslation(pos);
    }
    
    public override void _Ready()
    {
        pointCloud = new Godot.Collections.Array();
        
    }

    /// <summary>
    /// Is called by the main process 60x per second, calls the LIDAR detector to collect data, and then resets the LIDAR's oritentation
    /// </summary>
    public override void _PhysicsProcess(float delta)
    {
        LIDAR_DRIVER();
        Godot.Collections.Array cams = GetTree().GetNodesInGroup("liCams");
        foreach(Camera Cam in cams)
        {
            Cam.Orthonormalize();
        }
    }
    /// <summary>
    /// Drives the LIDAR process, writes to file, closes file, handles interpolation logic and  LIDAR camera movement control
    /// </summary>
    public void LIDAR_DRIVER()
    {
        Godot.Collections.Array gridSpace = gridding();
        GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Left,maxLR});
        GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Up,_maxAng});
        string largerSide = (string)gridSpace[3];
        int topside=0;
        int sideSide=0;
        switch(largerSide)
        {
            case "lr":
                sideSide = (int)gridSpace[1];
                topside = (int)gridSpace[2];
                break;
            case "ud":
                sideSide = (int)gridSpace[2];
                topside = (int)gridSpace[1];
                break;
            default:
                break;
        }

        for(int i=0;i<topside;i++)
        {
            if(i>0)
            {
                camResetTop();
            }
            for(int j=0;j<sideSide;j++)
            {
                var result = imaging();
                if(!result.Equals(new Vector3(0,0,0)))
                {
                    var collisionLoc = result;
                    float collisionDist = distancing(result);
                    pointCloud.Add(collisionLoc);
                    distCloud.Add(collisionDist);

                }
                camMovement((float)gridSpace[0],"down");

            }
            camMovement((float)gridSpace[0],"left");
        }

        if(_resolution<1)
        {
            interp();
        }

        writeFile();

    }
    private void camResetTop()
    {
        GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Up,_maxAng});
    }
    //responsible for camera panning
    private void camMovement(float dist, string dir)
    {
        switch(dir)
        {
            case "up":
                GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Up,dist});
                break;
            case "down":
                GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Down,dist});
                break;
            case "left":
                GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Left,dist});
                break;
            case "right":
                GetTree().CallGroup("liCams","RotateObjectLocal",new Godot.Collections.Array{Vector3.Right,dist});
                break;
            default:
                break;
        }

    }
    //responsible for actually taking the raycast and returning the collision data
    private Vector3 imaging()
    {
        
        
        var dir = -GlobalTransform.basis.z;
        
        var ray = (RayCast)GetNode("RayCast");
        ray.SetCastTo(dir*100);
        ray.SetEnabled(true);
        
        if(ray.IsEnabled() && ray.IsColliding())
        {
            return ray.GetCollisionPoint();

        }
        
        
        return new Vector3(0,0,0);
    }

    private float distancing(Vector3 collisionLocation)
    {   
        return this.GetTranslation().DistanceTo(collisionLocation);
    }

    //divides the fov of the LIDAR sensor into sectors in order to take the correct number of points as per
    //user specifications, returns number of data samples needed to be produced, ie, how often we need to raycast
    //returns camera jump distance in Return[0],number samples needed on larger side in Return[1], and number of samples on smaller side in Return[2]
    private Godot.Collections.Array gridding()
    {
        double totalRads = maxLR + _maxAng + _minAng;
        double spacing = totalRads/dataSampleSize;
        string larger;
        double largerDim = Math.Max(maxLR,(_maxAng+_minAng));
        if(largerDim.Equals(maxLR))
        {
            larger = "lr";
        }
        else{
            larger = "ud";
        }
        double largerRatio = largerDim/spacing;
        int numOnLarger = (int)(largerDim/(largerRatio+1));
        int numOnSmaller = (int)dataSampleSize - numOnLarger;
                
        return new Godot.Collections.Array{largerRatio,numOnLarger,numOnSmaller,larger};
    }
    //handles interpolation
    private void interp()
    {
        int numOfInterp = _beamNum-(int)dataSampleSize;
        int dataRatio = pointCloud.Count/numOfInterp;
        int interps=0;
        int itr = 0;
        while(interps<numOfInterp)
        {
            Vector3 node1 = (Vector3)pointCloud[itr];
            Vector3 node2 = (Vector3)pointCloud[itr+1];
            if(node1.DistanceTo(node2) < 1)
            {
                float dist1 = (float)distCloud[itr];
                float dist2 = (float)distCloud[itr+1];
                if(Math.Abs(dist1-dist2)<2)
                {   
                    float x = (node1.x+node2.x)/2;
                    float y = (node1.y + node2.y)/2;
                    float z = (node1.z + node2.z)/2;
                    pointCloud.Add(new Vector3(x,y,z));
                    distCloud.Add(distancing(new Vector3(x,y,z)));
                    interps++;
                    itr += dataRatio;
                }
                else{
                    itr++;
                }
            }
            else{
                itr++;
            }

            if(itr >= pointCloud.Count-1+dataRatio)
            {
                itr = 0;
            }
        }

    }

    //handles actually writing to the file with the data from the class variable array set aside for temporarily storing this data
    private void writeFile()
    {
        resultPCD = new File();
        resultPCD.Open("c://Users/John Parent/Dropbox/a/pc.json", (int)File.ModeFlags.Write);
        resultPCD.StoreLine("VERSION .7");
        resultPCD.StoreLine("FIELDS x y z");
        resultPCD.StoreLine("SIZE 4 4 4");
        resultPCD.StoreLine("TYPE F F F");
        resultPCD.StoreLine("COUNT 1 1 1");
        resultPCD.StoreLine("WIDTH "+_beamNum);
        resultPCD.StoreLine("Height 1");
        resultPCD.StoreLine("VIEWPOINT 0 0 0 1 0 0 0");
        resultPCD.StoreLine("POINTS "+_beamNum);
        resultPCD.StoreLine("DATA ascii");
        foreach(Vector3 xyz in pointCloud)
        {
            resultPCD.StoreLine(xyz.x + " " +xyz.y + " " +xyz.z );
        }
    }
    /// <summary>
    /// when LIDAR exits and data is no longer needed the file written to is closed
    /// </summary>
    public override void _ExitTree()
    {
        resultPCD.Close();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
