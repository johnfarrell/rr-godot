using Godot;
using System;
using System.Collections.Generic;

public class BotConnection : WindowDialog
{

    // Basic class to make working with the connections a little easier.
    public class ConnectionItem : IEquatable<ConnectionItem>
    {

        public Spatial baseNode { get; set; }
        public string connectName { get; set; }

        public int connId { get; set; }

        public override string ToString()
        {
            return connectName;
        }

        public override int GetHashCode()
        {
            return connId;
        }

        public override bool Equals(object obj)
        {
            if(obj == null) { return false; }
            ConnectionItem objAsConn = obj as ConnectionItem;
            if(objAsConn == null) { return false; }
            else return Equals(objAsConn);
        }

        public bool Equals(ConnectionItem other)
        {
            if(other == null) { return false; }
            return (this.connId.Equals(other.connId));
        }
    }
    
    [Signal]
    public delegate void ConnectionSent(string path);

    Button ui_connectBtn;
    Button ui_refreshBtn;
    ItemList ui_connectList;


    private List<ConnectionItem> connections;

    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ui_connectBtn = GetNode<Button>("VList/HBoxContainer/Connect");
        ui_refreshBtn = GetNode<Button>("VList/HBoxContainer/Refresh");

        ui_refreshBtn.Connect("pressed", this, "AddConnection");
        ui_connectBtn.Connect("pressed", this, "ConnectToSelected");
        ui_connectList = GetNode<ItemList>("VList/ConnectionList");

        
        connections = new List<ConnectionItem>();

        this.Connect("ConnectionSent", GetNode("/root/main/UI/AppWindow/EnvironmentContainer/ToolboxPanelFixed"), "NewArmConnection");
    }

    public void AddConnection()
    {
        Spatial env = GetNode<Spatial>("/root/main/env");
        for(var i = 0; i < env.GetChildCount(); i++)
        {
            if(env.GetChild(i).GetScript() == null)
            {
                continue;
            }
            else if(env.GetChild(i) is Arm)
            {
                GD.Print("adding connection");
                ConnectionItem new_item = new ConnectionItem
                {
                    connectName = env.GetChild(i).Name,
                    baseNode = (Spatial)env.GetChild(i)
                };
                AddConnectionItem(new_item);
            }
            else
            {
            }
        }
    }

    public void AddConnectionItem(ConnectionItem item)
    {
        item.connId = connections.Count;
        connections.Add(item);
        ui_connectList.AddItem(item.connectName);
    }

    public void ConnectRequest()
    {
        this.PopupCentered();
    }

    private void ConnectToSelected()
    {
        var selected = ui_connectList.GetSelectedItems();
        if(selected.Length == 0)
        {
            GD.Print("Please select an item!");
            return;
        }
        else if(selected.Length > 1)
        {
            GD.Print("Please only select one item!");
            return;
        }
        else
        {
            ConnectionItem selectedItem = connections[selected[0]];
            var path = selectedItem.baseNode.GetPath().ToString();
            EmitSignal("ConnectionSent", path);
        }
    }
}
