using CoffeeCatProject.Players.Components.Scripts;
using CoffeeCatProject.Players.PlayerCharacters.Scripts;
using CoffeeCatProject.Players.WeaponManager.Scripts;
using Godot;

namespace CoffeeCatProject.Players.Weapons.Shotgun.Scripts;

public partial class WeaponShotgun : Player
{
	// Constants
	private const float BulletAngle = 6.5f;
	private const float CoolDownTime = 1.2f;
	private const int BulletCount = 6;

	// Nodes
	private AnimatedSprite2D _sprite;
	private Timer _cooldownTimer;
	private Marker2D _muzzle;

	// Packed scene: bullets
	private readonly PackedScene _shotgunShells =
		ResourceLoader.Load<PackedScene>("res://Players/Weapons/Shotgun/Scenes/bullet_shotgun.tscn");

	// Vars
	private float _spriteDirection;
	private bool _wallSlide;
	private Vector2 _muzzlePosition;
	private bool _onCooldown;

	public override void _Ready()
	{
		// Get the nodes   
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_cooldownTimer = GetNode<Timer>("shotCoolDownTimer");
		_muzzle = GetNode<Marker2D>("muzzle");

		// Play idle animation
		_sprite.Play("idle");

		// Set timer values
		_cooldownTimer.SetOneShot(true);
		_cooldownTimer.WaitTime = CoolDownTime;

		// Connect to timer signal
		_cooldownTimer.Timeout += CooldownTimeout;

		// Set muzzle position
		_muzzlePosition = _muzzle.Position;
	}

	private void ShootBullets()
	{
		// Spawn bullets and start cooldown timer
		var rng = new RandomNumberGenerator();

		for (int i = 0; i < BulletCount; i++)
		{
			var bulletInstance = (BulletShotgun)_shotgunShells.Instantiate();
			
			// Set bullet's direction
			bulletInstance.Direction = _spriteDirection;

			// Set bullets rotations
			bulletInstance.RotationDegrees = rng.RandfRange(-BulletAngle, BulletAngle);

			// Set bullets spawn location
			bulletInstance.GlobalPosition = _muzzle.GlobalPosition;

			// Add bullets to root
			GetTree().Root.AddChild(bulletInstance);
		}

		_cooldownTimer.Start();
	}

	private void CooldownTimeout()
	{
		_onCooldown = false;
	}

public override void _Process(double delta)
	{
		if (Velocity.X < 0)
		{
			_spriteDirection = -1.0f;
			_sprite.FlipH = false;
		}

		if (Velocity.X > 0)
		{
			_spriteDirection = 1.0f;
			_sprite.FlipH = true;
		}
	}
}
