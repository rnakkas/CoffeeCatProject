using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;
public partial class ShootingComponent : Node
{
	// Node
	private Timer _shotCooldownTimer;
	
	// Variables
	private bool _onCooldown;
	
	[Signal]
	public delegate void ShootingStartEventHandler();

	[Signal]
	public delegate void ShootingEndEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_shotCooldownTimer = GetNode<Timer>("shotCoolDownTimer");
		
		// Set timer values
		_shotCooldownTimer.SetOneShot(true);
		_shotCooldownTimer.SetWaitTime(0.9);
		
		// Connecting signals
		_shotCooldownTimer.Timeout += OnTimerTimeout;

	}

	public void ShootStart()
	{
		GD.Print("Start shooting");
		EmitSignal(SignalName.ShootingStart);
		_shotCooldownTimer.Start();
		_onCooldown = true;
	}

	public void ShootEnd()
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
