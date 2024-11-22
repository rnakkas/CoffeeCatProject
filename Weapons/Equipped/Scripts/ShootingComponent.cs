using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;

// This component only handles the shooting logic for weapons
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
		// // Set timer values
		// ShotCooldownTimer.SetOneShot(true);
		// ShotCooldownTimer.SetWaitTime(0.9);
		
		// Connecting signals
		ShotCooldownTimer.Timeout += OnTimerTimeout;

	}

	private void ShootStart()
	{
		EmitSignal(SignalName.ShootingStart);
		ShotCooldownTimer.Start();
		OnCooldown = true;
	}

	private void ShootEnd()
	{
		EmitSignal(SignalName.ShootingEnd);
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot") && !OnCooldown)
		{
			ShootStart();
		}

		if (Input.IsActionJustReleased("shoot"))
		{
			ShootEnd();
		}
	}
	
	// Connecting signals
	private void OnTimerTimeout()
	{
		OnCooldown = false;
	}
}
