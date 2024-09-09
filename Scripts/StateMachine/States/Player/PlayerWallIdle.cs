using Godot;
using System;

public partial class PlayerWallIdle : State
{
    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter(Variant arg)
    {
        var wallDir = GetWichWallCollided();
        if (wallDir != 0)
            _parent.Direction = wallDir;

        _parent.PlayAnim("PlayerWallIdle");
    }


    public override void Exit()
    {
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
        // air (jump)
        if (Input.IsActionJustPressed("jump"))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
            return true;
        }

        // dash
        if (Input.IsActionJustPressed("dash"))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerDash", default);
            return true;
        }

        // air (fall)
        if (!Input.IsActionPressed("climb") || !(_parent.IsOnFloor() || _parent.IsOnWall()))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            return true;
        }

        // wall run
        if (inputDirectionY != 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallRun", default);
            return true;
        }

        // floor idle
        if (_parent.IsOnFloor() && !Input.IsActionPressed("climb"))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
            return true;
        }

        return false;
    }


    // с какой стеной было соприкосновение в последний момент
    public int GetWichWallCollided()
    {
        var collision = _parent.GetLastSlideCollision();
        if (collision.GetNormal().X > 0)
            return -1;
        if (collision.GetNormal().X < 0)
            return 1;
        return 0;
    }
}
