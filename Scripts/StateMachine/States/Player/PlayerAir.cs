using Godot;
using PinkInk.Scripts.ProjectLogic;
using System;
using System.Reflection.Emit;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerAir : State
    {
        private const double COYOTE_JUMP_TIME = 0.1;   // время после падения, когда еще можно совершить прыжок
        private const double TODO_JUMP_TIME = 0.1; // время запоминания, что надо сделать прыжок при приземлении

        private int _lastDirection;
        private Timer _coyoteJumpTimer;
        private Timer _todoJumpTimer;


        public override void _Ready()
        {
            base._Ready();
            _coyoteJumpTimer = GetNode<Timer>("CoyoteJump");
            _coyoteJumpTimer.WaitTime = COYOTE_JUMP_TIME;

            _todoJumpTimer = GetNode<Timer>("TodoJump");
            _todoJumpTimer.WaitTime = TODO_JUMP_TIME;
        }


        public override void Enter(Variant arg)
        {
            if (arg.VariantType == Variant.Type.String)
            {
                if ((string)arg == "jump")
                {
                    _parent.Velocity = new Vector2(_parent.Velocity.X, _parent.JumpVelocity);
                    _parent.PlayAnim("PlayerAirJump", 2f);
                }
            }
            else
            {
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
                _parent.PlayAnim("PlayerFloorRun", 1.25f);
                _lastDirection = _parent.Direction;
            }

            // запоминаем, чтобы сделать прыжок при падении
            if (Input.IsActionJustPressed("jump"))
                _todoJumpTimer.Start();

            // Применяем скорость
            var velocity = _parent.Velocity;

            velocity.X = inputDirectionX * _parent.Speed;

            velocity += _parent.GetGravity() * (float)delta;

            _parent.Velocity = velocity;

            _parent.MoveAndSlide();

            StateTransitonCheck(inputDirectionX, inputDirectionY);
        }


        private void StateTransitonCheck(float inputDirectionX, float inputDirectionY)
        {
            // air (coyot jump)
            if (Input.IsActionJustPressed("jump") && _coyoteJumpTimer.TimeLeft > 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return;
            }

            // air (jump on floor)
            if (_todoJumpTimer.TimeLeft > 0 && _parent.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return;
            }

            // wall idle (Если мы на стене и движемся в сторону стены)
            if (_parent.IsOnWall() && (GetWichWallCollided() * inputDirectionX > 0))
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
                return;
            }

            // floor idle
            if (_parent.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
                return;
            }
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
}
