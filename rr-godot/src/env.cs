using Godot;
using System;

public class env : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Basic enum to hold the toolbar item ID of the AddMesh buttons
    enum MeshID
    {
        Cube = 0,
        Sphere = 1,
        Cylinder = 2,
        Prism = 3,
        Capsule = 4
    }

    [Signal]
    public delegate void envUpdated();


    Vector3 lastPos = new Vector3();
    Vector3 update = new Vector3(2,0,0);

    bool mouseInside = false;
    bool mouseClicked = false;

    Vector3 dragStart = new Vector3();

    Godot.Collections.Dictionary selectedObject = null;

    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("envUpdated", GetNode("../../../LeftMenu/TreeContainer/Environment"), "UpdateTree");
        
    }

    private void toolbarAddMeshItemPressed(int id)
    {
        switch(id) {
            case (int) MeshID.Cube:
                GD.Print("Cube");
                addCubeMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Sphere:
                GD.Print("Sphere");
                addSphereMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Cylinder:
                GD.Print("Cylinder");
                addCylinderMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Prism:
                GD.Print("Prism");
                addPrismMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Capsule:
                GD.Print("Capsule");
                addCapsuleMesh();
                EmitSignal("envUpdated");
                break;
            default:
                GD.Print("Unrecognized button id");
                break;
        }
    }

    private void addCubeMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new CubeMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Cube";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp);
    }

    private void addSphereMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new SphereMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Sphere";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp);
    }

    private void addCylinderMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new CylinderMesh();
        temp.CreateTrimeshCollision();
        
        temp.Name = "Cylinder";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp);
    }

    private void addPrismMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new PrismMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Prism";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp);
    }

    private void addCapsuleMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new CapsuleMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Capsule";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp);
    }

    public override void _Input(InputEvent @event)
    {
        if(mouseInside)
        {
            if(@event is InputEventMouseButton && @event.IsAction("mouse_left_click"))
            {
                mouseClicked = !mouseClicked;
                if(selectedObject != null)
                {
                    selectedObject = null;
                }
            }
        }
    }

    private void OnEnvContainerMouseEntered()
    {
        mouseInside = true;
    }

    private void OnEnvContainerMouseExit()
    {
        mouseInside = false;
    }

    private Godot.Collections.Dictionary GetObjUnderMouse()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayFrom = GetViewport().GetCamera().ProjectRayOrigin(mousePos);
        Vector3 rayTo = rayFrom + GetViewport().GetCamera().ProjectRayNormal(mousePos) * 1000;
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        var selection = spaceState.IntersectRay(rayFrom, rayTo);

        return selection;
    }

    public override void _PhysicsProcess(float _delta)
    {
        if(mouseClicked && selectedObject == null)
        {
            // Select the proper thingy
            selectedObject = GetObjUnderMouse();
            if(selectedObject.Count == 0) {
                selectedObject = null;
            }
            else{
                dragStart = (Vector3) selectedObject["position"];
                // GD.Print("Selected object: " + selectedObject);
            }
            
        }
        if(mouseClicked && selectedObject != null)
        {
            // Handle moving the thingy
            // Get position of new ray cast from camera to mouse
            var obj = GetObjUnderMouse();
            if(obj.Count == 0)
            {
                // Skip over processing the case where the mouse has left the object
                return;
            }
            Vector3 newPos = (Vector3) obj["position"];
            Vector3 dragDelta = newPos - dragStart;

            CollisionObject collider = (CollisionObject) selectedObject["collider"];

            var parMesh = (MeshInstance) collider.GetParent();
            parMesh.Translate(dragDelta);
            
            dragStart = newPos;
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
