using Godot;
using Godot.Collections;

public partial class State : Node2D
{ 
    protected EntityBase _parent;
    public Dictionary<string, string> Args;

    [Signal]
    public delegate void TransitionedEventHandler(State state, string newStateName);


    public override void _Ready()
    {
        _parent = GetParent<StateMachine>().GetParent<EntityBase>();
        Args = new Dictionary<string, string>();
    }


    public virtual void Enter() { }


    public virtual void Exit() { }


    public virtual void Update(double delta) { }


    public virtual void PhysicsUpdate(double delta) { }
}