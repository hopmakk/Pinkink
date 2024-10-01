using Godot;
using Godot.Collections;
using System;

public partial class PlayerDash : PlayerStateBase
{
    private const float DASH_SPEED = 500.0f;
    private const float DASH_LENGTH = 45.0f;

    private float _dashTime;

    private GpuParticles2D GPUParticles2D;

    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter()
    {
        _player.DashAvailable = false;
        _player.ModulateTween.Kill();
        _player.Anim.Modulate = Color.Color8(170, 255, 255);

        _dashTime = DASH_LENGTH / DASH_SPEED;
        GPUParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
        GPUParticles2D.Restart();

        var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        // текущее направление персонажа
        if (direction == Vector2.Zero)
            direction.X = _player.Direction;

        // Применяем скорость
        _player.Velocity = direction * DASH_SPEED;
    }


    public override void Exit()
    {
        Tween tween = _player.SkewTween;
        tween.TweenProperty(_player.Anim, "skew", 0.1, 0.1f);
        tween.TweenProperty(_player.Anim, "skew", -0.1, 0.1f);
        tween.TweenProperty(_player.Anim, "skew", 0.05, 0.1f);
        tween.TweenProperty(_player.Anim, "skew", -0.05, 0.1f);
        tween.TweenProperty(_player.Anim, "skew", 0.02, 0.1f);
        tween.TweenProperty(_player.Anim, "skew", -0.02, 0.1f);
        tween.TweenProperty(_player.Anim, "skew", 0, 0.1f);

        Tween tween1 = _player.GetTree().CreateTween();
        tween1.TweenProperty(_player.Anim, "scale", new Vector2(1.1f, 0.9f), 0.1f);
        tween1.TweenProperty(_player.Anim, "scale", new Vector2(1.0f, 1.0f), 0.1f);
    }


    public override void PhysicsUpdate(double delta)
    {
        _dashTime -= (float)delta;

        _player.MoveAndSlide();

        if (StateTransitonCheck())
            return;
    }


    private bool StateTransitonCheck()
    {
        // death
        if (_player.HealthComponent.CurrentHP <= 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerDeath");
            return true;
        }

        // air
        if (_dashTime <= 0)
        {
            Args["AirStateParam"] = "dash";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            return true;
        }

        return false;
    }
}
