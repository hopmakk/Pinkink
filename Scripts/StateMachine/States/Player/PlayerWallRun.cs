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


    public override void PhysicsUpdate(double delta)
    {
        var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
        var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

        if (StateTransitonCheck(inputDirectionX, inputDirectionY))
            return;

        var velocity = _parent.Velocity;

        velocity.Y = inputDirectionY * _parent.Speed;

        _parent.Velocity = velocity;

        _parent.MoveAndSlide();
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

        // wall slide
        if (!Input.IsActionPressed("climb"))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallSlide", default);
            return true;
        }

        // air (fall) мы в воздухе если:
        // если мы не держимся за стену либо не находимся на поверхностях
        // А так же мы не ползем вверх когда сверху потолок (проверка на застревание)
        // А так же мы не ползем вниз когда сверху пол (проверка на застревание)
        if (!_parent.IsOnWall()
            && !(_parent.IsOnCeiling() && inputDirectionY < 0)
            && !(_parent.IsOnFloor() && inputDirectionY > 0))
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
            return true;
        }

        // wall idle
        if (inputDirectionY == 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
            return true;
        }

        return false;
    }
}
