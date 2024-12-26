using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles detecting collisions with bullets, projectiles and attacks for enemies and player entities.
// This includes detecting attack type and transferring the damage of the attack to the health component and detecting
//	the direction of the attacks/projectiles

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
	[Export] private HealthComponent _healthComponent;
	
	public override void _Ready()
	{
		AreaEntered += HitByMeleeAttack;
		AreaEntered += HitByProjectileAttack;
	}
	
	private void HitByMeleeAttack(Node2D area)
	{
		switch (area.Name)
		{
			case "attack_hitbox":
				GD.Print("player has been attacked by melee: SetState(State.Hurt)");
				// SetState(State.Hurt); Commented out for now, reenable once hurt state has been figured out.
				break;
            
			case "damage_player_area":
				GD.Print("player has been attacked by fatty: SetState(State.Hurt)");
				break;
		}
	}

	private void HitByProjectileAttack(Node2D area)
	{
		switch (area.Name)
		{
			case "fatty_spit":
				GD.Print("player has been hit by fatty's spit projectile: SetState(State.Hurt)");
				_healthComponent.TakeDamage(2);
				break; 
			
			case "bullet_shotgun":
				GD.Print("enemy hit by shotgun bullets");
				break;
		}
	}
}
