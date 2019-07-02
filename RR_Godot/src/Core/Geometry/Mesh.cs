using System;
using Godot;

namespace RR_Godot.Core.Geometry
{
    /// <summary>
    /// Used to command a mesh to draw vertices in a certain way.
    /// </summary>
    public enum PrimiteType
    {
        /// <summary>
        /// Each vertex is rendered as a single point
        /// </summary>
        POINTS = 0,
        /// <summary>
        /// Every two vertices are drawn as a line.
        /// </summary>
        LINES = 1,
        /// <summary>
        /// Every vertex is the next point in a line.
        /// </summary>
        LINE_STRIP = 2,
        /// <summary>
        /// Similar to LINE_STRIP except the first and last
        /// vertex are connected in a closed loop.
        /// </summary>
        LINE_LOOP = 3,
        /// <summary>
        /// Every three vertices are drawn as a triangle.
        /// </summary>
        TRIANGLES = 4,
        /// <summary>
        /// Draws the vertices as triangles where each triangle shares one complete
        /// edge with a neighbor and another with the next neighbor.
        /// </summary>
        TRIANGLE_STRIP = 5,
        /// <summary>
        /// Similar to TRIANGLE_STRIP except each triangle belongs to a
        /// set of triangles that shares one common vertex.
        /// </summary>
        TRIANGLE_FAN = 6
    }

    public class Mesh
    {
        private SurfaceTool Surface;

        private MeshAdder adder;

        public Mesh()
        {
            adder = GetNode
            Surface = new SurfaceTool();
        }

        public void StartDrawing(PrimiteType RenderType)
        {
            GD.Print("STARTED MESH DRAWING");
            Godot.Mesh.PrimitiveType type = (Godot.Mesh.PrimitiveType) RenderType;
            Surface.Begin(type);
        }

        public void CommitSurface()
        {
            GD.Print("COMMITING DRAWING");
            
            ArrayMesh finalSurface = Surface.Commit();
            Godot.Collections.Array arr = Surface.CommitToArrays();

            GD.Print(arr);
            var m = new MeshInstance();
            m.Mesh = finalSurface;

            MeshAdder.AddMesh(finalSurface);
            // GetTree().Root.GetNode("main/env").AddChild(m);
        }

        public void AddIndex(int index)
        {
            Surface.AddIndex(index);
        }

        public void AddVertex(RR_Godot.Core.Geometry.Vector3 vertex)
        {
            Godot.Vector3 gdCopy = new Godot.Vector3(vertex.x, vertex.y, vertex.z);

            Surface.AddVertex(gdCopy);
        }

        public void AddVertex(float x, float y, float z)
        {
            Surface.AddVertex(new Godot.Vector3(x, y, z));
        }

    }    
}