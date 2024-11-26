using Godot;

namespace CoffeeCatProject.Player.Components.Scripts;

// This component only handles the shooting logic for weapons
//TODO: make shooting logic similar to re4, if button held keep shooting with cooldown between shots
[GlobalClass]
public partial class ShootingComponent : Node
{
	// Variables, data to get from player and weapons
	public bool OnCooldown { get; set; }
	public float SpriteDirection { get; set; }
	public float BulletAngle { get; set; }
	public int BulletCount { get; set; }
	public Vector2 MuzzlePosition { get; set; }
	
	[Signal]
	public delegate void ShootingStartEventHandler();

	[Signal]
	public delegate void ShootingEndEventHandler();
	
	public override void _Ready()
	{
		
	}

	private void Shooting()
	{
		GD.Print("shooting component emits shoot start signal");
		EmitSignal(SignalName.ShootingStart);
	}

	private void StopShooting()
	{
		GD.Print("Shooting comp end");
		EmitSignal(SignalName.ShootingEnd);
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			Shooting();
		}

		if (Input.IsActionJustReleased("shoot"))
		{
			StopShooting();
		}
	}
}
