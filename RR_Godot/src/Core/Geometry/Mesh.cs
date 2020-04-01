using Godot;

namespace RR_Godot.Core
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
            Surface = new SurfaceTool();
            adder = new MeshAdder();
        }

        /// <summary>
        /// Sets the mesh into draw mode.
        /// <para>
        /// This needs to be called before any vertices or
        /// indices can be added
        /// </para>
        /// </summary>
        /// <param name="RenderType">
        /// Defines the way the mesh is drawn.
        /// <para>
        /// See <see cref="RR_Godot.Core.PrimitiveType"/> for more info.
        /// </para>
        /// </param>
        public void StartDrawing(PrimiteType RenderType)
        {
            GD.Print("STARTED MESH DRAWING");
            Godot.Mesh.PrimitiveType type = (Godot.Mesh.PrimitiveType) RenderType;
            Surface.Begin(type);
        }

        /// <summary>
        /// Finalizes mesh drawing and adds the built mesh to
        /// the world environment.
        /// </summary>
        public void CommitSurface()
        {
            GD.Print("COMMITING DRAWING");
            
            ArrayMesh finalSurface = Surface.Commit();

            adder.AddMesh(finalSurface);
        }

        /// <summary>
        /// Adds an index if building the mesh by indices.
        /// </summary>
        /// <param name="index"></param>
        public void AddIndex(int index)
        {
            Surface.AddIndex(index);
        }

        public void AddNormal(RR_Godot.Core.Geometry.Vector3Cp normal)
        {
            Surface.AddNormal(new Godot.Vector3(normal.x, normal.y, normal.z));
        }

        public void AddNormal(float x, float y, float z)
        {
            Surface.AddNormal(new Godot.Vector3(x, y, z));
        }

        /// <summary>
        /// Adds a vertex to the mesh being drawn.
        /// </summary>
        /// <param name="vertex">Vector3 object of the vertex.</param>
        public void AddVertex(RR_Godot.Core.Geometry.Vector3Cp vertex)
        {
            Surface.AddVertex(new Godot.Vector3(vertex.x, vertex.y, vertex.z));
        }

        /// <summary>
        /// Adds a vertex to the mesh being drawn.
        /// <para>Overload of <see cref="AddVertex"/></para>
        /// </summary>
        /// <param name="x">X-Coord of the vertex.</param>
        /// <param name="y">Y-Coord of the vertex.</param>
        /// <param name="z">Z-Coord of the vertex.</param>
        public void AddVertex(float x, float y, float z)
        {
            Surface.AddVertex(new Godot.Vector3(x, y, z));
        }
    }    
}