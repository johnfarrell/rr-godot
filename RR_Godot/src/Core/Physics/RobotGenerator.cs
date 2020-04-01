using Godot;
using System;

namespace RR_Godot.Core.Physics
{
    public class RobotGenerator
    {
        enum robot_type
        {
            unknown,
            serial,
            dual_arm,
            differential_drive,
            planar,
            floating,
            freeform,
            other
        }

        string name { get; set; }

        robot_type type;
        int num_joints { get; set; }
        int num_links { get; set; }

        public RobotGenerator() {}
    }
}