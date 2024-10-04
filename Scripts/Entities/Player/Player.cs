using Godot;
using Godot.Collections;

public partial class Player : EntityBase
{
    private const float SPEED = 85.0f;
    private const float JUMP_VELOCITY = 275.0f;

    public bool DashAvailable { get; set; }
    private RichTextLabel _label;


    // Создан, чтобы не обращаться несколько раз к изменению одного и того же свойства
    #region объект твин анимации для skew
    public Tween SkewTween
    {
        get
        {
            if (_skewTween != null)
                _skewTween.Kill();  // завершить предыдущую анимацию
            _skewTween = CreateTween();
            return _skewTween;
        }
        set { _skewTween = value; }
    }
    private Tween _skewTween;
    #endregion

    #region объект твин анимации для Modulate
    public Tween ModulateTween
    {
        get
        {
            if (_modulateTween != null)
                _modulateTween.Kill();  // завершить предыдущую анимацию
            _modulateTween = CreateTween();
            return _modulateTween;
        }
        set { _modulateTween = value; }
    }
    private Tween _modulateTween;
    #endregion


    public override void _Ready()
    {
        base._Ready();

        Speed = SPEED;
        JumpVelocity = JUMP_VELOCITY;
        Direction = 1;
        DashAvailable = true;
        _label = GetNode<RichTextLabel>("../../UI/Control/TestLabel");
        AnimNamesWithDirection = new Dictionary<string, string[]>()
        {
            { "PlayerFloorIdle", new[]{ "floor_idle_left", "floor_idle_right" } },
            { "PlayerFloorRun", new[]{ "floor_run_left", "floor_run_right" } },
            { "PlayerWallIdle", new[]{ "wall_idle_left", "wall_idle_right" } },
            { "PlayerWallRun", new[]{ "wall_run_left", "wall_run_right" } },
            { "PlayerAir", new[]{ "air_left", "air_right" } },
            { "PlayerAirJump", new[]{ "jump_left", "jump_right" } },
        };
    }


    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);

        _label.Text = $"X: {Velocity.X} \nY: {Velocity.Y}";
        // двигаемся
        //MoveAndSlide();

        //Movement(delta);
    }


    private void Movement(double delta)
    {
        Vector2 velocity = Velocity;
        var isOnFloor = IsOnFloor();
        var isOnCeiling = IsOnCeiling();
        var isOnWall = IsOnWall();

        // находим вектор 
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        // если мы на полу или на потолке
        if (isOnFloor || isOnCeiling)
        {
            velocity.Y = 0; // сбрасываем вертикальную скорость

            // когда мы пытаемся упасть с потолка
            if (direction.Y > 0)
            {
                velocity.Y = 1;
            }
        }

        // когда мы на стене
        if (isOnWall)
        {
            // прилипаем к ней
            velocity.X = 10 * Mathf.Sign(velocity.X);
        }

        // X
        if (direction.X != 0)
        {
            velocity.X = direction.X * Speed;
        }
        else
        {
            if (isOnFloor || isOnCeiling)
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Y
        if (isOnWall)
        {
            if (direction.Y != 0)
            {
                velocity.Y = direction.Y * Speed;
            }
            else
            {
                velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
            }
        }

        // прыжок
            if (Input.IsActionJustPressed("ui_accept"))
            {
                velocity.Y = JumpVelocity;
            }

        // если не на поверхностях - действует гравитация
        if (!(isOnFloor || isOnCeiling || isOnWall))
        {
            velocity += GetGravity() * (float)delta;
        }

        _label.Text = $"X: {velocity.X} \nY: {velocity.Y}";
        Velocity = velocity;
    }
}
