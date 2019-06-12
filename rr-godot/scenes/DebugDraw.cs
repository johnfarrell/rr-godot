using Godot;
using System;
using Object = Godot.Object;

public class DebugDraw : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public enum DebugDrawType
    {
        Disable = 0, Unshaded = 1, Overdraw = 2, Wireframe = 3
    }

    public enum PerspectiveType
    {
        Front = 0, Back = 1, Orthogonal = 2, NOrthogonal = 3
    }
    private DebugDrawType currentDrawType = DebugDrawType.Disable;
    private PerspectiveType currentPerspective = PerspectiveType.Front;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }


    private void toolbarChangeRendTypePressed(int id)
    {
        currentDrawType = (DebugDrawType) id;
        switch(currentDrawType)
        {
            case DebugDrawType.Disable:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Disabled;
                GD.Print(currentDrawType);
                
                break;

            case DebugDrawType.Overdraw:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Overdraw;
                GD.Print(currentDrawType);
                
                break;

            case DebugDrawType.Unshaded:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Unshaded;
                GD.Print(currentDrawType);
                
                break;
            case DebugDrawType.Wireframe:
                GetNode<Viewport>("../Viewport").DebugDraw = Viewport.DebugDrawEnum.Wireframe;
                GD.Print(currentDrawType);

                break;
            default:
                GD.Print("Unrecognized Menu Item");
                break;
        }
        
        

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void toolbarChangePerspective(int id)
    {
        currentPerspective = (PerspectiveType) id;
        Node tmp = this.GetNode("../Viewport/Camera/CameraObj");
        GD.Print(tmp.GetName());
        GD.Print(tmp.GetType());
        Godot.Camera tmp2 = (Godot.Camera)tmp;

        Godot.Camera subCam = (Godot.Camera)GetNode("../Viewport/Camera/CameraObj");
        Script gdClass = ResourceLoader.Load("res://src/CameraObj.gd") as Script;
        Object gdCam = new Object();
        gdCam = (Godot.Object)gdClass.Call("new");
        gdCam.SetScript(gdClass);
        
        
        switch(currentPerspective)
        {
            case PerspectiveType.Front:
                
                //GD.Print(subCam.GetClass() + subCam.GetFilename());
                
                Vector3 newLook = new Vector3(5,1,0);
                subCam.LookAtFromPosition(newLook,new Vector3(0,0,0),new Vector3(0,1,0));
                gdCam.Call("_update_mouselook");
                gdCam.Call("_update_movement");
                break;
            case PerspectiveType.Back:
                //subCam = GetNode<Camera>("../Viewport/Camera/CameraObj");
                newLook = new Vector3(-5,1,0);
                subCam.LookAtFromPosition(newLook,new Vector3(0,0,0),new Vector3(0,1,0));
                gdCam.Call("_update_mouselook");
                gdCam.Call("_update_movement");
                break;
            case PerspectiveType.Orthogonal:
                //subCam = GetNode<Camera>("../Viewport/Camera/CameraObj");
                subCam.SetOrthogonal(3,1,10);
                gdCam.Call("_update_mouselook");
                gdCam.Call("_update_movement");
                break;
            case PerspectiveType.NOrthogonal:
                subCam.SetProjection(0);
                gdCam.Call("_update_mouselook");
                gdCam.Call("_update_movement");
                break;
            default:
                GD.Print("Unrecognized Menu Item");
                break;
        }
    }
}
