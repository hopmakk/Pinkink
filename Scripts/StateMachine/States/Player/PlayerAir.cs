using Godot;
using System;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerAir : State
    {
        private const double COYOTE_JUMP_TIME = 0.1;    // время после падения, когда еще можно совершить прыжок
        private const double TODO_JUMP_TIME = 0.1;      // время запоминания, что надо сделать прыжок при приземлении
        private const float POST_DASH_Y_SPEED = -50.0f; // величина сглаживания дэша
        private const float MAX_GRAVITY = 200.0f;       // максимально возможная гравитация

        private int _lastDirection;
        private Vector2 _lastVelocity;
        private string _stateParam;    // с каким параметром мы перешли в это состояние 
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

            _stateParam = "";
            _lastVelocity = Vector2.Zero;
        }


        public override void Enter(Variant arg)
        {
            if (arg.VariantType == Variant.Type.String)
            {
                _stateParam = (string)arg;
                if (_stateParam == "jump")
                {
                    var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
                    var jumpDirection = new Vector2(inputDirectionX, -1);
                    _parent.Velocity = jumpDirection * _parent.JumpVelocity;
                    _parent.PlayAnim("PlayerAirJump", 2f);
                }

                else if (_stateParam == "jumpWall")
                {
                    var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
                    var jumpDirection = new Vector2(inputDirectionX, -1);
                    _parent.Velocity = jumpDirection * _parent.JumpVelocity;
                    _parent.PlayAnim("PlayerAir", 2f);
                }

                else if (_stateParam == "dash")
                {
                    _parent.Velocity = new Vector2(_parent.Velocity.X, POST_DASH_Y_SPEED);
                    _parent.PlayAnim("PlayerAir");
                }
            }
            else
            {
                _stateParam = "";
                _coyoteJumpTimer.Start();
                _parent.PlayAnim("PlayerAir");
            }

            _lastDirection = _parent.Direction;
        }


        public override void Exit()
        {
            _coyoteJumpTimer.Stop();
            _todoJumpTimer.Stop();

            Tween tween = _parent.AnimSpriteTween;
            tween.TweenProperty(_parent.Anim, "skew", 0.1, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", -0.1, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", 0.05, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", -0.05, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", 0.02, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", -0.02, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", 0, 0.1f);

            Tween tween1 = _parent.GetTree().CreateTween();
            tween1.TweenProperty(_parent.Anim, "scale", new Vector2(1.1f, 0.9f), 0.1f);
            tween1.TweenProperty(_parent.Anim, "scale", new Vector2(1.0f, 1.0f), 0.1f);
        }

        
        public override void PhysicsUpdate(double delta)
        {
            var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
            var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

            // текущее направление персонажа
            if (inputDirectionX != 0)
                _parent.Direction = Mathf.Sign(inputDirectionX);

            // если оно отличается от предыдщуего - меняем анимацию
            if (_parent.Direction * _lastDirection < 0)
            {
                _parent.PlayAnim("PlayerAir");
                _lastDirection = _parent.Direction;
            }

            // запоминаем, чтобы сделать прыжок при падении
            if (Input.IsActionJustPressed("jump"))
                _todoJumpTimer.Start();

            // Применяем скорость
            var velocity = _parent.Velocity;

            velocity.X = inputDirectionX * _parent.Speed;

            if (velocity.Y < MAX_GRAVITY)
                velocity += _parent.GetGravity() * (float)delta;

            _parent.Velocity = velocity;

            _parent.MoveAndSlide();

            if (StateTransitonCheck(inputDirectionX, inputDirectionY))
                return;

            _lastVelocity = _parent.Velocity;
        }


        private bool StateTransitonCheck(float inputDirectionX, float inputDirectionY)
        {
            // death
            if (_parent.HealthComponent.CurrentHP <= 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDeath", default);
                return true;
            }

            // air (coyot jump)
            if (Input.IsActionJustPressed("jump") && _coyoteJumpTimer.TimeLeft > 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return true;
            }

            // air (jump on floor)
            if (_todoJumpTimer.TimeLeft > 0 && _parent.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return true;
            }

            // dash
            if (Input.IsActionJustPressed("dash") && _stateParam != "dash")
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDash", default);
                return true;
            }

            // floor idle
            if (_parent.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
                if (_lastVelocity.Y >= MAX_GRAVITY)
                    GPUParticles2D.Restart();
                return true;
            }

            // wall
            if (_parent.IsOnWall())
            {
                // wall idle
                if (Input.IsActionPressed("climb"))
                {
                    EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
                    return true;
                }

                // wall slide (если игрок движется в сторону стены и вертикальная скорость направлена вниз)
                if (inputDirectionX * GetCollidedWallDirection() > 0 && _parent.Velocity.Y >= 0)
                {
                    EmitSignal(State.SignalName.Transitioned, this, "PlayerWallSlide", default);
                    return true;
                }
                
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
}
