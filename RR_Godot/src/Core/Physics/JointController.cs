using Godot;
using System;
using System.Collections.Generic;


namespace RR_Godot.Core
{
    public class JointController : Node
    {
        // Called when the node enters the scene tree for the first time.

        // Holds a reference to all of the active joints in the world
        private List<Godot.Joint> _jointList;

        public override void _Ready()
        {
            _jointList = new List<Godot.Joint>();

            GetNode("/root/main/UI/AppWindow/LeftMenu/ObjectInspector/JointController").Connect(
                "MotorTargetChanged", this, "HingeMotorTargetChange");

            GD.Print("JointControl Singleton Loaded...");
        }

        public void RegisterJoint(Godot.Joint joint)
        {
            _jointList.Add(joint);
        }

        public void HingeMotorTargetChange(float newTarget, string jointName)
        {
            GD.Print(jointName + ": New Target speed of " + newTarget);
            Godot.HingeJoint reqJoint = (Godot.HingeJoint) FindJoint(jointName);

            if(reqJoint == null)
            {
                GD.Print("ERROR: Could not find requested joint [" + jointName + "]");
                return;
            }
            reqJoint.SetParam(Godot.HingeJoint.Param.MotorTargetVelocity, newTarget);
        }

        // Search predicate to find an item in _jointList by
        // the Godot.Joint.Name property
        private Predicate<Godot.Joint> ByName(string Name)
        {
            return delegate(Godot.Joint joint)
            {
                return joint.Name == Name;
            };
        }

        /// <summary>
        /// <para>FindJoint</para>
        /// Searches the active joints in the world and returns a
        /// Joint object if one exists by name jointName.
        /// </summary>
        /// <param name="jointName">Name of the joint to look for</param>
        /// <returns>
        /// Joint object containing the first joint that has the name specified.
        /// Returns null if there is no joint found.
        /// </returns>
        public Godot.Joint FindJoint(string jointName)
        {
            return _jointList.Find(ByName(jointName));
        }

        /// <summary>
        /// <para>FindAllJoints</para>
        /// Similar to FindJoint, but instead returns a list of all the joints
        /// that have the specified name.
        /// </summary>
        /// <param name="jointName">Joint Name to look for.</param>
        /// <returns></returns>
        public List<Godot.Joint> FindAllJoints(string jointName)
        {
            return _jointList.FindAll(ByName(jointName));
        }
    }

}