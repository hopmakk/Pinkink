﻿using Godot;
using Godot.Collections;
using static Godot.TextServer;

public partial class EntityBase : CharacterBody2D
{
    public float Speed { get; set; }            // скорость персонажа
    public float JumpVelocity { get; set; }     // скорость прыжка
    public int Direction { get; set; }          // текущее направление, куда смотрит персонаж
    public AnimatedSprite2D Anim { get; set; }  // AnimatedSprite2D
    public HealthComponent HealthComponent { get; set; }  // HealthComponent
    public Dictionary<string, string[]> AnimNamesWithDirection { get; set; }    // названия анимаций и их интерпритация для разных направлений


    public override void _Ready()
    {
        Anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        HealthComponent = GetNode<HealthComponent>("Components/HealthComponent");
        AnimNamesWithDirection = new Dictionary<string, string[]>();
        Direction = 1;

        var stateMachine = GetNodeOrNull<StateMachine>("StateMachine");
        if (stateMachine != null)
            stateMachine.Launch();
    }


    public override void _PhysicsProcess(double delta)
    {

    }

    
    // проигрывать анимацию с выбором направления
    public void PlayAnim(string name, float speed = 1, bool fromEnd = false)
    {
        if (!AnimNamesWithDirection.ContainsKey(name))
            return;

        var selectedAnimNames = AnimNamesWithDirection[name];

        if (Direction > 0)
            Anim.Play(selectedAnimNames[1], speed, fromEnd);
        else if (Direction < 0)
            Anim.Play(selectedAnimNames[0], speed, fromEnd);
    }

}
