using Godot;
using System;


public class MouseEventPassthrough : Control
{
    // DO NOT REMOVE THIS FILE, THE ENTIRE ENVIRONMENT INTERACTION WITH
    // MOUSE EVENTS (i.e. GIZMO) DEPENDS ON THIS

    internal enum View{
        one = 0, two = 1, twoAlt = 2, three = 3, tAlt = 4,four = 5,
    }
    String HOR1 = "VerticalSplit/HSplit1";
    String HOR2 = "VerticalSplit/HSplit2";
    String V1 = "VerticalSplit/HSplit1/Viewport1";
    String V2 = "VerticalSplit/HSplit1/Viewport2";
    String V3 = "VerticalSplit/HSplit2/Viewport3";
    String V4 = "VerticalSplit/HSplit2/Viewport4";

    /// <summary>
    /// handles view switching between different numbers of viewports
    /// and alterantive styles of viewing those viewports
    /// </summary>
    /// <param name = "id" >the ID passed by the popmenu mouseclick event signal </param>
    private void toolbarViewItemPressed(int id)
    {
        GD.Print(id);
        GD.Print(this.GetPath()+HOR2);
        switch(id)
        {
            //make 2nd horizontal split and viewport 2, hidden
            case (int)View.one:
                GetNode<ViewportContainer>(V1).Visible = true;
                GetNode<Godot.HSplitContainer>(HOR2).Visible = false;
                GetNode<ViewportContainer>(V2).Visible = false;
                break;
            //make 2nd horizontal split hidden (viewport 1 +2 visible)    
            case (int)View.two:
                GetNode<ViewportContainer>(V1).Visible = true;
                GetNode<Godot.HSplitContainer>(HOR2).Visible = false;
                GetNode<ViewportContainer>(V2).Visible = true;
                break;

            //both horizontal viewports visible, viewports 1 and 3 only
            case (int)View.twoAlt:
                GetNode<ViewportContainer>(V1).Visible = true;
                GetNode<ViewportContainer>(V2).Visible = false;
                GetNode<Godot.HSplitContainer>(HOR2).Visible = true;
                GetNode<ViewportContainer>(V3).Visible = true;
                GetNode<ViewportContainer>(V4).Visible = false;
                break;
            //both horizontal visible, viewports 1,2, + 3
            case (int)View.three:
                GetNode<ViewportContainer>(V1).Visible = true;
                GetNode<ViewportContainer>(V2).Visible = true;
                GetNode<Godot.HSplitContainer>(HOR2).Visible = true;
                GetNode<ViewportContainer>(V3).Visible = true;
                GetNode<ViewportContainer>(V4).Visible = false;
                break;
            case (int)View.tAlt:
                GetNode<ViewportContainer>(V1).Visible = true;
                GetNode<ViewportContainer>(V2).Visible = false;
                GetNode<Godot.HSplitContainer>(HOR2).Visible = true;
                GetNode<ViewportContainer>(V3).Visible = true;
                GetNode<ViewportContainer>(V4).Visible = true;
                break;
            case (int)View.four:
                GetNode<ViewportContainer>(V1).Visible = true;
                GetNode<ViewportContainer>(V2).Visible = true;
                GetNode<Godot.HSplitContainer>(HOR2).Visible = true;
                GetNode<ViewportContainer>(V3).Visible = true;
                GetNode<ViewportContainer>(V4).Visible = true;
                break;
            default:
                break;
        }
    }
}
