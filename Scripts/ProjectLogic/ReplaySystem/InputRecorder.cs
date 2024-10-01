using Godot;
using System.Collections.Generic;
using System.IO;

public partial class InputRecorder : Node
{
    [Export]
    public bool Record { get; set; } = false;

    private List<string> _inputLog = new List<string>();
    private List<string> ActionsNames = new List<string>()
    {
        "ui_right",
        "ui_left",
        "ui_up",
        "ui_down",
        "jump",
        "dash",
        "menu",
    };

    public override void _PhysicsProcess(double delta)
    {
        if (!Record) return;

        foreach (var action in ActionsNames)
        {
            if (Input.IsActionJustPressed(action))
            {
                _inputLog.Add($"{Time.GetTicksMsec()}|{action}|pressed");
            }
            if (Input.IsActionJustReleased(action))
            {
                _inputLog.Add($"{Time.GetTicksMsec()}|{action}|released");
            }
        }
    }


    public void SaveInputLog()
    {
        var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        var directoryPath = Path.Combine(desktopPath, $"PlayTest_v{ProjectSettings.GetSetting("application/config/version")}");
        
        var filePath = $"inputs_{Time.GetTimeStringFromSystem()}.txt".Replace(':', '_');
        filePath = Path.Combine(directoryPath, filePath); 

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllLines(filePath, _inputLog);
    }


    public override void _ExitTree()
    {
        GD.Print("Игра закрывается, удаление из сцены.");
        SaveInputLog();
    }
}