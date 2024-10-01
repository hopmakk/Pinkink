using Godot;
using System;

public partial class Stalactite : CharacterBody2D
{
	private GpuParticles2D _particles;
	private RayCast2D _rayCast;
	private bool _fall;
	private const double MAX_GRAVITY = 300;

	public override void _Ready()
	{
		_particles = GetNode<GpuParticles2D>("GPUParticles2D");
        _rayCast = GetNode<RayCast2D>("RayCast2D");
		_fall = false;
    }


	public override void _Process(double delta)
	{
		if (_rayCast.IsColliding())
		{
			var collider = _rayCast.GetCollider();

            // если снизу прошло существо - падаем
            if (collider is CharacterBody2D)
			{
				_fall = true;
				_particles.OneShot = true;
                _rayCast.TargetPosition = Vector2.Zero;
            }
            // если же снизу пол, назначаем target на величину до него 
			else if (_rayCast.GetCollisionMaskValue(1))
			{
                var groundCollidePoint = _rayCast.GetCollisionPoint();
                groundCollidePoint.Y--;
                _rayCast.TargetPosition = groundCollidePoint - GlobalPosition;
				_rayCast.ForceRaycastUpdate();
            }
        }

        // падаем
        if (_fall && !IsOnFloor())
		{
            var velocity = Velocity;

            if (velocity.Y < MAX_GRAVITY)
                velocity += GetGravity() * (float)delta;

            Velocity = velocity;
            MoveAndSlide();

			//if (_rayCast.GetCollisionMaskValue(1))
   //             _fall = false;
        }
	}
}
