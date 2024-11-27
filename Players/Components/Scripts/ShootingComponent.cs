using Godot;
using CoffeeCatProject.Players.WeaponManager.Scripts;

namespace CoffeeCatProject.Players.Components.Scripts;

// This component only handles the shooting logic for weapons
//TODO: make shooting logic similar to re4, if button held keep shooting with cooldown between shots
public partial class ShootingComponent : Node
{
	// Variables, data to get from player and weapons
	public float SpriteDirection { get; set; }
	public float BulletAngle { get; set; }
	public int BulletCount { get; set; }
	public Vector2 MuzzlePosition { get; set; }
	public PackedScene BulletScene { get; set; }

	private bool _onCooldown;
	
	[Export] public Timer CooldownTimer { get; set; }
	
	[Signal]
	public delegate void IsShootingEventHandler();

	[Signal]
	public delegate void NotShootingEventHandler();
	
	// Nodes
	private WeaponManagerScript _weaponManagerScriptNode;
	
	public override void _Ready()
	{
		// Get the nodes
		_weaponManagerScriptNode = GetParent().GetNode<WeaponManagerScript>("weapon_manager");
		
		// Set timer values
		CooldownTimer.SetOneShot(true);
		
		// Connect to timer signal
		CooldownTimer.Timeout += CooldownTimeout;
	}

	private void Shooting()
	{
		GD.Print("shooting component emits shooting signal and starts cooldown timer");
		EmitSignal(SignalName.IsShooting);
		_onCooldown = true;
		CooldownTimer.Start();
		
		SpawnBullets();
	}

	private void StopShooting()
	{
		GD.Print("Shooting comp end");
		EmitSignal(SignalName.NotShooting);
	}

	private void SpawnBullets()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		
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
		if (
			(Input.IsActionJustPressed("shoot") || Input.IsActionPressed("shoot")) && 
			!_weaponManagerScriptNode.WallSlide
			)
		{
			Shooting();
		}

		if (Input.IsActionJustReleased("shoot"))
		{
			StopShooting();
		}
	}

	private void CooldownTimeout()
	{
		GD.Print("cooldown timeout reached");
		_onCooldown = false;
	}
}
