using Godot;
using System;

public class Viewport2 : ViewportContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Viewport v1;
    Boolean t1;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        v1 = this.GetViewport();
       
        
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
      GD.Print(this.GetLocalMousePosition());
  }

}
