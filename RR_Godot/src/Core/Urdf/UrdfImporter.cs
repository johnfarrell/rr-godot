using System.Collections.Generic;

using Godot;
using RosSharp.Urdf;

namespace RR_Godot.Core.Urdf
{
    public class UrdfImporter
    {
        private void UrdfPrint(string msg, int indent = 0)
        {
            string form_msg = " U\t";
            form_msg = form_msg.PadRight(indent, ' ');
            form_msg += msg;
            GD.Print(form_msg);
        }

        private bool HandleJoints(List<RosSharp.Urdf.Joint> joint_list)
        {
            UrdfPrint("Parsing joints...");
            foreach (var joint in joint_list)
            {
                UrdfPrint("\tFound Joint: " + joint.name);
            }
            return false;
        }

        private bool HandleLinks(List<RosSharp.Urdf.Link> link_list)
        {
            UrdfPrint("Parsing links...");
            foreach (var link in link_list)
            {
                UrdfPrint("\tFound Link: " + link.name);

                UrdfPrint("\t  parsing link visuals...");
                foreach (var vis in link.visuals)
                {
                    try
                    {
                        UrdfPrint("\t\tVisual: " + vis.geometry.mesh.filename);
                    }
                    catch
                    {
                        // GD.Print("No visual data");
                    }
                }
            }
            return false;
        }

        private bool HandleMaterials(List<Link.Visual.Material> mat_list)
        {
            UrdfPrint("Parsing materials...");
            foreach (var mat in mat_list)
            {
                UrdfPrint("\tFound Material: " + mat.name);
            }

            return false;
        }

        public void PrintTree(Link base_link, int level = 0)
        {
            int count = 1;

            foreach (var joint in base_link.joints)
            {
                int child_count = joint.ChildLink.joints.Count;
                string status = "(" + count + ") - Link " + joint.ChildLink.name + " has " + child_count + " children";
                UrdfPrint(status, level);

                PrintTree(joint.ChildLink, level + 2);
                count += 1;
            }
        }

        public bool Parse(string file_name)
        {
            UrdfPrint("Parsing file: " + file_name);
            Robot t = new Robot(file_name);

            UrdfPrint("Robot name: " + t.name);

            int child_count = t.root.joints.Count;
            string status = "Root " + t.root.name + " has " + child_count + " children";
            UrdfPrint(status);

            PrintTree(t.root, 2);
            // HandleJoints(t.joints);
            // HandleLinks(t.links);
            // HandleMaterials(t.materials);

            return false;
        }
    }
}