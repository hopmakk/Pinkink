using Godot;
using PinkInk.Scripts.ProjectLogic;
using System;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerFloorRun : State
    {
        private int _lastDirection;


        public override void _Ready()
        {
            base._Ready();
        }


        public override void Enter(Variant arg)
        {
            _parent.PlayAnim("PlayerFloorRun", 1.25f);
            _lastDirection = _parent.Direction;
        }


        public override void Exit()
        {
            Tween tween = _parent.AnimSpriteTween;
            tween.TweenProperty(_parent.Anim, "skew", 0.05, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", -0.05, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", 0.02, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", -0.02, 0.1f);
            tween.TweenProperty(_parent.Anim, "skew", 0, 0.1f);
        }


        public override void Update(double delta)
        {
        }


        public override void PhysicsUpdate(double delta)
        {
            var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
            var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

            // надо ли менять состояние
            StateTransitonCheck(inputDirectionX, inputDirectionY);

            // текущее направление персонажа
            if (inputDirectionX != 0)
                _parent.Direction = Mathf.Sign(inputDirectionX);

            // если оно отличается от предыдщуего - меняем анимацию
            if (_parent.Direction * _lastDirection < 0)
            {
                _parent.PlayAnim("PlayerFloorRun", 1.25f);
                _lastDirection = _parent.Direction;
            } 

            // Применяем скорость
            var velocity = _parent.Velocity;

            velocity.X = inputDirectionX * _parent.Speed;

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

            // floor idle
            if (inputDirectionX == 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
                return;
            }

            // wall idle
            if (_parent.IsOnWall() && inputDirectionY < 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
                return;
            }
        }
    }
}
