using Godot;
using System;
using System.Xml;

//This namespace contains .urdf defined elements that can be imported into Godot
//each object can be instantiated and called to create an object in Godot based on .urdf specs
namespace UrdfParser
{
    
    public class Robot
    {
        public string name;
        System.Collections.ArrayList joints;
        System.Collections.ArrayList links;
        System.Collections.ArrayList materials
        
    }

    public class Link
    {
        String name;
        Material mat;

    }

    public class Joint
    {
        String name;
        Node parent;
        Node child;
        Node type;
        Vector3 axis
        vector3 origin
        bool fixed;

    }

     
    public class Pose
    {
        Vector3 xyz;
        Vector3 rpy;
    }

    public class Color
    {
        String rgba;
    }

    public class Inertia
    {
        float mass;
        Vector3 ixx;
        Vector3 ixy;
        Vector3 ixz;
        Vector3 iyy;
        Vector3 iyz;
        Vector3 izz;

    }

    public class Transmission
    {

    }


    public class Material
    {
        string name;
        Color color;
        Texture texture;
    }



    public class Geometry
    {
        Shape structure;
    }
    public class Box : Shape
    {
        Vector3 dim;
        Origin origin;
    }


    public class Cylinder : Shape
    {
        float radius;
        float length;
    }

    public class Mesh : Shape
    {
        string filename;
    }
    public class Visual
    {
        Geometry geo;
        Material mat;
    }

    

    public class Origin
    {
        Vector3 rpy;
        Vector3 xyz;
    }

    public abstract class Shape
    {

    }

}