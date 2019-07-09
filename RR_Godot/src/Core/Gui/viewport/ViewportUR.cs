using Godot;
using System;

public class ViewportUR : Viewport
{
    Node c1;
    Viewport v1;

    String loc = "UR";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetChild(0).SetProcessInput(false);
        c1= this.GetChild(0);
        v1 = this;
    }
    
    //Input proccessor for UL hand camera
    public override void _Input(InputEvent @event)
    {  
        if ( @event is InputEventMouseMotion && mouseMoveIn())
        { 
            c1.SetProcessInput(true);
        }
        else if (@event is InputEventMouseMotion && !mouseMoveIn())
        {
            c1.SetProcessInput(false);
        }
    }

    private bool mouseMoveIn()
    { 
        bool mouseLoc = false;
        Vector2 m = GetMousePosition();
        Vector2 v = GetViewport().Size;

        if( (m.x < v.x && m.x > 0)&&(m.y < v.y && m.y > 0))
        {
            mouseLoc = true;
        }
        return mouseLoc;
    }
}
