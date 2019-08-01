using Godot;
using System;

public class Camera : Spatial
{
    float distance = 2.5f;
    bool collisions = true;
    int yaw_limit = 360;
    int pitch_limit = 360;

    float _yaw = 0.0f;
    float _pitch = 0.0f;
    float _total_yaw = 0.0f;
    float _total_pitch = 0.0f;
    Vector2 mousePosition = new Vector2(0.0f,0.0f);
    
    //Exported editor accessible variables
    [Export]
    private Godot.Camera cam;

    [Export(PropertyHint.Range,"0.0 , 1.0")]
    float smoothness = 0.5f;

    [Export(PropertyHint.Range,"0.0, 1.0")]
    public float sensitivity = 0.5f;

    [Export(PropertyHint.Range,"0.0, 10.0")]
    float acceleration = 1f;

    [Export(PropertyHint.Range,"0.0, 10.0")]
    float deceleration = 0.1f;

    [Export]
    private bool enabled = true;

    [Export]
    private bool press = false;

    [Export]
    private bool local = true;

    [Export]
    private String forward;
    [Export]
    private String back;
    [Export]
    private String left;
    [Export]
    private String right;
    [Export]
    private String up;
    [Export]
    private String down;

    [Export]
    Vector3 direction = new Vector3(0.0f,0.0f,0.0f);

    [Export]
    Vector3 speed = new Vector3(0.0f, 0.0f, 0.0f);

    [Export]
    Vector3 max_speed = new Vector3(2.0f, 2.0f, 2.0f);
    
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        this.cam = this.GetNode<Godot.Camera>("CameraObj");
        this.cam.Current = true;

    }
    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    public override void _Process(float delta)
    {
        updateMouselook();
        updateMovement(delta);
    }
    /// <summary>
    /// An input event handler for the c# camera
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        
        
        if(@event is InputEventMouseButton && @event.IsAction("mouse_right_click"))
        {
            press=!press;
        }
        if(@event is InputEventMouseMotion && press)
        {
            InputEventMouseMotion _event = (InputEventMouseMotion)@event;
            mousePosition = _event.Relative;
        }
        
        if(@event.IsActionPressed(forward))
        {
            direction.z = -1f;
        }
        else if(@event.IsActionPressed(back))
        {
            direction.z = 1f;
        }
        else if(!@event.IsActionPressed(back) && !@event.IsActionPressed(forward)&&!@event.IsPressed())
        {
            if(@event is InputEventMouseMotion || @event is InputEventMouseButton)
            {
                
            }
            else
            {
                direction.z = 0f;
            }
            
        }
        if(@event.IsActionPressed(left))
        {
            direction.x = -1f;
        }
        else if(@event.IsActionPressed(right))
        {
            direction.x = 1f;
        }
        else if(!@event.IsActionPressed(left) && !@event.IsActionPressed(right)&&!@event.IsPressed())
        {
            if(@event is InputEventMouseMotion || @event is InputEventMouseButton)
            {
                
            }
            else
            {
                direction.x = 0f;
            }
            
        }
        if(@event.IsActionPressed(up))
        {
            direction.y = 1f;
        }
        else if(@event.IsActionPressed(down))
        {
            direction.y = -1f;
        }
        else if(!@event.IsActionPressed(up) &&!@event.IsActionPressed(down) && !@event.IsPressed())
        {
            if(@event is InputEventMouseMotion || @event is InputEventMouseButton)
            {
                
            }
            else
            {
                direction.y = 0f;
            }    
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        var space_state = GetWorld().DirectSpaceState;

        var raycast_from = cam.ProjectRayOrigin(GetTree().Root.GetMousePosition());
        var raycast_to = raycast_from + cam.ProjectRayNormal(GetTree().Root.GetMousePosition()) * 100;

        var result = space_state.IntersectRay(raycast_from, raycast_to, null, 0b11);
    }


    private void updateMouselook()
    {
        
        mousePosition *= sensitivity;
        _yaw = (float)_yaw * smoothness + mousePosition.x * (1.0f - smoothness);
        _pitch = _pitch * smoothness + mousePosition.y * (1.0f - smoothness);
        mousePosition = new Vector2(0, 0);

        if (yaw_limit < 360)
        {
            _yaw = Mathf.Clamp(_yaw, -yaw_limit - _total_yaw, yaw_limit - _total_yaw);
        }
            
        if (pitch_limit < 360)
        {
            _pitch = Mathf.Clamp(_pitch, -pitch_limit - _total_pitch, pitch_limit - _total_pitch);
        }
            
        _total_yaw += _yaw;
        _total_pitch += _pitch;
        this.RotateObjectLocal(new Vector3(1,0,0), Mathf.Deg2Rad(-_pitch));
        this.RotateY(Mathf.Deg2Rad(-_yaw));
    }

    private void updateMovement(float delta)
    {
        Vector3 offset = max_speed * acceleration * direction;
	
	    speed.x = Mathf.Clamp(speed.x + offset.x, -max_speed.x, max_speed.x);
	    speed.y = Mathf.Clamp(speed.y + offset.y, -max_speed.y, max_speed.y);
	    speed.z = Mathf.Clamp(speed.z + offset.z, -max_speed.z, max_speed.z);
	
	    
	    if (direction.x == 0)
        {
            speed.x *= (1.0f - deceleration);
        }
		
	    if (direction.y == 0)
        {
            speed.y *= (1.0f - deceleration);
        }
		
	    if (direction.z == 0)
        {
            speed.z *= (1.0f - deceleration);
        }
		
	    if (local)
        {
            this.Translate(speed * delta);
        } 	
	    else
        {
            this.GlobalTranslate(speed * delta);
        }
		    
    }
}