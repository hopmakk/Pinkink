using Boulder.Scripts.Models;
using Godot;
using System.Collections.Generic;

public partial class HurtboxComponent : Area2D
{
	[Export]
	public double DAMAGE { get; set; } = 1;
	[Export]
	public double KNOCKBACK_FORCE { get; set; } = 1;

	[Signal]
	public delegate void DoDamageSignalEventHandler();


	public override void _Ready()
	{
        AreaEntered += HurtboxComponentScript_AreaEntered;
    }


    public override void _Process(double delta)
	{
	}


    private void HurtboxComponentScript_AreaEntered(Area2D area)
    {
        if (area is HitboxComponent hitboxComponent)
        {
            DoDamage(hitboxComponent);
        }
    }


    public void DoDamage(HitboxComponent hitboxComponent)
	{
		EmitSignal(SignalName.DoDamageSignal);

		var attack = new Attack(
            DAMAGE,
            KNOCKBACK_FORCE,
			(hitboxComponent.GlobalPosition - this.GlobalPosition).Normalized()
			);

		hitboxComponent.DoDamage(attack);
	}
}
