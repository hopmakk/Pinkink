using Godot;

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
            ChangeAnim();
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
            direction.Y = 0;
            direction = direction.Round();
            
            // надо ли менять состояние
            StateTransitonCheck(direction);

            if (direction.X * _lastDirection < 0)
                ChangeAnim();

            var velocity = _parent.Velocity;

            velocity.X = direction.X * _parent.Speed;

            _parent.Velocity = velocity;

            _parent.MoveAndSlide();
        }


        private void ChangeAnim()
        {
            _parent.PlayAnim("PlayerFloorRun", 1.25f);
            _lastDirection = _parent.Direction;
        }


        private void StateTransitonCheck(Vector2 direction)
        {
            // idle
            if (direction.X == 0)
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
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
