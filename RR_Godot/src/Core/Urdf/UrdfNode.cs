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
        /// Adds a child node to this node.
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
        public RigidBody CreateLink()
        {
            RigidBody retVal = new RigidBody();

            // Create Rigid Body
            retVal.Name = _link.name;
            retVal.Mode = RigidBody.ModeEnum.Rigid;
            retVal.SetMass((float)_link.inertial.mass);


            // Create the MeshInstance
            MeshInstance tempMesh = new MeshInstance();
            tempMesh.Mesh = CreateVisualGeometry(_link.visuals);
            tempMesh.Name = _link.name + "_mesh";

            // Add the collision information to the RigidBody
            Shape colShape = CreateCollisionGeometry(_link.collisions);

            retVal.AddChild(tempMesh);

            return retVal;
        }

        /// <summary>
        /// <para>CreateCollisionGeometry</para>
        /// Similar to CreateVisualGeometry, except it uses a links
        /// collision information to generate a Godot Shape.
        /// </summary>
        /// <param name="collisions">List of Collision information.</param>
        /// <returns></returns>
        public Shape CreateCollisionGeometry(List<Link.Collision> collisions)
        {
            if (collisions.Count < 1)
            {
                return null;
            }
            Link.Collision workingCol = collisions[0];

            if (workingCol.geometry.box != null)
            {
                return CreateBox(workingCol.geometry.box).CreateConvexShape();
            }
            if (workingCol.geometry.cylinder != null)
            {
                return CreateCylinder(workingCol.geometry.cylinder).CreateConvexShape();
            }
            if (workingCol.geometry.sphere != null)
            {
                return CreateSphere(workingCol.geometry.sphere).CreateConvexShape();
            }
            if (workingCol.geometry.mesh != null)
            {
                return CreateMesh(workingCol.geometry.mesh).CreateConvexShape();
            }
            return null;
        }

        // TODO
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
            SpatialMaterial linkMat = CreateMaterial(workingVis.material);

            // Create the meshs
            if (workingVis.geometry.box != null)
            {
                return CreateBox(workingVis.geometry.box, linkMat);
            }
            if (workingVis.geometry.cylinder != null)
            {
                return CreateCylinder(workingVis.geometry.cylinder, linkMat);
            }
            if (workingVis.geometry.sphere != null)
            {
                return CreateSphere(workingVis.geometry.sphere, linkMat);
            }
            if (workingVis.geometry.mesh != null)
            {
                return CreateMesh(workingVis.geometry.mesh);
            }
            return null;
        }

        private SpatialMaterial CreateMaterial(Link.Visual.Material baseMat)
        {
            if (baseMat == null)
            {
                return null;
            }

            SpatialMaterial temp = new SpatialMaterial();
            var matColor = new Godot.Color();
            matColor.r = (float)baseMat.color.rgba[0];
            matColor.g = (float)baseMat.color.rgba[1];
            matColor.b = (float)baseMat.color.rgba[2];
            matColor.a = (float)baseMat.color.rgba[3];
            temp.AlbedoColor = matColor;

            return temp;
        }

        private Godot.Mesh CreateBox(
            Link.Geometry.Box box,
            SpatialMaterial mat = null)
        {
            CubeMesh temp = new CubeMesh();

            if (mat != null)
            {
                temp.Material = mat;
            }
            temp.Size = new Vector3(
                (float)box.size[0],
                (float)box.size[2],
                (float)box.size[1]
            );

            return temp;
        }

        private Godot.Mesh CreateCylinder(
            Link.Geometry.Cylinder cyl,
            SpatialMaterial mat = null)
        {
            CylinderMesh temp = new CylinderMesh();

            if (mat != null)
            {
                temp.Material = mat;
            }

            temp.RadialSegments = 16;
            temp.TopRadius = (float)cyl.radius;
            temp.BottomRadius = (float)cyl.radius;
            temp.Height = (float)cyl.length;

            return temp;
        }

        private Godot.Mesh CreateSphere(
            Link.Geometry.Sphere sphere,
            SpatialMaterial mat = null)
        {
            SphereMesh temp = new SphereMesh();
            if (mat != null)
            {
                temp.Material = mat;
            }

            temp.RadialSegments = 16;
            temp.Radius = (float)sphere.radius;
            temp.Height = (float)(sphere.radius * 2.0);

            return temp;
        }

        /// <summary>
        /// <para>CreateMesh</para>
        /// Creates a Godot mesh instance using a Urdf
        /// defined Mesh geometry object.
        /// </summary>
        /// <param name="mesh">Urdf mesh object generated from a link.</param>
        /// <param name="mat">Optional material to apply to the mesh.</param>
        /// <returns>A Godot.Mesh representing the Urdf mesh data.</returns>
        private Godot.Mesh CreateMesh(
            Link.Geometry.Mesh mesh,
            SpatialMaterial mat = null)
        {
            // Temporary placeholder code
            CylinderMesh temp = new CylinderMesh();
            temp.RadialSegments = 16;
            temp.Height = 0.25F;
            temp.BottomRadius = 0.05F;
            temp.TopRadius = 0.05F;

            return temp;
        }
    }
}