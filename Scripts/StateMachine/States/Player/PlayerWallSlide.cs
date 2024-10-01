using Godot;
using Godot.Collections;
using System;

public partial class PlayerWallSlide : PlayerStateBase
{
    public float SLIDE_SPEED = 50.0f;

    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter()
    {
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

        var velocity = _player.Velocity;

        velocity.Y = SLIDE_SPEED;

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
            // если игрок указывает вниз и прыгает - отцепляемся, иначе - прыгаем
            if (inputDirectionY == 1 && inputDirectionX == 0)
            {
                Args["AirStateParam"] = "";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            }
            else
            {
                Args["AirStateParam"] = "jumpWall";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
            }
                
            return true;
        }

        // dash
        if (Input.IsActionJustPressed("dash") && _player.DashAvailable)
        {
            Args["AirStateParam"] = "dash";
            EmitSignal(State.SignalName.Transitioned, this, "PlayerDash");
            return true;
        }

        //// wall Idle
        //if (Input.IsActionPressed("climb") && _player.IsOnWall())
        //{
        //    EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle");
        //    return true;
        //}

        // floor idle
        if (_player.IsOnFloor())
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle");
            return true;
        }

        // air (fall)
        if (!_player.IsOnWall() && !_player.IsOnFloor() )
            //|| (inputDirectionX * GetCollidedWallDirection() <= 0))
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
