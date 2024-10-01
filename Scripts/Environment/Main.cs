using Godot;
using PinkInk.Scripts.ProjectLogic;
using System;

public partial class Main : Node2D
{



	public override void _Ready()
	{
        TestSingleton.RunConsole();
    }


	public override void _Process(double delta)
	{     
        if (Input.IsMouseButtonPressed(MouseButton.Left))
            TestSingleton.ConsoleWriteL("-----------", ConsoleColor.White, ConsoleColor.Red);
	}
}
