using Godot;
using System;

public partial class State : Node2D
{
    protected CharacterBody2D _parent;

    [Signal]
    public delegate void TransitionedEventHandler(State state, string newStateName);


    public override void _Ready()
    {
        _parent = GetParent<StateMachine>().GetParent<CharacterBody2D>();
    }


    public virtual void Enter() { }


    public virtual void Exit() { }


    public virtual void Update(double delta) { }


    public virtual void PhysicsUpdate(double delta) { }
}
