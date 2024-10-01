using Godot;
using Godot.Collections;
using PinkInk.Scripts.ProjectLogic;
using System;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerFloorRun : PlayerStateBase
    {
        private int _lastDirection;


        public override void _Ready()
        {
            base._Ready();
        }


        public override void Enter()
        {
            _player.PlayAnim("PlayerFloorRun", 1.25f);
            _lastDirection = _player.Direction;
        }


        public override void Exit()
        {
            Tween tween = _player.SkewTween;
            tween.TweenProperty(_player.Anim, "skew", 0.05, 0.1f);
            tween.TweenProperty(_player.Anim, "skew", -0.05, 0.1f);
            tween.TweenProperty(_player.Anim, "skew", 0.02, 0.1f);
            tween.TweenProperty(_player.Anim, "skew", -0.02, 0.1f);
            tween.TweenProperty(_player.Anim, "skew", 0, 0.1f);
        }


        public override void Update(double delta)
        {
        }


        public override void PhysicsUpdate(double delta)
        {
            var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
            var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

            // надо ли менять состояние
            if (StateTransitonCheck(inputDirectionX, inputDirectionY))
                return;

            // текущее направление персонажа
            if (inputDirectionX != 0)
                _player.Direction = Mathf.Sign(inputDirectionX);

            // если оно отличается от предыдщуего - меняем анимацию
            if (_player.Direction * _lastDirection < 0)
            {
                _player.PlayAnim("PlayerFloorRun", 1.25f);
                _lastDirection = _player.Direction;
            } 

            // Применяем скорость
            var velocity = _player.Velocity;

            velocity.X = inputDirectionX * _player.Speed;

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
                Args["AirStateParam"] = "jump";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
                return true;
            }

            // dash
            if (Input.IsActionJustPressed("dash") && _player.DashAvailable)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDash");
                return true;
            }

            // air (fall)
            if (!(_player.IsOnFloor() || _player.IsOnWall()))
            {
                Args["AirStateParam"] = "";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
                return true;
            }

            // floor idle
            if (inputDirectionX == 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle");
                return true;
            }

            //// wall idle
            //if (_player.IsOnWall() && Input.IsActionPressed("climb"))
            //{
            //    EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle");
            //    return true;
            //}

            return false;
        }
    }
}
