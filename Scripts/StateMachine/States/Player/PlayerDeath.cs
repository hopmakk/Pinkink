using Godot;
using Godot.Collections;
using System;

public partial class PlayerDeath : PlayerStateBase
{
    private const float MAX_GRAVITY = 200.0f;       // максимально возможная гравитация
    private GpuParticles2D GPUParticles2D;

    public override void _Ready()
    {
        base._Ready();
        GPUParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
    }

    public override void Enter()
    {
        _player.Anim.Play("death");
        GPUParticles2D.Restart();
    }


    public override void Exit()
    {
    }


    public override void PhysicsUpdate(double delta)
    {
        // Применяем скорость
        var velocity = _player.Velocity;

        if (velocity.Y < MAX_GRAVITY)
            velocity += _player.GetGravity() * (float)delta;

        if (velocity.X > 1)
        {
            velocity.X = velocity.X / 1.05f;
        }
        else
        {
            velocity.X = 0;
        }

        _player.Velocity = velocity;

        _player.MoveAndSlide();

        if (StateTransitonCheck())
            return;
    }


    private bool StateTransitonCheck()
    {
        if (Input.IsActionJustPressed("menu"))
        {
            _player.GlobalTransform = new Transform2D(0, Vector2.Zero);
            _player.HealthComponent.CurrentHP = 1;

            Args["AirStateParam"] = "";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            return true;
        }

        return false;
    }
}
