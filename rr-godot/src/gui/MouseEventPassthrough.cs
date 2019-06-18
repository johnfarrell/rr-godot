using Godot;
using System;

public class MouseEventPassthrough : Control
{
    // DO NOT REMOVE THIS FILE, THE ENTIRE ENVIRONMENT INTERACTION WITH
    // MOUSE EVENTS (i.e. GIZMO) DEPENDS ON THIS

    public override void _UnhandledInput(InputEvent @event)
    {
        GetNode<Viewport>("VerticalSplit/HSplit1/Viewport1/Viewport")._UnhandledInput(@event);
    }
}
