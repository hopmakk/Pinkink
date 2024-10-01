using Godot;
using Godot.Collections;
using System;

public partial class PlayerWallIdle : PlayerStateBase
{
    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter()
    {
        // сбросим вертикальную скорость
        _player.Velocity = new Vector2(_player.Velocity.X, 0);

        // обновим направление игрока
        var wallDir = GetCollidedWallDirection();
        if (wallDir != 0)
            _player.Direction = wallDir;

        _player.PlayAnim("PlayerWallIdle");
    }


    public override void PhysicsUpdate(double delta)
    {
        var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
        var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

        if (StateTransitonCheck(inputDirectionX, inputDirectionY))
            return;
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

        // wall run
        if (inputDirectionY != 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallRun");
            return true;
        }

        // air (fall)
        if (!Input.IsActionPressed("climb")
            || !_player.IsOnFloor() && !_player.IsOnWall())
        {
            Args["AirStateParam"] = "";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            return true;
        }

        return false;
    }


    // с какой стеной было соприкосновение в последний момент
    public int GetCollidedWallDirection()
    {
        var collision = _player.GetLastSlideCollision();
        if (collision.GetNormal().X > 0)
            return -1;
        if (collision.GetNormal().X < 0)
            return 1;
        return 0;
    }
}
