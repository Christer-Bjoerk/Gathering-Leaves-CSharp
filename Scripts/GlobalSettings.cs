using Godot;
using System;

public partial class GlobalSettings : Node
{
    private Save save;

    public override void _Ready()
    {
        save = GetNode<Save>("/root/Save");
    }

    public void UpdateMasterVolume(float value)
    {
        AudioServer.SetBusVolumeDb(0, value);
        save.SaveGame("MasterVolume", value);
    }

    public void UpdateSFXVolume(float value)
    {
        AudioServer.SetBusVolumeDb(1, value);
        save.SaveGame("SFXVolume", value);
    }

    public void SaveLeafTimer(double value)
    {
        save.SaveGame("LeafTimer", value);
    }
}
