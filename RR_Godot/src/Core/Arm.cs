// ------- Arm.cs ------
// Author: John Farrell
//          john@johnjfarrell.com
//
// This file contains the code to control the default
// 3DoF arm that is built into the environment.
// Just a very basic implementation of joint control
// through velocity targets.

using Godot;
using System;

public class Arm : StaticBody
{
    Generic6DOFJoint J0;
    HingeJoint J1;
    HingeJoint J2;

    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;

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
