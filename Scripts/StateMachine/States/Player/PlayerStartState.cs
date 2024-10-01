using Godot;
using Godot.Collections;

namespace PinkInk.Scripts.StateMachine.States.Player
{
    public partial class PlayerStartState : PlayerStateBase
    {
        private const float MAX_GRAVITY = 200.0f;       // максимально возможная гравитация


        public override void _Ready()
        {
            base._Ready();
        }


        public override void Enter()
        {
            Args.Add("AirStateParam", "");

            _player.Anim.Play("idle");
        }


        public override void Exit()
        {
        }


        public override void Update(double delta)
        {
        }


        public override void PhysicsUpdate(double delta)
        {
            // Применяем скорость
            var velocity = _player.Velocity;

            if (velocity.Y < MAX_GRAVITY)
                velocity += _player.GetGravity() * (float)delta;

            _player.Velocity = velocity;

            _player.MoveAndSlide();

            if (StateTransitonCheck())
                return;
        }


        private bool StateTransitonCheck()
        {
            // floor idle
            if (_player.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle");
                return true;
            }

            return false;
        }
    }
}
