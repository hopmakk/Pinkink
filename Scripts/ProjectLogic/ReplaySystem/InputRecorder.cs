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


    private StateMachine _playerStateMachine;
    private List<string> _stateLog = new List<string>() { "initial" };


    public override void _Ready()
    {
        _playerStateMachine = GetNode<StateMachine>("../LevelBase/Entities/Player/StateMachine");
    }


    public override void _PhysicsProcess(double delta)
    {
        if (!Record) return;

        // Если началось новое состояние - записываем его
        var stateBeforeInput = _playerStateMachine.CurrentState.Name;
        if (_stateLog[^1] != stateBeforeInput)
            _stateLog.Add(stateBeforeInput);

        foreach (var action in ActionsNames)
        {
            var actionType = "";

            if (Input.IsActionJustPressed(action))
            {
                actionType = "pressed";
            }
            if (Input.IsActionJustReleased(action))
            {
                actionType = "released";
            }

            if (actionType != "")
                _inputLog.Add($"{Time.GetTicksMsec()}|{action}|{actionType}");
        }
    }


    public void SaveInputLog()
    {
        _stateLog.RemoveAt(0);

        var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        var directoryPath = Path.Combine(desktopPath, $"PlayTest_v{ProjectSettings.GetSetting("application/config/version")}");
        
        var inputsFilePath = $"{Time.GetTimeStringFromSystem()}i.txt".Replace(":", "");
        var statesFilePath = $"{Time.GetTimeStringFromSystem()}s.txt".Replace(":", "");

        inputsFilePath = Path.Combine(directoryPath, inputsFilePath);
        statesFilePath = Path.Combine(directoryPath, statesFilePath); 

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllLines(inputsFilePath, _inputLog);
        File.WriteAllLines(statesFilePath, _stateLog);
    }


    public override void _ExitTree()
    {
        if (!Record) return;
        SaveInputLog();
    }
}