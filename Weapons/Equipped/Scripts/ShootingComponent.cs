using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;
public partial class ShootingComponent : Node
{
	// Variables
	private bool _onCooldown;
	
	[Export]
	public Timer ShotCooldownTimer { get; set; }
	
	[Signal]
	public delegate void ShootingStartEventHandler();

	[Signal]
	public delegate void ShootingEndEventHandler();
	
	public override void _Ready()
	{
		// Set timer values
		ShotCooldownTimer.SetOneShot(true);
		ShotCooldownTimer.SetWaitTime(0.9);
		
		// Connecting signals
		ShotCooldownTimer.Timeout += OnTimerTimeout;

	}

	private void ShootStart()
	{
		GD.Print("Start shooting");
		EmitSignal(SignalName.ShootingStart);
		ShotCooldownTimer.Start();
		_onCooldown = true;
	}

	private void ShootEnd()
	{
		GD.Print("Stop shooting");
		EmitSignal(SignalName.ShootingEnd);
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot") && !_onCooldown)
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
		_onCooldown = false;
	}
}
