// ------ ViewportLL.cs ------
//
// These classes are used to handle mouse
// capturing and management with the multi-camera
// mode.
// Similar to the ViewportX.cs classes, this should
// be combined into a singular class and instanced in
// scenes to make viewport creation/deletion dynamic.

using Godot;
using System;

public class ViewportLL : Viewport
{
    Node c1;
    Viewport v1;

    String loc = "LL";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetChild(0).SetProcessInput(false);
        c1= this.GetChild(0);
        v1 = this;
    }
 
    //Input proccessor for UL hand camera
    // public override void _Input(InputEvent @event)
    // {  
    //     if ( @event is InputEventMouseMotion && mouseMoveIn())
    //     {
    //         c1.SetProcessInput(true);
    //     }
    //     else if (@event is InputEventMouseMotion && !mouseMoveIn())
    //     {
    //         c1.SetProcessInput(false);
    //     }
    // }

    private bool mouseMoveIn()
    {
        bool mouseLoc = false;
        Vector2 m = GetMousePosition();
        Vector2 v = GetViewport().Size;
        
        if((m.x < v.x && m.x >0)&&(m.y < v.y && m.y > 0))
        {
            mouseLoc = true;   
        }
        return mouseLoc;
    }
}
