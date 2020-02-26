using Godot;
using System;
using System.Collections.Generic;


namespace RR_Godot.Core.Physics
{
    public class JointController : Node
    {
        // Called when the node enters the scene tree for the first time.

        // Holds a reference to all of the active joints in the world
        public List<Godot.Joint> _jointList;

        private bool HingeTargetChanged;
        private float HingeTargetVal;
        private Godot.HingeJoint ReqHingeJoint;

        public override void _Ready()
        {

            _jointList = new List<Godot.Joint>();

            GetNode("/root/main/UI/AppWindow/LeftMenu/ObjectInspector/JointController").Connect(
                "MotorTargetChanged", this, "HingeMotorTargetChange");
            GetNode("/root/Global").Connect("UrdfFileAdded", this, "UpdateList");
            GD.Print("JointControl Singleton Loaded...");
        }

        public void UpdateList(RigidBody rootObj)
        {
            Queue<Spatial> nodeQueue = new Queue<Spatial>();

            foreach (var child in rootObj.GetChildren())
            {
                nodeQueue.Enqueue((Spatial) child);
            }

            // Standard BFS traversal of the tree structure
            Spatial curr = nodeQueue.Dequeue();
            while(curr != null)
            {
                if (!curr.GetType().Equals(typeof(Godot.PinJoint)) &&
                    !curr.GetType().Equals(typeof(Godot.HingeJoint)) &&
                    !curr.GetType().Equals(typeof(Generic6DOFJoint)))
                {
                    // If the current object isn't a joint, its a link so you need
                    // to add all of the children to the queue.
                    // Some of this will be a meshinstance/collisionshape, but those
                    // will get filtered out.
                    foreach (var child in curr.GetChildren())
                    {
                        nodeQueue.Enqueue((Spatial)child);
                    }
                    if (nodeQueue.Count == 0)
                    {
                        break;
                    }
                    curr = nodeQueue.Dequeue();
                    continue;
                }
                Godot.Joint tempJoint = (Godot.Joint)curr;

                RegisterJoint(tempJoint);

                nodeQueue.Enqueue((Spatial)curr.GetChild(0));

                if (nodeQueue.Count == 0)
                {
                    break;
                }
                curr = nodeQueue.Dequeue();
            }
        }

        public void RegisterJoint(Godot.Joint joint)
        {
            _jointList.Add(joint);
        }

        public void HingeMotorTargetChange(float newTarget, string jointName)
        {
            
            Godot.HingeJoint reqJoint = (Godot.HingeJoint) FindJoint(jointName);
            if(reqJoint == null)
            {
                GD.Print("ERROR: Could not find requested joint [" + jointName + "]");
                return;
            }
            HingeTargetVal = newTarget;
            HingeTargetChanged = true;
            ReqHingeJoint = reqJoint;
            GD.Print(jointName + ": New Target speed of " + newTarget);
        }

        public override void _PhysicsProcess(float delta)
        {
            if(HingeTargetChanged)
            {
                GD.Print("fjioefjieos");
                HingeTargetChanged = false;
                ReqHingeJoint.SetParam(HingeJoint.Param.MotorTargetVelocity, HingeTargetVal);
            }
            if(ReqHingeJoint != null)
            {
                UpdateInspectorInfo();
            }
        }

        public void UpdateInspectorInfo()
        {
            
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