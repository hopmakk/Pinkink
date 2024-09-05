using Godot;
using System.Reflection.Emit;

public partial class Player : CharacterBody2D
{
    public const float SPEED = 100.0f;
    public const float JUMP_VELOCITY = -200.0f;

    private int playerDirection = 0;

    private RichTextLabel _label;
    private AnimatedSprite2D _anim;


    public override void _Ready()
    {
        _label = GetNode<RichTextLabel>("../UI/Control/TestLabel");
        _anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }


    public override void _PhysicsProcess(double delta)
	{
        // двигаемся
        MoveAndSlide();

        Movement(delta);
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
            velocity.X = direction.X * SPEED;
        }
        else
        {
            if (isOnFloor || isOnCeiling)
                velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);
        }

        // Y
        if (isOnWall)
        {
            if (direction.Y != 0)
            {
                velocity.Y = direction.Y * SPEED;
            }
            else
            {
                velocity.Y = Mathf.MoveToward(Velocity.Y, 0, SPEED);
            }
        }

        // прыжок
        if (Input.IsActionJustPressed("ui_accept"))
        {
            velocity.Y = JUMP_VELOCITY;
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
