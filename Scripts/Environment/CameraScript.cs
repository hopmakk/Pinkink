using Godot;
using System;

public partial class CameraScript : Camera2D
{
    [Export]
    public Node2D Player {  get; set; }

    [Export]
    public float SmoothSpeed { get; set; } = 10.0f; // Скорость сглаживания


    private float _currentZoom = 5.0f;
    private const float ZOOM_SPEED = 0.3f;
    private const float MAX_ZOOM = 7.0f;
    private const float MIN_ZOOM = 4.0f;


    public override void _Ready()
    {
        Zoom = new Vector2(_currentZoom, _currentZoom);
    }


    public override void _Process(double delta)
    {
        if (Player != null)
        {
            // Получаем текущую позицию камеры и позицию игрока
            Vector2 currentPosition = Position;
            Vector2 targetPosition = Player.Position;

            // Интерполируем позицию камеры для плавного движения
            //Vector2 newPosition = currentPosition.Lerp(targetPosition, SmoothSpeed * (float)delta);
            Vector2 newPosition = targetPosition;

            // Обновляем позицию камеры
            Position = newPosition;
        }
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton eventButton))
            return;

        if (eventButton.ButtonIndex == MouseButton.WheelUp)
        {
            if (_currentZoom < MAX_ZOOM)
            {
                _currentZoom += ZOOM_SPEED;
                Zoom = new Vector2(_currentZoom, _currentZoom);
            }
                
        }
        else if (eventButton.ButtonIndex == MouseButton.WheelDown)
        {
            if (_currentZoom > MIN_ZOOM)
            {
                _currentZoom -= ZOOM_SPEED;
                Zoom = new Vector2(_currentZoom, _currentZoom);
            }
                
        }
    }

}
