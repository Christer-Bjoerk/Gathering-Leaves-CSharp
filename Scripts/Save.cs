using Godot;

public partial class Save : Node
{
    private string filePath = "user://saveDat.txt";

    // Default configurations
    public Godot.Collections.Dictionary<string, Variant> SaveData =
    new Godot.Collections.Dictionary<string, Variant>
    {
        {"MasterVolume", -10f},
        {"SFXVolume", -10f},
        {"LeafTimer", 1.5}
    };

    public void SaveGame(string keyName, Variant data)
    {
        // Open file to be overwritten
        using var saveGame = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);

        foreach (var (key, value) in SaveData)
        {
            // Update the data by checking if it has matching keys
            SaveData[keyName] = data;

            var jsonString = Json.Stringify(SaveData);

            saveGame.StoreLine(jsonString);
        }
    }

    public void LoadGame()
    {
        if (!FileAccess.FileExists(filePath))
        {
            return; // Error! We don't have a save to load.
        }

        using var saveGame = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

        while (saveGame.GetPosition() < saveGame.GetLength())
        {
            var jsonString = saveGame.GetLine();

            var json = new Json();

            // Check if there's anything wrong with the data
            var parseResult = json.Parse(jsonString);
            if (parseResult != Error.Ok)
            {
                GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
                continue;
            }

            SaveData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
        }
    }
}
