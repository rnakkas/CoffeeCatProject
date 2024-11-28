using Godot;

namespace CoffeeCatProject.Players.Components.Scripts;

// This component only handles the shooting logic for weapons
public partial class ShootingComponent : Node
{
	// Variables, data to get from player and weapons
	public float SpriteDirection { get; set; }
	public float BulletAngle { get; set; }
	public int BulletCount { get; set; }
	public PackedScene BulletScene { get; set; }
	public bool WallSlide { get; set; }

	private bool _onCooldown;
	private Vector2 _muzzlePosition;
	
	[Export] public Timer CooldownTimer { get; set; }
	[Export] private Marker2D Muzzle  { get; set; }
	
	[Signal] public delegate void IsSHootingEventHandler();
	
	public override void _Ready()
	{
		// Set timer values
		CooldownTimer.SetOneShot(true);
		
		// Connect to timer signal
		CooldownTimer.Timeout += CooldownTimeout;
		
		// Set muzzle position
		_muzzlePosition = Muzzle.Position;
	}

	private void ShootBullets()
	{
		// Spawn bullets and start cooldown timer
		var rng = new RandomNumberGenerator();
		for (int i = 0; i < BulletCount; i++)
		{
			var bulletInstance = BulletScene.Instantiate<Area2D>();
			
			// Set bullets rotations
			bulletInstance.RotationDegrees = rng.RandfRange(-BulletAngle, BulletAngle);
			
			// Set bullets spawn location
			bulletInstance.GlobalPosition = Muzzle.GlobalPosition;
			
			// Add bullets to root
			GetTree().Root.AddChild(bulletInstance);
		}
		
		CooldownTimer.Start();
	}
	
	public override void _Process(double delta)
	{
		// Flip muzzle position based on player's direction 
		if (SpriteDirection < 0)
		{
			Muzzle.Position = new Vector2(Muzzle.Position.X, Muzzle.Position.Y);
		}
		
		if (SpriteDirection > 0)
		{
			Muzzle.Position = new Vector2(-Muzzle.Position.X, Muzzle.Position.Y);
		}
		
		// Shooting
		if ((!Input.IsActionJustPressed("shoot") && !Input.IsActionPressed("shoot")) ||
		    WallSlide ||
		    _onCooldown)
		{
			return;
		}

		EmitSignal(SignalName.IsSHooting);
		_onCooldown = true;
		ShootBullets();
	}

	private void CooldownTimeout()
	{
		_onCooldown = false;
	}
}
