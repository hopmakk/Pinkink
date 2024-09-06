using Godot;
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
        }


        public override void Update(double delta)
        {
        }

        
        public override void PhysicsUpdate(double delta)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

            if (direction.X * _lastDirection < 0)
                ChangeAnim();

            // запоминаем, чтобы сделать прыжок при падении
            if (Input.IsActionJustPressed("jump"))
                _todoJumpTimer.Start();

            var velocity = _parent.Velocity;

            velocity.X = direction.X * _parent.Speed;

            velocity += _parent.GetGravity() * (float)delta;

            _parent.Velocity = velocity;

            _parent.MoveAndSlide();

            StateTransitonCheck(direction);
        }


        private void ChangeAnim()
        {
            _parent.PlayAnim("PlayerAir");
            _lastDirection = _parent.Direction;
        }


        private void StateTransitonCheck(Vector2 direction)
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

            // floor idle
            if (_parent.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transitioned, this, "PlayerFloorIdle", default);
                return;
            }
        }
    }
}
