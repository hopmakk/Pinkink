using Godot;
using Godot.Collections;

public partial class EntityBase : CharacterBody2D
{
    public float Speed { get; set; }            // скорость персонажа
    public float JumpVelocity { get; set; }     // скорость прыжка
    public int Direction { get; set; }          // текущее направление, куда смотрит персонаж
    public AnimatedSprite2D Anim { get; set; }  // AnimatedSprite2D
    public Dictionary<string, string[]> AnimNamesWithDirection { get; set; }    // названия анимаций и их интерпритация для разных направлений

    // Запрос и прерывание новой Tween анимации
    public Tween AnimSpriteTween
    {
        get
        {
            if (_animSpriteTween != null)
                _animSpriteTween.Kill();  // завершить предыдущую анимацию
            _animSpriteTween = CreateTween();
            return _animSpriteTween;
        }
        set { _animSpriteTween = value; }
    }
    private Tween _animSpriteTween;


    public override void _Ready()
    {
        Anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        AnimNamesWithDirection = new Dictionary<string, string[]>();

        var stateMachine = GetNodeOrNull<StateMachine>("StateMachine");
        if (stateMachine != null)
            stateMachine.Launch();
    }


    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (direction.X != 0)
            Direction = Mathf.Sign(direction.X);
    }

    
    // проигрывать анимацию с выбором направления
    public bool PlayAnim(string name, float speed = 1, bool fromEnd = false)
    {
        if (!AnimNamesWithDirection.ContainsKey(name))
            return false;

        var selectedAnimNames = AnimNamesWithDirection[name];

        if (Direction > 0)
            Anim.Play(selectedAnimNames[1], speed, fromEnd);
        else if (Direction < 0)
            Anim.Play(selectedAnimNames[0], speed, fromEnd);

        return true;
    }
}
