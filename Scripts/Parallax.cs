using Godot;
using System;

public partial class Parallax : ParallaxBackground
{
    [Export]
    private float backgroundSpeed = 100f;

    public override void _Process(double delta)
    {
        ScrollOffset -= new Vector2(backgroundSpeed * (float)delta, 0f);
    }
}
