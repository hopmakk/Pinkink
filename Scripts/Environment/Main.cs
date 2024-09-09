using Godot;
using PinkInk.Scripts.ProjectLogic;
using System;

public partial class Main : Node2D
{
	private bool _consoleOpened;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        TestSingleton.RunConsole();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{     
        if (Input.IsMouseButtonPressed(MouseButton.Left))
            TestSingleton.ConsoleWriteL("-----------", ConsoleColor.White, ConsoleColor.Red);
	}
}
