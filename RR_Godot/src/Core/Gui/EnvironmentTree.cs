using Godot;
using System;

/// <summary>
/// Handles the processing for the tree-view of world objects on the lGD.Print("GRIDGENERATOR.CS: READY"); eft menu
/// </summary>
public class EnvironmentTree : Tree
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateTree();
        GD.Print("ENVIRONMENTTREE.CS: READY");
    }

    /// <summary>
    /// Gets the first depth of children of the root spatial node
    /// </summary>
    public void UpdateTree()
    {
        // TODO: Some way to serialize the nodes so the entire environment doesn't have to be
        //       iterated over to update it. Can be CPU intensive for large environments

        Spatial env = GetNode<Spatial>("/root/main/env");

        this.Clear();
        TreeItem root = this.CreateItem();
        this.HideRoot = false;
        root.SetText(0, "Environment");
        
        PopulateTree(root, env);
        root.Collapsed = false;
    }

    private void PopulateTree(TreeItem root, Spatial currEnvBase)
    {
        for(var i = 0; i < currEnvBase.GetChildCount(); ++i)
        {
            TreeItem tempChild = this.CreateItem(root);
            tempChild.SetText(0, currEnvBase.GetChild(i).Name);

            PopulateTree(tempChild, (Spatial) currEnvBase.GetChild(i));
        }
        root.Collapsed = true;
    }
}