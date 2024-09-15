using Godot;

public partial class PlayerDash : State
{
    private const float DASH_SPEED = 500.0f;
    private const float DASH_LENGTH = 45.0f;

    private float _dashTime;

    public override void _Ready()
    {
        base._Ready();
    }


    public override void Enter(Variant arg)
    {
        _dashTime = DASH_LENGTH / DASH_SPEED;

        var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        // текущее направление персонажа
        if (direction == Vector2.Zero)
            direction.X = _parent.Direction;

        // Применяем скорость
        _parent.Velocity = direction * DASH_SPEED;
    }


    public override void Exit()
    {
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
        _dashTime -= (float)delta;

        _parent.MoveAndSlide();

        if (StateTransitonCheck())
            return;
    }


    private bool StateTransitonCheck()
    {
        // air
        if (_dashTime <= 0)
        {
            EmitSignal(State.SignalName.Transitioned, this, "PlayerAir", "dash");
            return true;
        }

        return false;
    }
}
