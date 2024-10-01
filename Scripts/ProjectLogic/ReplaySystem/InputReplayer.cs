using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class InputReplayer : Node
{
    [Export]
    public bool Replay { get; set; } = false;

    private const string _directoryPath = "C:\\Users\\HOPMa\\OneDrive\\Рабочий стол\\PlayTest_v0.0.3\\";
    private const string _file = "inputs_15_07_24.txt";

    private List<string> _inputLog;
    private int _currentIndex = 0;
    private ulong _startTime;

    public override void _Ready()
    {
        _inputLog = new List<string>(File.ReadAllLines(_directoryPath + _file));
        _startTime = Time.GetTicksMsec();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Replay) return;

        if (_currentIndex >= _inputLog.Count)
            return;

        string[] logEntry = _inputLog[_currentIndex].Split('|');

        ulong timestamp = ulong.Parse(logEntry[0]);
        string currentAction = logEntry[1];
        string currentactionTag = logEntry[2];
        var currentGameTime = Time.GetTicksMsec();

        // Проверяем, прошло ли достаточно времени для выполнения действия
        if (currentGameTime - _startTime >= timestamp)
        {
            if (currentactionTag == "pressed")
            {
                Input.ActionPress(currentAction);
            }
            else if (currentactionTag == "released")
            {
                Input.ActionRelease(currentAction);
            }

            _currentIndex++;
        }
    }
}
