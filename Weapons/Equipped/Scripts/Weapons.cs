using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;
public partial class Weapons : Node2D
{
	public override void _Ready()
	{
	}

	public void ShootWeapon(
		object bullet, 
		AnimatedSprite2D animation, 
		Timer cooldownTimer, 
		Marker2D muzzle,
		int bulletCount,
		float bulletAngleDegree
		)
	{
		//Shooting logic
		
		//Animations
		AnimateSpriteBasedOnActions();
		
	}

	public void AnimateSpriteBasedOnActions()
	{
		//Animations logic
	} 
	
	public override void _Process(double delta)
	{
	}
}
