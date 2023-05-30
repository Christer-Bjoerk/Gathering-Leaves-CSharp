using Godot;
using System;

public partial class PauseMenu : ColorRect
{
    [Signal]
    public delegate void MainMenuEventHandler();

    [Signal]
    public delegate void ShowSettingsEventHandler();

    private bool isGamePaused = false;
    public bool canPauseGame = false;

    public override void _Ready()
    {
        Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("pause") && canPauseGame)
        {
            isGamePaused =! isGamePaused;
            Visible = isGamePaused;
            GetTree().Paused = isGamePaused;
        }
    }

    public void _OnResumeButtonPressed()
    {
        isGamePaused = false;
        Visible = isGamePaused;
        GetTree().Paused = isGamePaused;
    }

    public void _OnMainMenuButtonPressed()
    {
        EmitSignal(SignalName.MainMenu);
        isGamePaused = false;
        Visible = isGamePaused;
        GetTree().Paused = isGamePaused;

    }

    public void _OnSettingsButtonPressed()
    {
        isGamePaused = true;
        Visible = isGamePaused;
        GetTree().Paused = isGamePaused;

        EmitSignal(SignalName.ShowSettings);
    }

    public void _OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    public void _OnShowPauseMenu()
    {
        Show();

        isGamePaused = true;
        Visible = isGamePaused;
        GetTree().Paused = isGamePaused;
    }
}
