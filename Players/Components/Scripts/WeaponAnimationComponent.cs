using Godot;

namespace CoffeeCatProject.Players.Components.Scripts;

// This only handles the animation logic for weapons
public partial class WeaponAnimationComponent : Node
{
	public bool WallSlide { get; set; }
	public bool Shooting { get; set; }

	private bool _onCooldown;
	
	[Export] public Timer CooldownTimer { get; set; }
	
	// Animation node
	private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		// Get animation node from weapon
		_sprite = GetParent().GetNode<AnimatedSprite2D>("sprite");
		
		// Set timer values
		CooldownTimer.SetOneShot(true);
		
		// Connect to timer signal
		CooldownTimer.Timeout += CooldownTimeout;
		
		_sprite.Play("idle");
	}
	
	private void CooldownTimeout()
	{
		_onCooldown = false;
	}
	
	public override async void _Process(double delta)
	{
		if (Shooting && !_onCooldown)
		{
			_onCooldown = true;
			CooldownTimer.Start();
			_sprite.Play("shoot");
		}
		else if (WallSlide)
		{
			_sprite.Play("wall_slide");
		}
		else
		{
			if (_sprite.Animation == "shoot")
			{
				await ToSignal(_sprite, "animation_finished");
			}
			_sprite.Play("idle");
		}
	}
}
