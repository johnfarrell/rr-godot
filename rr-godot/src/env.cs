using Godot;
using System;

public class env : Spatial
{
    // Basic enum to hold the toolbar item ID of the AddMesh buttons
    public enum MeshID
    {
        Cube = 0,
        Sphere = 1,
        Cylinder = 2,
        Prism = 3,
        Capsule = 4
    }

    // Enum to hold the toolbar item ID of the Manipulation type buttons
    public enum ManipType
    {
        Translate = 0,
        Rotate = 1,
        Scale = 2
    }

    [Signal]
    public delegate void envUpdated();

    // Stores the origin of the last MeshInstance placed
    private Vector3 lastPos = new Vector3();
    // Update delta for placing new MeshInstances
    private Vector3 update = new Vector3(2,0,0);

    private bool mouseInside = false;
    private bool mouseClicked = false;

    // Stores the start position of a click-and-drag motion on an object
    private Vector3 dragStart = new Vector3();

    // Currently selected object in the world
    private Godot.Collections.Dictionary selectedObject = null;

    private ManipType currentManipType = ManipType.Translate;

    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("envUpdated", GetNode("../../../LeftMenu/TreeContainer/Environment"), "UpdateTree");
        Spatial gizmoScene = GetNode<Spatial>("gizmos");

        gizmoScene.SetAsToplevel(true);

    }

    private void toolbarChangeManipTypePressed(int id)
    {   
        currentManipType = (ManipType) id;
        // switch(id) {
        //     case (int) ManipType.Translate:
        //         currentManipType = ManipType.Translate;
        //         break;
        //     case (int) ManipType.Rotate:
        //         currentManipType = 
        //         break;
        //     case (int) ManipType.Scale:
        //         GD.Print("Scale");
        //         break;
        //     default:
        //         GD.Print("Unrecognized button id");
        //         break;
        // }
    }

    /// <summary>
    /// Godot signal handler for the AddMesh toolbar menu being used
    /// </summary>
    /// <param name="id">Index number of the button pressed on the menu</param>
    private void toolbarAddMeshItemPressed(int id)
    {
        switch(id) {
            case (int) MeshID.Cube:
                addCubeMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Sphere:
                addSphereMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Cylinder:
                addCylinderMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Prism:
                addPrismMesh();
                EmitSignal("envUpdated");
                break;
            case (int) MeshID.Capsule:
                addCapsuleMesh();
                EmitSignal("envUpdated");
                break;
            default:
                GD.Print("Unrecognized button id");
                break;
        }
    }

    /// <summary>
    /// Adds a CubeMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addCubeMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new CubeMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Cube";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp, true);
    }

    /// <summary>
    /// Adds a SphereMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addSphereMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new SphereMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Sphere";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp, true);
    }

    /// <summary>
    /// Adds a CylinderMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addCylinderMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new CylinderMesh();
        temp.CreateTrimeshCollision();
        
        temp.Name = "Cylinder";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp, true);
    }

    /// <summary>
    /// Adds a PrismMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addPrismMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new PrismMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Prism";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp, true);
    }

    /// <summary>
    /// Adds a CapsuleMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addCapsuleMesh()
    {
        var temp = new MeshInstance();
        temp.Mesh = new CapsuleMesh();
        temp.CreateTrimeshCollision();

        temp.Name = "Capsule";
        temp.Translate(lastPos + update);
        lastPos = temp.Translation;

        AddChild(temp, true);
    }

    /// <summary>
    /// Handles input events in the viewport.
    /// Called by Godot, do not call manually
    /// </summary>
    /// <param name="@event">InputEvent obj containing Godot event information</param>
    public override void _Input(InputEvent @event)
    {
        GD.Print("ENV.CS: " + @event);
        // GetNode("/root")._Input(@event);
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

    /// <summary>
    /// Signal reciever for MouseEnter signal sent by EnvironmentContainer node
    /// </summary>
    private void OnEnvContainerMouseEntered()
    {
        mouseInside = true;
    }

    /// <summary>
    /// Signal reciever for MouseExit signal sent by EnvironmentContainer node
    /// </summary>
    private void OnEnvContainerMouseExit()
    {
        mouseInside = false;
    }

    /// <summary>
    /// Uses raycasting to determine what collision object is under the mouse pointer
    /// </summary>
    private Godot.Collections.Dictionary GetObjUnderMouse()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayFrom = GetViewport().GetCamera().ProjectRayOrigin(mousePos);
        Vector3 rayTo = rayFrom + GetViewport().GetCamera().ProjectRayNormal(mousePos) * 1000;
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        var selection = spaceState.IntersectRay(rayFrom, rayTo);

        return selection;
    }

    /// <summary>
    ///  Called every physics frame, more reliable than using screen frames
    /// Called by Godot, do not call manually
    /// </summary>
    public override void _PhysicsProcess(float _delta)
    {
        if(mouseClicked && selectedObject == null)
        {
            // Select the proper thingy
            selectedObject = GetObjUnderMouse();
            if(selectedObject.Count == 0)
            {
                // Raycast returned an empty dictionary, user clicked in empty space
                selectedObject = null;
            }
            else
            {
                // User clicked on an actual object, so updated the dragStart vector
                dragStart = (Vector3) selectedObject["position"];
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

        
            Type parType = collider.GetParent().GetType();

            GD.Print(parType.ToString() == "Godot.MeshInstance");

            if(parType.ToString() == "Godot.MeshInstance")
            {
                MeshInstance parMesh = collider.GetParent<MeshInstance>();
                switch (currentManipType)
                {
                    case ManipType.Translate:
                        parMesh.Translate(dragDelta);
                        break;
                    case ManipType.Rotate:
                        break;
                    case ManipType.Scale:
                        Vector3 normal = (Vector3) obj["normal"];
                        parMesh.Scale += (normal / 50);
                        break;
                    default:
                        break;
                }
            
                dragStart = newPos;
            }
            else
            {
                GD.Print(parType);
            }

            
        }
    }
}
