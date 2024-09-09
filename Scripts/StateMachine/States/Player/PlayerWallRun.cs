using Godot;
using System;

public partial class PlayerWallRun : State
{
    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter(Variant arg)
    {
        _parent.PlayAnim("PlayerWallRun", 1.25f);
    }


    public override void Exit()
    {
    }


    public override void PhysicsUpdate(double delta)
    {
        var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
        var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

        StateTransitonCheck(inputDirectionX, inputDirectionY);

        var velocity = _parent.Velocity;

        velocity.Y = inputDirectionY * _parent.Speed;

        _parent.Velocity = velocity;

        _parent.MoveAndSlide();
    }


    private void StateTransitonCheck(float inputDirectionX, float inputDirectionY)
    {
        // air (jump)
        if (Input.IsActionJustPressed("jump"))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
            return;
        }

        // air (fall)
        if (!(_parent.IsOnFloor() || _parent.IsOnWall()))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            return;
        }

        // wall idle
        if (inputDirectionY == 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
            return;
        }
        
        // floor idle
        if (_parent.IsOnFloor() && inputDirectionY > 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
            return;
        }
    }
}
