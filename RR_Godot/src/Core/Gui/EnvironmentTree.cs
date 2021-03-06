using Godot;
using System;

/// <summary>
/// Handles the processing for the tree-view of world objects on the lGD.Print("GRIDGENERATOR.CS: READY"); eft menu
/// </summary>
public class EnvironmentTree : Tree
{
    [Signal]
    public delegate void ObjectSelected(string name);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateTree();
        this.Connect("item_selected", this, "ItemSelected");
        GD.Print("ENVIRONMENTTREE.CS: READY");
    }

    public void ItemSelected()
    {
        GD.Print(GetSelected().GetText(0));
        EmitSignal("ObjectSelected", GetSelected().GetText(0));
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
            if(currEnvBase.GetChild(i).IsInGroup("SceneTreeIgnore"))
            {
                continue;
            }
            TreeItem tempChild = this.CreateItem(root);
            tempChild.SetText(0, currEnvBase.GetChild(i).Name);

            if(!currEnvBase.GetChild(i).IsInGroup("SceneTreeIgnore"))
            {
                PopulateTree(tempChild, (Spatial) currEnvBase.GetChild(i));
            }
        }
        root.Collapsed = true;
    }
}