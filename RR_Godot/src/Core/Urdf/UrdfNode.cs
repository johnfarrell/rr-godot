using RosSharp.Urdf;
using System.Collections.Generic;

namespace RR_Godot.Core.Urdf
{
    public class UrdfNode
    {
        // Whether or not this node represents the root node
        public bool _isRoot { get; set; }

        public string _name { get; set; }

        // Parent node link
        public UrdfNode _parent { get; private set; }
        // Array of child node(s)
        private List<UrdfNode> _children { get; set; }

        // Link elemnt of this node
        public Link _link { get; private set; }
        // Joint element of this node
        public Joint _joint { get; private set; }

        // X Y Z offset from parent node in meters
        public double[] _offsetXyz { get; set; }
        // Roll (X), Pitch (Y), Yaw (Z) offset from
        // parent node in radians
        public double[] _offsetRpy { get; set; }

        UrdfNode()
        {
            _children = new List<UrdfNode>();
            _parent = null;
            _link = null;
            _joint = null;
        }
        public UrdfNode(UrdfNode parent, Link link, Joint joint)
        {
            _children = new List<UrdfNode>();
            _parent = parent;
            _link = link;
            _joint = joint;
        }

        /// <summary>
        /// <para>AddChild</para>
        /// Adds a child node to this node
        /// </summary>
        /// <param name="child">
        /// Fully defined UrdfNode to be
        /// added as the child.
        /// </param>
        public void AddChild(UrdfNode child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// <para>GetChild</para>
        /// Gets a specific child
        /// </summary>
        /// <param name="index">
        /// Number of the child to get in 0-index form
        /// <para>i.e. in the range of [0,GetNumChildren)</para>
        /// </param>
        /// <returns>UrdfNode containing the specific child.</returns>
        public UrdfNode GetChild(int index)
        {
            if ((index < 0) ||
                (index >= GetNumChildren()))
            {
                return null;
            }
            return _children[index];
        }

        /// <summary>
        /// <para>GetChildren</para>
        /// Gets the list of current children of this node.
        /// </summary>
        /// <returns>UrdfNode[] containing the children nodes.</returns>
        public UrdfNode[] GetChildren()
        {
            return _children.ToArray();
        }

        /// <summary>
        /// <para>HasChild</para>
        /// Tests to see whether or not this node has a specific child.
        /// </summary>
        /// <param name="node">UrdfNode containing the node in question.</param>
        /// <returns>True if node is a child, false if not</returns>
        public bool HasChild(UrdfNode node)
        {
            return _children.Contains(node);
        }

        /// <summary>
        /// <para>GetNumChildren</para>
        /// Returns the current count of children of this node.
        /// <para>Analogous to the number of joints a link has
        /// in raw Urdf.</para>
        /// </summary>
        /// <returns>Int containing the number of children.</returns>
        public int GetNumChildren()
        {
            return _children.Count;
        }

        /// <summary>
        /// <para>SetParent</para>
        /// Sets the parent of this node.
        /// </summary>
        /// <param name="parent">
        /// Fully specified UrdfNode to be the parent.
        /// Cannot be the same node you are calling the
        /// function from.
        /// </param>
        /// <returns>
        /// True if the parent was successfully set,
        /// false if otherwise.
        /// </returns>
        public bool SetParent(UrdfNode parent)
        {
            // Cant set a node with the same link as a parent
            // of itself
            if (this._link == parent._link)
            {
                return false;
            }
            this._parent = parent;
            return true;
        }
    }
}