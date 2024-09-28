using Godot;
using System;

public partial class PlayerDeath : State
{
    private const float MAX_GRAVITY = 200.0f;       // максимально возможная гравитация
    private GpuParticles2D GPUParticles2D;

    public override void _Ready()
    {
        base._Ready();
        GPUParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
    }

    public override void Enter(Variant arg)
    {
        _parent.Anim.Play("death");
        GPUParticles2D.Restart();
    }


    public override void Exit()
    {
    }


    public override void PhysicsUpdate(double delta)
    {
        // Применяем скорость
        var velocity = _parent.Velocity;

        if (velocity.Y < MAX_GRAVITY)
            velocity += _parent.GetGravity() * (float)delta;

        if (velocity.X > 1)
        {
            velocity.X = velocity.X / 1.05f;
        }
        else
        {
            velocity.X = 0;
        }

        _parent.Velocity = velocity;

        _parent.MoveAndSlide();

        if (StateTransitonCheck())
            return;
    }


    private bool StateTransitonCheck()
    {
        if (Input.IsActionJustPressed("menu"))
        {
            _parent.GlobalTransform = new Transform2D(0, Vector2.Zero);
            _parent.HealthComponent.CurrentHP = 1;
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            return true;
        }

        return false;
    }
}
