using Godot;

public partial class PlayerControllerCmpnt : Node2D
{
    public const float SPEED = 100.0f;
    public const float JUMP_VELOCITY = -300.0f;

    private Player _player;  // игрок


    public override void _Ready()
	{
        _player = GetParent<Node2D>().GetParent<Player>();
    }


    public void в(double delta)
	{
        Vector2 velocity = _player.Velocity;

        if (Input.IsActionJustPressed("ui_accept"))
        {
            velocity.Y = JUMP_VELOCITY;
        }

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (direction.X != 0)
            velocity.X = direction.X * SPEED;
        else
            velocity.X = Mathf.MoveToward(_player.Velocity.X, 0, SPEED);

        if (direction.Y != 0)
            velocity.Y = direction.Y * SPEED;
        else
            velocity.Y = Mathf.MoveToward(_player.Velocity.Y, 0, SPEED);

        _player.Velocity = velocity;
    }
}
