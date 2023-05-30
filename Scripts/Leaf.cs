using Godot;

public partial class Leaf : RigidBody2D
{
    [Signal]
    public delegate void DamagePlayerEventHandler();

    private void _OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
        EmitSignal(SignalName.DamagePlayer);
    }
}
