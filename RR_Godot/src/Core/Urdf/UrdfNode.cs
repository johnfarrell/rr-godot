using Godot;
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
        public RosSharp.Urdf.Joint _joint { get; private set; }

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
        public UrdfNode(UrdfNode parent, Link link, RosSharp.Urdf.Joint joint)
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

        /// <summary>
        /// Creates a GLink object containing the necessary
        /// structures for representing this node
        /// inside of a 3D Godot environment.
        /// <para>
        /// Does not apply necessary transformations to 
        /// position the node in the 3D space.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public GLink CreateGLink()
        {
            GLink retVal = new GLink();

            // Create Rigid Body
            retVal._rigidBody.Name = _link.name;
            retVal._rigidBody.Mode = RigidBody.ModeEnum.Rigid;
            retVal._rigidBody.SetMass((float)_link.inertial.mass);

            // Create the MeshInstance
            retVal._meshInst.Mesh = CreateVisualGeometry(_link.visuals);
            retVal._meshInst.Name = _link.name + "_mesh";

            // Create the CollisionShape from _meshInstame = _link.name + "_collision";
            retVal._meshInst.CreateTrimeshCollision();
            // retVal._meshInst.cr
            StaticBody coll = (StaticBody)retVal._meshInst.GetChild(0);


            // Remove both children
            // retVal._meshInst.GetChild(0).RemoveChild(coll);
            // retVal._meshInst.RemoveChild(retVal._meshInst.GetChild(0));
            // var shape_owner = retVal._rigidBody.CreateShapeOwner(new Object());

            // retVal._rigidBody.ShapeOwnerAddShape(shape_owner,coll.ShapeOwnerGetShape(0,0));
            retVal._colShape.Name = _link.name + "_collision";

            // retVal._rigidBody.ShapeOwnerAddShape(0, retVal._colShape);

            // Add the meshinstance and collisionshape as children
            // of the rigidbody.
            retVal._rigidBody.AddChild(retVal._colShape);
            retVal._rigidBody.AddChild(retVal._meshInst);

            return retVal;
        }

        // TODO
        // * Functionalize this into seperate functions for each visual type
        /// <summary>
        /// <para>CreateVisualGeometry</para>
        /// Parses a ROS link and creates a MeshInstance for the visual
        /// geometry specified in that link.
        /// </summary>
        /// <param name="visuals">List of visuals stored in the Urdf link.</param>
        /// <returns>
        /// <para>
        /// A mesh containing the accurate visual geometry of the link if no error.
        /// <para>
        /// <para>
        /// Null if there was an error.
        /// </para></returns>
        public Godot.Mesh CreateVisualGeometry(List<Link.Visual> visuals)
        {
            // The union of the geometries defined in the list define
            // the end visual of the link, but for now we will just use
            // the first one
            if (visuals.Count < 1)
            {
                return null;
            }

            Link.Visual workingVis = visuals[0];

            // Create the material if it exists
            SpatialMaterial linkMat = new Godot.SpatialMaterial();
            if (workingVis.material != null)
            {
                var matColor = new Godot.Color();
                matColor.r = (float)workingVis.material.color.rgba[0];
                matColor.g = (float)workingVis.material.color.rgba[1];
                matColor.b = (float)workingVis.material.color.rgba[2];
                matColor.a = (float)workingVis.material.color.rgba[3];
                linkMat.AlbedoColor = matColor;
            }

            // Create the meshs
            if (workingVis.geometry.box != null)
            {
                double[] uSize = workingVis.geometry.box.size;

                CubeMesh temp = new CubeMesh();
                temp.Material = linkMat;
                temp.Size = new Vector3(
                    (float)uSize[0],
                    (float)uSize[2],
                    (float)uSize[1]
                );

                return temp;
            }
            if (workingVis.geometry.cylinder != null)
            {
                double length = workingVis.geometry.cylinder.length;
                double radius = workingVis.geometry.cylinder.radius;

                CylinderMesh temp = new CylinderMesh();
                temp.RadialSegments = 16;
                temp.Material = linkMat;
                temp.TopRadius = (float)radius;
                temp.BottomRadius = (float)radius;
                temp.Height = (float)length;


                return temp;
            }
            if (workingVis.geometry.sphere != null)
            {
                double radius = workingVis.geometry.sphere.radius;

                SphereMesh temp = new SphereMesh();
                temp.RadialSegments = 16;
                temp.Material = linkMat;
                temp.Radius = (float)radius;
                temp.Height = (float)(radius * 2.0);

                return temp;
            }
            if (workingVis.geometry.mesh != null)
            {
                string filename = workingVis.geometry.mesh.filename;
                double[] scale = workingVis.geometry.mesh.scale;

                // Right now we are just using a placeholder cylinder
                // until runtime importing is working
                CylinderMesh temp = new CylinderMesh();
                temp.RadialSegments = 16;
                temp.Height = 0.25F;
                temp.BottomRadius = 0.05F;
                temp.TopRadius = 0.05F;

                return temp;
            }
            return null;
        }
    }
}