using Godot;

public partial class State : Node2D
{
    protected EntityBase _parent;

    [Signal]
    public delegate void TransitionedEventHandler(State state, string newStateName, Variant arg);


    public override void _Ready()
    {
        _parent = GetParent<StateMachine>().GetParent<EntityBase>();
    }


    public virtual void Enter(Variant arg) { }


    public virtual void Exit() { }


    public virtual void Update(double delta) { }


    public virtual void PhysicsUpdate(double delta) { }
}
