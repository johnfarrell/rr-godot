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

    //Enum to hold the toolbar item ID for the Drawstyle buttons
    public enum DebugDrawType
    {
        Disable = 0, Unshaded = 1, Overdraw = 2, Wireframe = 3
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

    private PackedScene gizmoScene = (PackedScene)GD.Load("res://scenes/gizmos.tscn");

    private Spatial gizmo;
    private DebugDrawType currentDrawType = DebugDrawType.Disable;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Connect signals from EnvironmentContainer for mouse enter/exit
        GetNode("../../").Connect("mouse_entered", this, "OnEnvContainerMouseEntered");
        GetNode("../../").Connect("mouse_exited", this, "OnEnvContainerMouseExit");

        // Connect tree update signal
        Connect(nameof(envUpdated), GetNode("../../../LeftMenu/TreeContainer/Environment"), "UpdateTree");
        
        gizmo = GetNode<Spatial>("../../Viewport2/gizmos");

        gizmo.Visible = false;
        GD.Print("ENV.CS: READY");
    }

    private void toolbarChangeManipTypePressed(int id)
    {   
        currentManipType = (ManipType) id;
        GD.Print("currentManipType: " + currentManipType);
    }

    private void toolbarAddMeshItemPressed()
    {
        GD.Print("fjeioajfioesa");
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

    //Godot signal handler for the use of the debug draw dropdown menu
    private void toolbarChangeRendTypePressed(int id)
    {
        currentDrawType = (DebugDrawType) id;
        switch(currentDrawType)
        {
            case DebugDrawType.Disable:
                GetNode<Viewport>("..").DebugDraw = Viewport.DebugDrawEnum.Disabled;
                GD.Print(currentDrawType);
                EmitSignal("envUpdated");
                break;

            case DebugDrawType.Overdraw:
                GetNode<Viewport>("..").DebugDraw = Viewport.DebugDrawEnum.Overdraw;
                GD.Print(currentDrawType);
                EmitSignal("envUpdated");
                break;

            case DebugDrawType.Unshaded:
                GetNode<Viewport>("..").DebugDraw = Viewport.DebugDrawEnum.Unshaded;
                GD.Print(currentDrawType);
                EmitSignal("envUpdated");
                break;
            case DebugDrawType.Wireframe:
                GetNode<Viewport>("..").DebugDraw = Viewport.DebugDrawEnum.Wireframe;
                GD.Print(currentDrawType);
                EmitSignal("envUpdated");
                break;
            default:
                GD.Print("Unrecognized Menu Item");
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
        // GD.Print("ENV.CS: " + @event);
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
            gizmo._Input(@event);
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

                // Reset the gizmos
                gizmo.Visible = false;
                gizmo.GlobalTranslate(new Vector3(0, 0, 0) - gizmo.GlobalTransform.origin);
                
            }
            else
            {
                
                // User clicked on an actual object, so updated the dragStart vector
                dragStart = (Vector3) selectedObject["position"];

                // Get the origin of the selected object and update the gizmos
                CollisionObject collider = (CollisionObject) selectedObject["collider"];
                Vector3 selectedObjectOrigin = collider.GlobalTransform.origin;

                gizmo.GlobalTranslate(selectedObjectOrigin - gizmo.GlobalTransform.origin);
            }
            
        }
        if(mouseClicked && selectedObject != null)
        {
            // Handle moving the thingy
            // Get position of new ray cast from camera to mouse
            CollisionObject collider = (CollisionObject) selectedObject["collider"];
            var obj = GetObjUnderMouse();
            if(obj.Count == 0)
            {
                // Process the case where the mouse has left the object
                return;
            }
            else
            {
                if(obj["collider"] != collider)
                {
                    selectedObject = obj;
                    collider = (CollisionObject) obj["collider"];
                }
            }
            Vector3 newPos = (Vector3) obj["position"];
            Vector3 dragDelta = newPos - dragStart;

            if(!gizmo.Visible)
            {
                gizmo.Visible = true;
            }
            gizmo.GlobalTranslate(collider.GlobalTransform.origin - gizmo.GlobalTransform.origin);
        
            Type parType = collider.GetParent().GetType();
            if(collider is Godot.RigidBody)
            {
                RigidBody body = (RigidBody) collider;
                switch (currentManipType)
                {
                    case ManipType.Translate:
                        body.Bounce = (float) 0.5;
                        body.ApplyImpulse(new Vector3(0,(float)0.5,0), new Vector3(0,1,0));
                        break;
                    default:
                        break;
                }
            }

            if(parType.ToString() == "Godot.MeshInstance")
            {
                MeshInstance parMesh = collider.GetParent<MeshInstance>();
                switch (currentManipType)
                {
                    case ManipType.Translate:
                        parMesh.GlobalTranslate(dragDelta);
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
                // GD.Print("GODDANGIT: " + parType);
            }
        }
        if(!mouseClicked && selectedObject != null)
        {
            CollisionObject collider = (CollisionObject) selectedObject["collider"];
            gizmo.GlobalTranslate(collider.GlobalTransform.origin - gizmo.GlobalTransform.origin);
        }
    }
}
