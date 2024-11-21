using Godot;

namespace CoffeeCatProject.Player.Scripts;

public partial class WeaponShotgun : AnimatedSprite2D
{
	// Variables
	private float _direction;
	public float Direction
	{
		get => _direction;
		set => _direction = value;
	}
	
	public override void _Ready()
	{
		Play("idle");
		
		FlipSprite();
	}

	public override void _Process(double delta)
	{
		FlipSprite();
	}
	
	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (_direction < 0)
		{
			FlipH = false;
		}

		if (_direction > 0)
		{
			FlipH = true;
		}
	} 
}
