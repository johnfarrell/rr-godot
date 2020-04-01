namespace RR_Godot.Core.Geometry
{
    public class Vector3Cp
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3Cp(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Cp(Vector3Cp copy)
        {
            this.x = copy.x;
            this.y = copy.y;
            this.z = copy.z;
        }
    }
}