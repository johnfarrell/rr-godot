using Godot;
using System;

/// <summary>
/// Handles the processing for the tree-view of world objects on the left menu
/// </summary>
public class EnvironmentTree : Tree
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateTree();
    }

    /// <summary>
    /// Gets the first depth of children of the root spatial node
    /// </summary>
    public void UpdateTree()
    {
        // TODO: Make this get all objects at any depth with accurate parent-child relations
        // TODO: Some way to serialize the nodes so the entire environment doesn't have to be
        //       iterated over to update it. Can be CPU intensive for large environments
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
}
