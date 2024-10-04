using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class InputReplayer : Node
{
    [Export]
    public bool Replay { get; set; } = false;

    private const string _directoryPath = "C:\\Users\\HOPMa\\OneDrive\\Рабочий стол\\PlayTest_v0.0.3\\";
    private const string _file = "131337";

    private List<string> _inputLog;
    private int _inputIndex = 0;
    private double _accumulatedTime = 0;

    private StateMachine _playerStateMachine;
    private RichTextLabel _replayLabel;

    private List<string> _stateLog;
    private int _stateIndex = 0;
    private int _synchrBreakIndex;

    public override void _Ready()
    {
        _playerStateMachine = GetNode<StateMachine>("../Level_test/Entities/Player/StateMachine");
        _replayLabel = GetNode<RichTextLabel>("../Level_test/UI/Control/ReplayLabel");
        _synchrBreakIndex = -1;

        // Читаем файлы
        _inputLog = new List<string>(File.ReadAllLines(_directoryPath + _file + "i.txt"));
        _stateLog = new List<string>(File.ReadAllLines(_directoryPath + _file + "s.txt"));
    }


    public override void _PhysicsProcess(double delta)
    {
        if (!Replay) return;
          
        // Добавляем время, прошедшее с последнего кадра
        _accumulatedTime += delta * 1000.0; // delta в секундах, переводим в миллисекунды

        StateCorrectableCheck();
        DoAction();
    }


    private void DoAction()
    {
        if (_inputIndex >= _inputLog.Count)
        {
            Replay = false;
            return;
        }

        string[] logEntry = _inputLog[_inputIndex].Split('|');
        ulong timestamp = ulong.Parse(logEntry[0]);
        string currentAction = logEntry[1];
        string currentActionTag = logEntry[2];

        // Проверяем, нужно ли выполнять текущее действие
        if (_accumulatedTime >= timestamp)
        {
            var labelText = "";
            labelText += $"{_inputIndex + 1}\t/ {_inputLog.Count}\n";

            if (_synchrBreakIndex >= 0)
                labelText += $"synchronization is broken ({_synchrBreakIndex})\n";

            _replayLabel.Text = labelText;

            // Выполняем действие
            if (currentActionTag == "pressed")
            {
                Input.ActionPress(currentAction);
            }
            else if (currentActionTag == "released")
            {
                Input.ActionRelease(currentAction);
            }

            _inputIndex++;
            DoAction();
        }
    }


    private void StateCorrectableCheck()
    {
        // проверяем если только есть синхронизация
        if (_synchrBreakIndex >= 0) return;

        var state = _playerStateMachine.CurrentState.Name;

        // если состояние обновилось
        if (state != _stateLog[_stateIndex])
        {
            _stateIndex++;

            // если это не то состояние, которое планировалось
            if (state != _stateLog[_stateIndex])
                _synchrBreakIndex = _stateIndex;
        }
    }
}