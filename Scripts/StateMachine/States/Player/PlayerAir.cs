using Godot;
using Godot.Collections;
using System;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerAir : PlayerStateBase
    {
        private const double COYOTE_JUMP_TIME = 0.1;    // время после падения, когда еще можно совершить прыжок
        private const double TODO_JUMP_TIME = 0.1;      // время запоминания, что надо сделать прыжок при приземлении
        private const float POST_DASH_Y_SPEED = -100.0f; // величина сглаживания дэша
        private const float MAX_GRAVITY = 200.0f;       // максимально возможная гравитация

        private int _lastDirection;
        private Vector2 _lastVelocity;
        private GpuParticles2D GPUParticles2D;
        private Timer _coyoteJumpTimer;
        private Timer _todoJumpTimer;   


        public override void _Ready()
        {
            base._Ready();

            GPUParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

            _coyoteJumpTimer = GetNode<Timer>("CoyoteJump");
            _coyoteJumpTimer.WaitTime = COYOTE_JUMP_TIME;

            _todoJumpTimer = GetNode<Timer>("TodoJump");
            _todoJumpTimer.WaitTime = TODO_JUMP_TIME;

            _lastVelocity = Vector2.Zero;
        }


        public override void Enter()
        {
            if (Args["AirStateParam"] == "jump")
            {
                var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
                var jumpDirection = new Vector2(inputDirectionX, -1);
                _player.Velocity = jumpDirection * _player.JumpVelocity;
                _player.PlayAnim("PlayerAirJump", 2f);
            }

            else if (Args["AirStateParam"] == "jumpWall")
            {
                var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
                var jumpDirection = new Vector2(inputDirectionX, -1);
                _player.Velocity = jumpDirection * _player.JumpVelocity;
                _player.PlayAnim("PlayerAir", 2f);
            }

            else if (Args["AirStateParam"] == "dash")
            {
                _player.Velocity = new Vector2(_player.Velocity.X, POST_DASH_Y_SPEED);
                _player.PlayAnim("PlayerAir");
            }

            else
            {
                _coyoteJumpTimer.Start();
                _player.PlayAnim("PlayerAir");
            }

            _lastDirection = _player.Direction;
        }


        public override void Exit()
        {
            _coyoteJumpTimer.Stop();
            _todoJumpTimer.Stop();

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
            var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
            var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

            // текущее направление персонажа
            if (inputDirectionX != 0)
                _player.Direction = Mathf.Sign(inputDirectionX);

            // если оно отличается от предыдщуего - меняем анимацию
            if (_player.Direction * _lastDirection < 0)
            {
                _player.PlayAnim("PlayerAir");
                _lastDirection = _player.Direction;
            }

            // запоминаем, чтобы сделать прыжок при падении
            if (Input.IsActionJustPressed("jump"))
                _todoJumpTimer.Start();

            // Применяем скорость
            var velocity = _player.Velocity;

            velocity.X = inputDirectionX * _player.Speed;

            if (velocity.Y < MAX_GRAVITY)
                velocity += _player.GetGravity() * (float)delta;

            _player.Velocity = velocity;

            _player.MoveAndSlide();

            if (StateTransitonCheck(inputDirectionX, inputDirectionY))
                return;

            _lastVelocity = _player.Velocity;
        }


        private bool StateTransitonCheck(float inputDirectionX, float inputDirectionY)
        {
            // death
            if (_player.HealthComponent.CurrentHP <= 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDeath");
                return true;
            }

            // air (coyot jump)
            if (Input.IsActionJustPressed("jump") && _coyoteJumpTimer.TimeLeft > 0)
            {
                Args["AirStateParam"] = "jump";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
                return true;
            }

            // air (jump on floor)
            if (_todoJumpTimer.TimeLeft > 0 && _player.IsOnFloor())
            {
                Args["AirStateParam"] = "jump";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return true;
            }

            // dash
            if (Input.IsActionJustPressed("dash") && _player.DashAvailable)
            {
                Args["AirStateParam"] = "dash";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDash");
                return true;
            }

            // floor idle
            if (_player.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle");
                if (_lastVelocity.Y >= MAX_GRAVITY)
                    GPUParticles2D.Restart();
                return true;
            }

            // wall
            if (_player.IsOnWall())
            {
                //// wall idle
                //if (Input.IsActionPressed("climb"))
                //{
                //    EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle");
                //    return true;
                //}

                // wall slide (если игрок движется в сторону стены и вертикальная скорость направлена вниз)
                if (inputDirectionX * GetCollidedWallDirection() > 0 && _player.Velocity.Y >= 0)
                {
                    EmitSignal(State.SignalName.Transitioned, this, "PlayerWallSlide");
                    return true;
                }
                
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
}
