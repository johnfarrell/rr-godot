using System;
using Godot;

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
    private Vector3 update = new Vector3(2, 0, 0);

    private bool mouseInside = false;
    private bool mouseClicked = false;

    // Stores the start position of a click-and-drag motion on an object
    private Vector3 dragStart = new Vector3();

    // Currently selected object in the world
    public Godot.Collections.Dictionary selectedObject = null;

    private bool gizmoActive = false;

    private PackedScene gizmoScene = (PackedScene)GD.Load("res://Godot/scenes/gizmos.tscn");

    private Spatial gizmo;

    private DebugDrawType currentDrawType = DebugDrawType.Disable;

    private Godot.Spatial marker;

    // True if the SceneTree is paused,
    // False if the SceneTree is running
    private bool simState;

    [Signal]
    public delegate void UpdateVelocities(
        float linx, float liny, float linz,
        float rotx, float roty, float rotz
    );

    [Signal]
    public delegate void UpdateTransform(
        float linx, float liny, float linz,
        float rotx, float roty, float rotz
    );


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect(
            "UpdateVelocities",
            GetNode("/root/main/UI/AppWindow/LeftMenu/ObjectInspector/ObjectInspector/VBoxContainer/VelocityMenu"),
            "UpdateAllVels"
        );
        this.Connect(
            "UpdateTransform",
            GetNode("/root/main/UI/AppWindow/LeftMenu/ObjectInspector/ObjectInspector/VBoxContainer/TransformMenu"),
            "UpdateAllTrans"
        );

        GetNode("/root/main/UI/AppWindow/LeftMenu/TreeContainer/Environment").Connect("ObjectSelected", this, "TreeItemSelected");

        // Connect translation signals
        VBoxContainer TransformInspector = (VBoxContainer)GetNode("/root/main/UI/AppWindow/LeftMenu/ObjectInspector/ObjectInspector/VBoxContainer/TransformMenu");
        TransformInspector.Connect("XTrans", this, "TranslateX");
        TransformInspector.Connect("YTrans", this, "TranslateY");
        TransformInspector.Connect("ZTrans", this, "TranslateZ");
        TransformInspector.Connect("XRot", this, "RotX");
        TransformInspector.Connect("YRot", this, "RotY");
        TransformInspector.Connect("ZRot", this, "RotZ");

        Node temp = GetNode("SelectedObject");
        GD.Print(temp.GetType());
        marker = (Godot.Spatial)temp;

        // Connect tree update signal
        Connect(nameof(envUpdated), GetNode("/root/main/UI/AppWindow/LeftMenu/TreeContainer/Environment/"), "UpdateTree");

        gizmo = GetNode<Spatial>("/root/main/UI/AppWindow/EnvironmentContainer/4WayViewport/VerticalSplit/HSplit1/Viewport1/Viewport/gizmos");

        for (var i = 0; i < gizmo.GetChildCount(); ++i)
        {
            gizmo.GetChild(i).Connect("HandlePressed", this, "GizmoClicked");
            gizmo.GetChild(i).Connect("HandleUnpressed", this, "GizmoUnclicked");
        }

        simState = false;
        GetTree().Paused = simState;

        // gizmo.Visible = false;
        GD.Print("ENV.CS: READY");
    }

    public void TreeItemSelected(string itemName)
    {
        GD.Print("ENV: " + itemName);
        try
        {
            Node item = this.FindNode(itemName, true, false);

            GD.Print("Item: " + item.Name + " Type: " + item.GetClass().ToString());
        }
        catch
        {
            GD.Print("it broke");
        }
        
    }

    public Godot.Collections.Dictionary GetSelectedObject()
    {
        return selectedObject;
    }

    public void GizmoClicked()
    {
        gizmoActive = true;
    }

    public void GizmoUnclicked()
    {
        gizmoActive = false;
    }

    public void GizmoActiveChange()
    {
        gizmoActive = !gizmoActive;
        GD.Print("ENV.CS: GIZMO ACTIVE: " + gizmoActive);
    }

    public void ToggleSimState()
    {
        simState = !simState;

        GetTree().Paused = !GetTree().Paused;

        GD.Print("SceneTree Paused Status: " + simState);
    }

    /// <summary>
    /// Godot signal handler for the AddMesh toolbar menu being used
    /// </summary>
    /// <param name="id">Index number of the button pressed on the menu</param>
    private void toolbarAddMeshItemPressed(int id)
    {
        switch (id)
        {
            case (int)MeshID.Cube:
                addCubeMesh();
                EmitSignal("envUpdated");
                break;
            case (int)MeshID.Sphere:
                addSphereMesh();
                EmitSignal("envUpdated");
                break;
            case (int)MeshID.Cylinder:
                addCylinderMesh();
                EmitSignal("envUpdated");
                break;
            case (int)MeshID.Prism:
                addPrismMesh();
                EmitSignal("envUpdated");
                break;
            case (int)MeshID.Capsule:
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
        StaticBody temp = new StaticBody();
        MeshInstance tempMesh = new MeshInstance();
        tempMesh.Mesh = new CubeMesh();

        tempMesh.CreateTrimeshCollision();

        // Get the collision shape and reparent it to the StaticBody
        CollisionShape collision = (CollisionShape)tempMesh.GetChild(0).GetChild(0);

        tempMesh.GetChild(0).RemoveChild(collision);
        tempMesh.RemoveChild(tempMesh.GetChild(0));

        temp.AddChild(collision);
        temp.AddChild(tempMesh);

        this.AddChild(temp, true);
    }

    /// <summary>
    /// Adds a SphereMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addSphereMesh()
    {
        StaticBody temp = new StaticBody();
        MeshInstance tempMesh = new MeshInstance();
        tempMesh.Mesh = new SphereMesh();

        tempMesh.CreateTrimeshCollision();

        // Get the collision shape and reparent it to the StaticBody
        CollisionShape collision = (CollisionShape)tempMesh.GetChild(0).GetChild(0);

        tempMesh.GetChild(0).RemoveChild(collision);
        tempMesh.RemoveChild(tempMesh.GetChild(0));

        temp.AddChild(collision);
        temp.AddChild(tempMesh);

        this.AddChild(temp, true);
    }

    /// <summary>
    /// Adds a CylinderMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addCylinderMesh()
    {
        StaticBody temp = new StaticBody();
        MeshInstance tempMesh = new MeshInstance();
        tempMesh.Mesh = new CylinderMesh();

        tempMesh.CreateTrimeshCollision();

        // Get the collision shape and reparent it to the StaticBody
        CollisionShape collision = (CollisionShape)tempMesh.GetChild(0).GetChild(0);

        tempMesh.GetChild(0).RemoveChild(collision);
        tempMesh.RemoveChild(tempMesh.GetChild(0));

        temp.AddChild(collision);
        temp.AddChild(tempMesh);

        this.AddChild(temp, true);
    }

    /// <summary>
    /// Adds a PrismMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addPrismMesh()
    {
        StaticBody temp = new StaticBody();
        MeshInstance tempMesh = new MeshInstance();
        tempMesh.Mesh = new PrismMesh();

        tempMesh.CreateTrimeshCollision();

        // Get the collision shape and reparent it to the StaticBody
        CollisionShape collision = (CollisionShape)tempMesh.GetChild(0).GetChild(0);

        tempMesh.GetChild(0).RemoveChild(collision);
        tempMesh.RemoveChild(tempMesh.GetChild(0));

        temp.AddChild(collision);
        temp.AddChild(tempMesh);

        this.AddChild(temp, true);
    }

    /// <summary>
    /// Adds a CapsuleMesh node to the world as a child
    /// of the root node.
    /// </summary>
    private void addCapsuleMesh()
    {
        StaticBody temp = new StaticBody();
        MeshInstance tempMesh = new MeshInstance();
        tempMesh.Mesh = new CapsuleMesh();

        tempMesh.CreateTrimeshCollision();

        // Get the collision shape and reparent it to the StaticBody
        CollisionShape collision = (CollisionShape)tempMesh.GetChild(0).GetChild(0);

        tempMesh.GetChild(0).RemoveChild(collision);
        tempMesh.RemoveChild(tempMesh.GetChild(0));

        temp.AddChild(collision);
        temp.AddChild(tempMesh);

        this.AddChild(temp, true);
    }

    /// <summary>
    /// Handles input events in the viewport.
    /// Called by Godot, do not call manually
    /// </summary>
    /// <param name="@event">InputEvent obj containing Godot event information</param>
    public override void _Input(InputEvent @event)
    {
        gizmo._Input(@event);
        if (@event is InputEventMouseButton && @event.IsAction("mouse_left_click"))
        {
            mouseClicked = !mouseClicked;
        }
    }

    /// <summary>
    /// Signal reciever for MouseEnter signal sent by EnvironmentContainer node
    /// </summary>
    private void OnEnvContainerMouseEntered()
    {
        GD.Print("MouseEnter");
        mouseInside = true;
    }

    /// <summary>
    /// Signal reciever for MouseExit signal sent by EnvironmentContainer node
    /// </summary>
    private void OnEnvContainerMouseExit()
    {
        GD.Print("MouseExit");
        mouseInside = false;
    }

    /// <summary>
    /// Uses raycasting to determine what collision object is under the mouse pointer
    /// </summary>
    private Godot.Collections.Dictionary GetObjUnderMouse()
    {
        Viewport viewport = GetNode<Viewport>("/root/main/UI/AppWindow/EnvironmentContainer/4WayViewport/VerticalSplit/HSplit1/Viewport1/Viewport");
        // Viewport viewport = GetViewport();
        Vector2 mousePos = viewport.GetMousePosition();
        Vector3 rayFrom = viewport.GetCamera().ProjectRayOrigin(mousePos);
        Vector3 rayTo = rayFrom + viewport.GetCamera().ProjectRayNormal(mousePos) * 1000;
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        var selection = spaceState.IntersectRay(rayFrom, rayTo);

        return selection;
    }

    // TODO: Move this out of the env file so there isn't a
    // god class
    private void ResetGizmoPosition()
    {
        UpdateGizmoPosition(new Vector3(0, 0, 0), false);
    }

    // TODO: Move this out of the env file also
    private void UpdateGizmoPosition(Vector3 TargetPos, bool SetVisible = true)
    {
        for (var x = 0; x < gizmo.GetChildCount(); ++x)
        {
            Spatial temp = gizmo.GetChild<Spatial>(x);

            // temp.Visible = SetVisible;
            temp.GlobalTranslate(TargetPos - temp.GlobalTransform.origin);
        }
    }

    private void ResetMarkerParent()
    {
        UpdateMarkerParent(this);
    }

    private void UpdateMarkerParent(Node newParent)
    {
        Node old_parent = marker.GetParent();

        old_parent.RemoveChild(marker);
        newParent.AddChild(marker);
    }

    private void TranslateX(float xVal)
    {
        Spatial selObj = (Spatial)GetSelectedObject()["collider"];
        if (selObj == null)
        {
            return;
        }
        Transform newTrans = selObj.GetTransform();

        newTrans.origin.x = xVal;

        selObj.SetTransform(newTrans);

    }
    private void TranslateY(float yVal)
    {
        Spatial selObj = (Spatial)GetSelectedObject()["collider"];
        if (selObj == null)
        {
            return;
        }
        Transform newTrans = selObj.GetTransform();

        newTrans.origin.y = yVal;

        selObj.SetTransform(newTrans);
    }
    private void TranslateZ(float zVal)
    {
        Spatial selObj = (Spatial)GetSelectedObject()["collider"];
        if (selObj == null)
        {
            return;
        }
        Transform newTrans = selObj.GetTransform();

        newTrans.origin.z = zVal;

        selObj.SetTransform(newTrans);
    }
    private void RotX(float xVal)
    {

    }
    private void RotY(float yVal)
    {

    }
    private void RotZ(float zVal)
    {

    }

    private void UpdateObjectTransform(Spatial obj)
    {
        try
        {
            Vector3 eul = obj.Transform.basis.GetEuler();
            EmitSignal("UpdateTransform",
                obj.Transform.origin.x,
                obj.Transform.origin.y,
                obj.Transform.origin.z,
                eul[0],
                eul[1],
                eul[2]
            );
        }
        catch
        {
            GD.Print("idk");
        }
    }

    private void UpdateObjectVelocities(CollisionObject obj)
    {
        try
        {
            RigidBody body = (RigidBody)obj;
            EmitSignal("UpdateVelocities",
                body.LinearVelocity.x,
                body.LinearVelocity.y,
                body.LinearVelocity.z,
                body.AngularVelocity.x,
                body.AngularVelocity.y,
                body.AngularVelocity.z);
        }
        catch
        {
            GD.Print("couldn't find rigid");
        }
    }

    /// <summary>
    ///  Called every physics frame, more reliable than using screen frames
    /// Called by Godot, do not call manually
    /// </summary>
    public override void _PhysicsProcess(float _delta)
    {
        if (selectedObject != null)
        {
            CollisionObject collider = (CollisionObject)selectedObject["collider"];
            UpdateGizmoPosition(collider.GlobalTransform.origin);
            UpdateObjectVelocities(collider);
        }
        if (gizmoActive && selectedObject != null)
        {
            CollisionObject collider = (CollisionObject)selectedObject["collider"];

            UpdateGizmoPosition(collider.GlobalTransform.origin);
            UpdateObjectVelocities(collider);
            return;
        }
        if (mouseClicked)
        {
            Godot.Collections.Dictionary tempObj = new Godot.Collections.Dictionary();
            // Get the clicked object (if any)
            try
            {
                tempObj = GetObjUnderMouse();
            }
            catch (Exception e)
            {
                GD.Print(e);
            }

            if (tempObj.Count == 0)
            {
                // User clicked on empty space
                ResetGizmoPosition();
                ResetMarkerParent();
                selectedObject = null;
            }
            else if (selectedObject != null &&
                tempObj["collider"] == selectedObject["collider"])
            {
                // User clicked on the same object
            }
            else
            {
                // User clicked on a different object
                selectedObject = tempObj;

                CollisionObject collider = (CollisionObject)selectedObject["collider"];

                UpdateGizmoPosition(collider.GlobalTransform.origin);
                UpdateMarkerParent(collider);
            }
        }
    }
}