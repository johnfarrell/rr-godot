using Godot;
using System;

namespace RR_Godot.Core.Urdf
{
    // TODO
    /// <summary>
    /// Class to translate between a Godot physics
    /// representation of a Urdf Link
    /// </summary>
    public class GLink
    {
    public RigidBody _rigidBody { get; set; }
        public CollisionShape _colShape { get; set; }
        public MeshInstance _meshInst { get; set; }

        public GLink()
        {
            _rigidBody = new RigidBody();
            _colShape = new CollisionShape();
            _meshInst = new MeshInstance();
        }
    }
}
