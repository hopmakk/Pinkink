﻿using Godot;
using PinkInk.Scripts.ProjectLogic;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node2D
{
    [Export]
	public State InitialState { get; set; }

    public State CurrentState { get; set; }
	public Dictionary<string, State> States { get; set; }


	public override void _Ready()
	{
        States = new Dictionary<string, State>();

		foreach (State child in GetChildren())
		{
			States.Add(child.Name.ToString().ToLower(), child);
            child.Transitioned += OnChildTransition;
        }
	}


    // запуск машины извне
    public void Launch()
    {
        if (InitialState != null)
        {
            CurrentState = InitialState;
            CurrentState.Enter();
        }
    }


    public override void _Process(double delta)
	{
		if (CurrentState != null)
            CurrentState.Update(delta);
    }


    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState != null)
            CurrentState.PhysicsUpdate(delta);
    }


    private void OnChildTransition(State state, string newStateName)
    {
        if (state != CurrentState)
        {
            return;
        }
        
         var newState = States[newStateName.ToLower()];

        if (newState == null)
            newState = InitialState;

        state.Exit();
        newState.Args = state.Args;
        newState.Enter();
        CurrentState = newState;

        //---------------------------------
        var player = GetParent<Player>();
        var str = newStateName + $" / floor:{player.IsOnFloor()} wall:{player.IsOnWall()}";
        newStateName = newStateName.ToLower();
        if (newStateName.Contains("wall"))
            TestSingleton.ConsoleWriteL(str, ConsoleColor.Magenta);
        else if (newStateName.Contains("floor"))
            TestSingleton.ConsoleWriteL(str, ConsoleColor.Yellow);
        else if (newStateName.Contains("air"))
            TestSingleton.ConsoleWriteL(str, ConsoleColor.Blue);
        else if (newStateName.Contains("dash"))
            TestSingleton.ConsoleWriteL(str, ConsoleColor.Cyan);
        else if (newStateName.Contains("death"))
            TestSingleton.ConsoleWriteL(str, ConsoleColor.Red);
        Console.WriteLine();
    }
}
