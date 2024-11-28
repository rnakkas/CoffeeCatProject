using Godot;

namespace CoffeeCatProject.Players.Components.Scripts;

// This component only handles the shooting logic for weapons
public partial class ShootingComponent : Node
{
	// Variables, data to get from player and weapons
	// public float SpriteDirection { get; set; }
	public float BulletAngle { get; set; }
	public int BulletCount { get; set; }
	public Vector2 MuzzlePosition { get; set; }
	public PackedScene BulletScene { get; set; }
	public bool WallSlide { get; set; }
	public bool Shooting { get; set; }

	private bool _onCooldown;
	
	[Export] public Timer CooldownTimer { get; set; }
	
	public override void _Ready()
	{
		// Set timer values
		CooldownTimer.SetOneShot(true);
		
		// Connect to timer signal
		CooldownTimer.Timeout += CooldownTimeout;
	}

	private void ShootBullets()
	{
		// Emit signal to get all the relevant data, spawn bullets and start cooldown timer
		SpawnBullets();
		CooldownTimer.Start();
		
		GD.Print("shooting component shoots bullets");
	}

	private void SpawnBullets()
	{
		var rng = new RandomNumberGenerator();
		
		// Instantiate the bullet scenes
		for (int i = 0; i < BulletCount; i++)
		{
			var bulletInstance = BulletScene.Instantiate<Area2D>();
			
			// Set bullets rotations
			bulletInstance.RotationDegrees = rng.RandfRange(-BulletAngle, BulletAngle);
			
			// Set bullets spawn location
			bulletInstance.GlobalPosition = MuzzlePosition;
			
			// Add bullets to root
			GetTree().Root.AddChild(bulletInstance);
		}
		
	}
	
	public override void _Process(double delta)
	{
		if (Shooting && !_onCooldown)
		{
			_onCooldown = true;
			ShootBullets();
		}
	}

	private void CooldownTimeout()
	{
		GD.Print("cooldown timeout reached");
		_onCooldown = false;
	}
}
