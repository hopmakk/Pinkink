using Godot;
using Godot.Collections;
using System;

public partial class PlayerWallRun : PlayerStateBase
{
    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter()
    {
        _player.PlayAnim("PlayerWallRun", 1.25f);
    }


    public override void PhysicsUpdate(double delta)
    {
        var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
        var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

        if (StateTransitonCheck(inputDirectionX, inputDirectionY))
            return;

        var velocity = _player.Velocity;

        velocity.Y = inputDirectionY * _player.Speed;

        _player.Velocity = velocity;

        _player.MoveAndSlide();
    }


    private bool StateTransitonCheck(float inputDirectionX, float inputDirectionY)
    {
        // death
        if (_player.HealthComponent.CurrentHP <= 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerDeath");
            return true;
        }

        // air (jump)
        if (Input.IsActionJustPressed("jump"))
        {
            Args["AirStateParam"] = "jump";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            return true;
        }

        // dash
        if (Input.IsActionJustPressed("dash") && _player.DashAvailable)
        {
            Args["AirStateParam"] = "dash";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerDash");
            return true;
        }

        // wall slide
        if (!Input.IsActionPressed("climb"))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallSlide");
            return true;
        }

        // air (fall) мы в воздухе если:
        // если мы не держимся за стену либо не находимся на поверхностях
        // А так же мы не ползем вверх когда сверху потолок (проверка на застревание)
        // А так же мы не ползем вниз когда сверху пол (проверка на застревание)
        if (!Input.IsActionPressed("climb")
            || !_player.IsOnWall()
            && !(_player.IsOnCeiling() && inputDirectionY < 0)
            && !(_player.IsOnFloor() && inputDirectionY > 0))
        {
            Args["AirStateParam"] = "";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            return true;
        }

        // wall idle
        if (inputDirectionY == 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle");
            return true;
        }

        return false;
    }
}
