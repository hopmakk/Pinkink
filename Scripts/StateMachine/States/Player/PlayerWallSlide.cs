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
        var wallDir = GetCollidedWallDirection();
        if (wallDir != 0)
            _parent.Direction = wallDir;

        _parent.PlayAnim("PlayerWallIdle");
    }


    public override void PhysicsUpdate(double delta)
    {
        var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
        var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

        if (StateTransitonCheck(inputDirectionX, inputDirectionY))
            return;

        var velocity = _parent.Velocity;

        velocity.Y = SLIDE_SPEED;

        _parent.Velocity = velocity;

        _parent.MoveAndSlide();
    }


    private bool StateTransitonCheck(float inputDirectionX, float inputDirectionY)
    {
        // death
        if (_parent.HealthComponent.CurrentHP <= 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerDeath", default);
            return true;
        }

        // air (jump)
        if (Input.IsActionJustPressed("jump"))
        {
            // если игрок указывает вниз и прыгает - отцепляемся, иначе - прыгаем
            if (inputDirectionY == 1 && inputDirectionX == 0)
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            else
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jumpWall");
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

        // floor idle
        if (_parent.IsOnFloor())
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
            return true;
        }

        // air (fall)
        if (!_parent.IsOnWall() && !_parent.IsOnFloor() )
            //|| (inputDirectionX * GetCollidedWallDirection() <= 0))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            return true;
        }

        return false;
    }


    // с какой стеной было соприкосновение в последний момент
    public int GetCollidedWallDirection()
    {
        var collision = _parent.GetLastSlideCollision();
        if (collision.GetNormal().X > 0)
            return -1;
        if (collision.GetNormal().X < 0)
            return 1;
        return 0;
    }
}
