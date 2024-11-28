using Godot;

namespace CoffeeCatProject.Players.Components.Scripts;

// This only handles the animation logic for weapons
public partial class WeaponAnimationComponent : Node
{
	public float SpriteDirection { get; set; }
	public bool WallSlide { get; set; }
	
	private bool _shooting;
	private bool _onCooldown;
	
	[Export] public Timer CooldownTimer { get; set; }
	[Export] private AnimatedSprite2D Sprite { get; set; }
	[Export] private ShootingComponent ShootingComponent  { get; set; }
	
	public override void _Ready()
	{
		// Set timer values
		CooldownTimer.SetOneShot(true);
		
		// Connect to timer signal
		CooldownTimer.Timeout += CooldownTimeout;
		
		// Connect to shooting component signal
		ShootingComponent.IsSHooting += IsShooting;
		
		Sprite.Play("idle");
	}
	
	private void CooldownTimeout()
	{
		_onCooldown = false;
	}

	private void IsShooting()
	{
		_shooting = true;
	}
	
	public override async void _Process(double delta)
	{
		// Flip weapon sprite based on player's direction 
		if (SpriteDirection < 0)
		{
			Sprite.FlipH = false;
		}
		
		if (SpriteDirection > 0)
		{
			Sprite.FlipH = true;
		}
		
		if (_shooting && !_onCooldown)
		{
			_shooting = false;
			_onCooldown = true;
			CooldownTimer.Start();
			Sprite.Play("shoot");
		}
		else if (WallSlide)
		{
			_shooting = false;
			Sprite.Play("wall_slide");
		}
		else
		{
			_shooting = false;
			
			if (Sprite.Animation == "shoot")
			{
				await ToSignal(Sprite, "animation_finished");
			}
			Sprite.Play("idle");
		}
	}
}
