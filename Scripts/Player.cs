using Godot;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Signal]
    public delegate void UpdateScoreEventHandler();

    [Signal]
    public delegate void GameOverEventHandler();

    [ExportCategory("Player Config")]
    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    [Export]
    public float ColorAlphaDecrease = 0.3f;

    [ExportGroup("Sound Effects")]
    [Export]
    public AudioStream CollectLeaves;
    [Export]
    public AudioStream Damaged;

    public int PlayerHealth { get; set; } = 3;

    private Vector2 ScreenSize; // Size of the game window.

    private AudioStreamPlayer AudioStreamPlayer;
    private AnimatedSprite2D AnimatedSprite2D;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        AudioStreamPlayer = GetNode<AudioStreamPlayer>("SFXPlayer");
        AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            AnimatedSprite2D.Play();
        }
        else
        {
            AnimatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        if (velocity.X != 0)
        {
            AnimatedSprite2D.Animation = "Run";
            AnimatedSprite2D.FlipV = false;
            // See the note below about boolean assignment.
            AnimatedSprite2D.FlipH = velocity.X < 0;
        }
        else if (velocity.X == 0)
        {
            AnimatedSprite2D.Animation = "Idle";
        }
    }

    private void _on_body_entered(PhysicsBody2D body)
    {
        body.QueueFree();
        EmitSignal(SignalName.UpdateScore);
        AudioStreamPlayer.Stream = CollectLeaves;
        AudioStreamPlayer.Play();
    }

    public void Start(Vector2 position)
    {
        Position = position;
    }

    public void OnLoseHealth()
    {
        PlayerHealth -= 1;
        AudioStreamPlayer.Stream = Damaged;
        AudioStreamPlayer.Play();

        Modulate = new Color(1,1,1, Modulate.A - ColorAlphaDecrease);

        if (PlayerHealth <= 0)
        {
            EmitSignal(SignalName.GameOver);
        }
    }
}