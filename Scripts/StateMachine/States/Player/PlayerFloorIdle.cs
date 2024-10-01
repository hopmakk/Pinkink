using Godot;
using Godot.Collections;
using System;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerFloorIdle : PlayerStateBase
    {
        private const double DASH_RELOAD_TIME = 0.2; // задержка перед включением дэша
        private Timer _dashReload;
        private bool _dashIsReloading;

        public override void _Ready()
        {
            base._Ready();
            _dashReload = GetNode<Timer>("DashReload");
            _dashReload.WaitTime = DASH_RELOAD_TIME;
            _dashReload.Timeout += DashReload_Timeout;
        }


        public override void Enter()
        {
            if (!_player.DashAvailable && !_dashIsReloading)
            {
                _dashIsReloading = true;
                _dashReload.Start();
            }
                
            _player.PlayAnim("PlayerFloorIdle");
        }


        public override void Exit()
        {
        }


        public override void Update(double delta)
        {
        }


        public override void PhysicsUpdate(double delta)
        {
            var inputDirectionX = Input.GetAxis("ui_left", "ui_right");
            var inputDirectionY = Input.GetAxis("ui_up", "ui_down");

            if (StateTransitonCheck(inputDirectionX, inputDirectionY))
                return;
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

            // air (fall)
            if (!(_player.IsOnFloor() || _player.IsOnWall()))
            {
                Args["AirStateParam"] = "";
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir");
                return true;
            }

            // dash
            if (Input.IsActionJustPressed("dash") && _player.DashAvailable)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDash");
                return true;
            }

            // run
            if (inputDirectionX != 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorRun");
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


        private void DashReload_Timeout()
        {
            _player.DashAvailable = true;
            _dashIsReloading = false;

            Tween tween = _player.ModulateTween;
            tween.TweenProperty(_player.Anim, "modulate", Color.Color8(255, 255, 255), DASH_RELOAD_TIME);
            

            //_player.Anim.Modulate = Color.Color8(255, 255, 255);
        }
    }
}
