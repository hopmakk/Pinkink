using Godot;

public partial class DashSkill : Node2D
{
    private const int JUMP_POWER = 500;                 // сила прыжка
    private const int JUMP_DIRECTION_RAY_LENGTH = 1000; // длина луча для выбора прыжка
    private const int MIN_RAY_LENGTH_TO_JUMP = 20;    // минимальная длина луча, чтобы прыгнуть 

    private Player _player;                 // игрок
    private RayCast2D _jumpDirectionRay;    // луч для определения столкновения с объектами при выборе стороны прыжка
    private bool _isSwiping;                // происходит ли свайп
    private Vector2 _jumpPoint;             // точка, в которую надо попасть



    public override void _Ready()
    {
        _player = GetParent<Node2D>().GetParent<Player>();
        _jumpDirectionRay = GetNode<RayCast2D>("RayCast2D");
    }


    public override void _Process(double delta)
    {
        var jumpDirection = Vector2.Zero;   // вектор направления от игрока до курсора

        // запоминаем точку из которой делается свайп
        if (Input.IsActionJustPressed("press") && !_isSwiping)
        {
            _isSwiping = true;
        }

        // в процессе свайпа
        if (_isSwiping)
        {
            jumpDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
            var targetRayVector = jumpDirection * JUMP_DIRECTION_RAY_LENGTH;
            _jumpDirectionRay.TargetPosition = targetRayVector;
            _jumpDirectionRay.ForceRaycastUpdate();

            if (_jumpDirectionRay.IsColliding())
                _jumpPoint = _jumpDirectionRay.GetCollisionPoint() - GlobalPosition;
            else
                _jumpPoint = targetRayVector;

            QueueRedraw();
        }

        // при отжатии
        if (Input.IsActionJustReleased("press") && _isSwiping)
        {
            if (_jumpPoint.Length() > MIN_RAY_LENGTH_TO_JUMP)
            {
                DoJump(jumpDirection);
                //DoJump(_jumpPoint);
            }

            _isSwiping = false;
            _jumpDirectionRay.TargetPosition = Vector2.Zero;
            _jumpPoint = Vector2.Zero;

            QueueRedraw();
        }
    }


    // расчитать и произвести прыжок по вектору свайпа
    public void DoJump(Vector2 direction)
    {
        var jumpVector = direction * JUMP_POWER;
        _player.Velocity = jumpVector;

        //_player.Translate(jumpPoint);
    }


    // отрисовка
    public override void _Draw()
    {
        if (_jumpPoint != Vector2.Zero)
            DrawJumpTrackLine();
    }


    // отрисовать линию прыжка
    private void DrawJumpTrackLine()
    {
        DrawLine(Position, _jumpPoint, Color.Color8(0, 200, 0), 1);
    }
}
