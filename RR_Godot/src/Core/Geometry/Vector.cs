namespace RR_Godot.Core.Geometry
{
    public class Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector3 copy)
        {
            this.x = copy.x;
            this.y = copy.y;
            this.z = copy.z;
        }
    }
}