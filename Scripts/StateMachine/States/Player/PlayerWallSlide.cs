using Godot;
using System;

public partial class PlayerWallSlide : State
{
    public float SLIDE_SPEED = 50.0f;

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


    public override void PhysicsUpdate(double delta)
    {
        if (StateTransitonCheck())
            return;

        var velocity = _parent.Velocity;

        velocity.Y = SLIDE_SPEED;

        _parent.Velocity = velocity;

        _parent.MoveAndSlide();
    }


    private bool StateTransitonCheck()
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

        // wall Idle
        if (Input.IsActionPressed("climb") && _parent.IsOnWall())
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
            return true;
        }

        // air (fall)
        if (!_parent.IsOnWall() && !_parent.IsOnFloor())
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            return true;
        }

        // floor idle
        if (_parent.IsOnFloor())
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
