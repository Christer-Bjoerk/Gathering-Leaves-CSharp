using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene leafScene { get; set; }

    private int _score;

    // Nodes
    private Timer leafTimer;
    private Player player;
    private HUD hud;
    private Marker2D marker2D;
    private PauseMenu pauseMenu;

    public override void _Ready()
    {
        leafTimer = GetNode<Timer>("LeafTimer");
        player = GetNode<Player>("Player");
        hud = GetNode<HUD>("HUD");
        marker2D = GetNode<Marker2D>("StartPosition");
        pauseMenu = GetNode<PauseMenu>("PauseMenu");
        Save save = GetNode<Save>("/root/Save");

        leafTimer.WaitTime = (double)save.SaveData["LeafTimer"];
    }

    public void _OnHudStartGame()
    {
        NewGame();
    }

    public void NewGame()
    {
        PrepareGame();
    }

    public void GameOver()
    {
        leafTimer.Stop();
    }

    private void _OnLeafTimerTimeout()
    {
        // Create a new instance of the Leaf scene.
        Leaf leaf = leafScene.Instantiate<Leaf>();

        // Choose a random location on Path2D.
        var leafSpawnLocation = GetNode<PathFollow2D>("LeafPath/SpawnLeafLocation");
        leafSpawnLocation.ProgressRatio = GD.Randf();

        leaf.Position = leafSpawnLocation.Position;

        // Spawn the mob by adding it to the Main scene.
        AddChild(leaf);

        leaf.DamagePlayer += GetNode<Player>("Player").OnLoseHealth;
    }

    private void _OnPlayerUpdateScore()
    {
        _score += 1;

        hud.UpdateScore(_score);
    }

    public void _OnPlayerGameOver()
    {
        // Delete remaining leaves
        GetTree().CallGroup("Leaf", Node.MethodName.QueueFree);

        hud.GameOverMessage();
        leafTimer.Stop();
    }

    public void _ReturnToMainMenu()
    {
        leafTimer.Stop();
        pauseMenu.canPauseGame = false;
        pauseMenu.Hide();
        hud.GetNode<ColorRect>("StartMenu").Show();
        GetTree().CallGroup("Leaf", Node.MethodName.QueueFree);
    }

    public void _OnShowSettingsMenu()
    {
        hud.GetNode<ColorRect>("SettingsInGame").Show();
    }

    private async void PrepareGame()
    {
        player.PlayerHealth = 3;
        player.Modulate = new Color(1,1,1,1);

        _score = 0;
        hud.UpdateScore(_score);

        player.Start(marker2D.Position);

        hud.ShowMessage("Get Ready");

        // Delay for x seconds before triggering the next function
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        leafTimer.Start();

        pauseMenu.canPauseGame = true;
    }

    public void _UpdateLeafWaitTime(double value)
    {
        leafTimer.WaitTime = value;
    }
}