using Godot;
using System;

public class EnvironmentTree : Tree
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // TreeItem root = this.CreateItem();
        // this.HideRoot = false;
        // root.SetText(0, "Environment");

        // TreeItem child1 = this.CreateItem(root);
        // child1.SetText(0, "Ground");
        // TreeItem child2 = this.CreateItem(root);
        // child2.SetText(0, "Light");
        // TreeItem subchild1 = this.CreateItem(child1);
        // subchild1.SetText(0, "Collision Mesh");

        UpdateTree();
    }

    public void UpdateTree()
    {
        Spatial env = GetNode<Spatial>("../../../EnvironmentContainer/Viewport/env");

        this.Clear();
        TreeItem root = this.CreateItem();
        this.HideRoot = false;
        root.SetText(0, "Environment");
        

        for(var i = 0; i < env.GetChildCount(); ++i)
        {
            TreeItem tempChild = this.CreateItem(root);
            Node tempNode = env.GetChild(i);

            tempChild.SetText(0, tempNode.Name);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
