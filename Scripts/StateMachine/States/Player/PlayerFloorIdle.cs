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
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

            StateTransitonCheck(direction);
        }


        private void StateTransitonCheck(Vector2 direction)
        {
            // run
            if (direction.X != 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorRun", default);
                return;
            }

            // air (fall)
            if (!(_parent.IsOnFloor() || _parent.IsOnWall() || _parent.IsOnCeiling()))
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", default);
                return;
            }

            // air (jump)
            if (Input.IsActionJustPressed("jump"))
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "jump");
                return;
            }
        }

    }
}
