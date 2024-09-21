using Boulder.Scripts.Models;
using Godot;
using System;
using static Godot.TextServer;

public partial class HitboxComponent : Area2D
{
	private HealthComponent _healthComponent;
	private CharacterBody2D _parentNode;

	private const int KNOCKBACK_POWER_MODIFICATOR = 1000;

	public override void _Ready()
	{
        _healthComponent = GetNode<HealthComponent>("../HealthComponent");

		if (GetParent<Node2D>() is CharacterBody2D parent)
			_parentNode = parent;
    }


	public override void _Process(double delta)
	{
	}


    public void DoDamage(Attack attack)
    {
		_healthComponent.DoDamage(attack);

		if (_parentNode != null)
		{
            //_parentNode.ApplyCentralImpulse(attack.AttackPosition * (float)attack.KnockBackForce * KNOCKBACK_POWER_MODIFICATOR);
        }
    }
}
