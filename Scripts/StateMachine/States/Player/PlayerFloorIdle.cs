using Godot;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    internal partial class PlayerFloorIdle : State
    {
        public override void _Ready()
        {
            base._Ready();
        }


        public override void Enter(Variant arg)
        {
            _parent.PlayAnim("PlayerFloorIdle");
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
            // air (jump)
            if (Input.IsActionJustPressed("jump"))
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return true;
            }

            // air (fall)
            if (!(_parent.IsOnFloor() || _parent.IsOnWall()))
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
                return true;
            }

            // dash
            if (Input.IsActionJustPressed("dash"))
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerDash", default);
                return true;
            }

            // run
            if (inputDirectionX != 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorRun", default);
                return true;
            }

            // wall idle
            if (_parent.IsOnWall() && inputDirectionY < 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerWallIdle", default);
                return true;
            }

            return false;
        }

    }
}
