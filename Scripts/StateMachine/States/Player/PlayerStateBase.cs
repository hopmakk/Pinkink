public partial class PlayerStateBase : State
{
    public Player _player;

    public override void _Ready()
    {
        base._Ready();
        _player = _parent as Player;
    }
}