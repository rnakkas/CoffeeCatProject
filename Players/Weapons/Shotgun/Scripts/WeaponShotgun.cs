using CoffeeCatProject.Players.PlayerCharacters.Scripts;
using Godot;

namespace CoffeeCatProject.Players.Weapons.Shotgun.Scripts;

public partial class WeaponShotgun : Node2D
{
	// Constants
	private const float BulletAngle = 6.5f;
	private const float CoolDownTime = 1.2f;
	private const int BulletCount = 6;

	// Nodes
	private AnimatedSprite2D _sprite;
	private Timer _cooldownTimer;
	private Marker2D _muzzle;
	private Player _player;

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
		_player = GetParentOrNull<Player>();
		
		// Play idle animation
		FlipSprite();
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
			bulletInstance.Direction = _player.SpriteDirection;

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

	private void FlipSprite()
	{
		if (_player != null)
		{
			if (_player.SpriteDirection < 0)
			{
				_sprite.FlipH = false;
				_muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
			}
			else if (_player.SpriteDirection > 0)
			{
				_sprite.FlipH = true;
				_muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
			}
		}
	}

	public override async void _Process(double delta)
	{
		FlipSprite();
		
		if (_player == null) 
			return;
		
		if ((Input.IsActionJustPressed("shoot") || Input.IsActionPressed("shoot")) &&
		    !_player.WallSlide &&
		    !_onCooldown)
		{
			_onCooldown = true;
			ShootBullets();
			_sprite.Play("shoot");
		}
		else if (_player.WallSlide)
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
