using Boulder.Scripts.Models;
using Godot;

public partial class HealthComponent : Node2D
{
	[Export]
	public double MAX_HP { get; set; } = 1;
	[Export]
	public double REGENERATION { get; set; } = 0;

    public double CurrentHP { get; set; }


	public override void _Ready()
	{
        CurrentHP = MAX_HP;
	}


	public override void _Process(double delta)
	{
		if (CurrentHP <= MAX_HP)
            CurrentHP += REGENERATION * delta;
		else
			CurrentHP = MAX_HP;
    }
	

	public void DoDamage(Attack attack)
	{
		CurrentHP -= attack.Damage;

		if (CurrentHP < 0)
		{
			GetParent().QueueFree();
		}
    }
}
