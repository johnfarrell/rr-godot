using Godot;
using System;

public class ViewportContainerTest : ViewportContainer
{
    // DO NOT REMOVE THIS FILE, THE ENTIRE ENVIRONMENT INTERACTION WITH
    // MOUSE EVENTS (i.e. GIZMO) DEPENDS ON THIS

    public override void _UnhandledInput(InputEvent @event)
    {
        GetNode<Viewport>("Viewport")._UnhandledInput(@event);
    }
}
