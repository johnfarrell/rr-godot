using Godot;
using System;

public class Arm : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Generic6DOFJoint J0;
    HingeJoint J1;
    HingeJoint J2;

    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        J0 = GetNode<Generic6DOFJoint>("J0");
        J1 = GetNode<HingeJoint>("J0/L0/J1");
        J2 = GetNode<HingeJoint>("J0/L1/J2");

        J0.AngularMotorZ__targetVelocity = 0F;
        J1.Motor__targetVelocity = 0F;
        J2.Motor__targetVelocity = 0F;
    }

    public void Stop()
    {
        up = false;
        down = false;
        left = false;
        right = false;
    }

    public void MoveUp()
    {
        up = true;
        down = false;
    }

    public void MoveDown()
    {
        up = false;
        down = true;
    }

    public void MoveRight()
    {
        right = true;
        left = false;
    }

    public void MoveLeft()
    {
        right = false;
        left = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        if(up)
        {
            J0.AngularMotorZ__targetVelocity = 0F;
            J1.Motor__targetVelocity = 1F;
            J2.Motor__targetVelocity = 1F;
        }
        else if(down)
        {
            J0.AngularMotorZ__targetVelocity = 0F;
            J1.Motor__targetVelocity = -1F;
            J2.Motor__targetVelocity = -1F;
        }
        else if(left)
        {
            J0.AngularMotorZ__targetVelocity = -1F;
            J1.Motor__targetVelocity = 0F;
            J2.Motor__targetVelocity = 0F;
        }
        else if(right)
        {
            J0.AngularMotorZ__targetVelocity = 1F;
            J1.Motor__targetVelocity = 0F;
            J2.Motor__targetVelocity = 0F;
        }
        else
        {
            J0.AngularMotorZ__targetVelocity = 0F;
            J1.Motor__targetVelocity = 0F;
            J2.Motor__targetVelocity = 0F;
        }
    }
}
