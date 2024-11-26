using Godot;

namespace CoffeeCatProject.Player.Components.Scripts;

// This component only handles the shooting logic for weapons
//TODO: make shooting logic similar to re4, if button held keep shooting with cooldown between shots
public partial class ShootingComponent : Node
{
	// Variables
	public bool OnCooldown { get; set; }
	
	[Export]
	public Timer ShotCooldownTimer { get; set; }
	
	[Signal]
	public delegate void ShootingStartEventHandler();

	[Signal]
	public delegate void ShootingEndEventHandler();
	
	public override void _Ready()
	{
		// Connecting signals
		ShotCooldownTimer.Timeout += OnTimerTimeout;
	}

	private void Shooting()
	{
		EmitSignal(SignalName.ShootingStart);
		GD.Print("shooting comp start");
		ShotCooldownTimer.Start();
		OnCooldown = true;
	}

	private void StopShooting()
	{
		GD.Print("Shooting comp end");
		EmitSignal(SignalName.ShootingEnd);
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot") && !OnCooldown)
		{
			Shooting();
		}

		if (Input.IsActionJustReleased("shoot"))
		{
			StopShooting();
		}
	}
	
	// Connecting signals
	private void OnTimerTimeout()
	{
		OnCooldown = false;
	}
}
