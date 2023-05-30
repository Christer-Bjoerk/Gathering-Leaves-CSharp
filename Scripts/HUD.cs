using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    [Signal]
    public delegate void ShowPauseMenuEventHandler();

    [Signal]
    public delegate void UpdateLeafTimerWaitTimeEventHandler(double value);

    private string globalNodePath = "/root/GlobalSettings";

    // Nodes
    private ColorRect startMenu;
    private ColorRect creditsMenu;
    private ColorRect settingsMenu;
    private ColorRect inGameSettingsMenu;
    private Label scoreLabel;
    private Label messageLabel;
    private Label leafSpawnTimerLabel;
    private Label leafTimerSliderValue;
    private Timer msgTimer;
    private CheckButton checkButton;
    private Slider leafSpawnerTimerSlider;
    private GlobalSettings settings;

    public override void _Ready()
    {
        startMenu = GetNode<ColorRect>("StartMenu");
        creditsMenu = GetNode<ColorRect>("Credits");
        settingsMenu = GetNode<ColorRect>("Settings");
        inGameSettingsMenu = GetNode<ColorRect>("SettingsInGame");

        scoreLabel = GetNode<Label>("ScoreLabel");
        messageLabel = GetNode<Label>("Message");
        leafSpawnTimerLabel = GetNode<Label>("Settings/TabContainer/Setting/MarginContainer/GridContainer/LeafSpawnTimerLabel");
        leafTimerSliderValue = GetNode<Label>("Settings/TabContainer/Setting/MarginContainer/GridContainer/LeafSpawnTimerLabel/TimerValue");

        msgTimer = GetNode<Timer>("MessageTimer");
        settings = GetNode<GlobalSettings>("/root/GlobalSettings");
        checkButton = GetNode<CheckButton>("Settings/TabContainer/Setting/MarginContainer/GridContainer/CheckButton");
        leafSpawnerTimerSlider = GetNode<Slider>("Settings/TabContainer/Setting/MarginContainer/GridContainer/LeafSpawnTimerLabel/LeaftSpawnTimerContainer2/LeafSpawnTimerSlider");
        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        Save save = GetNode<Save>("/root/Save");
        Slider masterVolume = GetNode<Slider>("Settings/TabContainer/Setting/MarginContainer/GridContainer/MasterVolContainer/MasterVolumeSlider");
        Slider sfxVolume = GetNode<Slider>("Settings/TabContainer/Setting/MarginContainer/GridContainer/SFXVolContainer/SFXVolumeSlider");
        Slider leafTimerSlider = GetNode<Slider>("Settings/TabContainer/Setting/MarginContainer/GridContainer/LeafSpawnTimerLabel/LeaftSpawnTimerContainer2/LeafSpawnTimerSlider");
        save.LoadGame();

        masterVolume.Value = (float)save.SaveData["MasterVolume"];
        sfxVolume.Value = (float)save.SaveData["SFXVolume"];
        leafTimerSlider.Value = (double)save.SaveData["LeafTimer"]; 
        
    }

    public void _OnPlayButtonPressed()
    {
        EmitSignal(SignalName.StartGame);
        startMenu.Hide();
    }

    public void _OnReturnButtonPressed()
    {
        CanShowCreditsMenu(false);
    }

    public void _OnCreditsButtonPressed()
    {
        CanShowCreditsMenu(true);
    }

    public void _OnReturnButtonSettingsPressed()
    {
        CanShowSettingsMenu(false);
    }

    public void _OnSettingsButtonPressed()
    {
        CanShowSettingsMenu(true);
    }

    public void UpdateScore(int value)
    {
        scoreLabel.Text = value.ToString();
    }

    public void ShowMessage(string message)
    {
        messageLabel.Text = message;
        messageLabel.Show();

        msgTimer.Start();
    }

    public void _OnMessageTimerTimeout()
    {
        messageLabel.Hide();
    }

    public async void GameOverMessage()
    {
        ShowMessage("Game Over");
        // Wait for the timer to stop before continueing
        await ToSignal(msgTimer, Timer.SignalName.Timeout);

        startMenu.Show();
    }

    public void _OnMasterVolumeSliderValueChanged(float value)
    {
        settings.UpdateMasterVolume(value);
    }

    public void _OnSfxVolumeSliderValueChanged(float value)
    {
        settings.UpdateSFXVolume(value);
    }

    public void _OnReturnButtonSettingsInGamePressed()
    {
        inGameSettingsMenu.Hide();
        EmitSignal(SignalName.ShowPauseMenu);
    }

    public void _OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    private void CanShowCreditsMenu(bool value)
    {
        if (value)
        {
            creditsMenu.Show();
            startMenu.Hide();
        }
        else if (!value)
        {
            creditsMenu.Hide();
            startMenu.Show();
        }
    }

    private void CanShowSettingsMenu(bool value)
    {
        if (value)
        {
            settingsMenu.Show();
            startMenu.Hide();
        }
        else if (!value)
        {
            settingsMenu.Hide();
            startMenu.Show();
        }
    }

    public void _OnCheckButtonToggled(bool buttonPressed)
    {
        leafSpawnTimerLabel.Visible = buttonPressed ? true : false;
    }

    public void _OnLeafSpawnTimerSliderValueChanged(float value)
    {
        double waitTime = (double)value;
        EmitSignal(SignalName.UpdateLeafTimerWaitTime, waitTime);
        leafTimerSliderValue.Text = waitTime.ToString("0.0");
        settings.SaveLeafTimer(waitTime);
    }
}
