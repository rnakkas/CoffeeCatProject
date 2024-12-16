using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

// Enemy is stationary, shoots projectiles in a straight line towards player's position
//TODO:
// -- enemy projectile logic to only shoot one projectile at a time, see the shotgun behaviour
// -- only tracking player and shoot at player when within a certain range, use an area2d to detect when player
//		enters this range.
// -- Then use the GlobalPosition.DistanceTo and AngleTo methods to determine direction and angle for shooting
// -- shooting projectile at player location, projectiles can move through platforms but not through walls
// -- dying when health reaches 0
// -- hurt player if it gets too close, for example if trying to wall jump and this enemy is on the wall
public partial class RangedEnemy : CharacterBody2D
{
	// Consts
	private const float AttackDelayTime = 0.8f;
	private const float AttackCooldownTime = 1.0f;
	
	// Vars
	private int _health = 50;
	private Vector2 _playerGlobalPosition, _mouthPosition;
	private float _direction;
	
	// Statuses
	private bool _playerInRange;
	private bool _attacking;
	private bool _hurt;
	
	// Nodes
	private AnimatedSprite2D _sprite;
	private Area2D _enemyHurtbox, _playerDetectionArea;
	private Timer _attackDelayTimer, _attackCooldownTimer;
	private Marker2D _mouth;
	
	// Packed scene: projectiles
	private readonly PackedScene _fattySpit =
		ResourceLoader.Load<PackedScene>("res://Enemies/Scenes/fatty_spit.tscn");
	
	public override void _Ready()
	{
		// Get the nodes
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_enemyHurtbox = GetNode<Area2D>("enemy_hurtbox");
		_attackDelayTimer = GetNode<Timer>("attack_delay_timer");
		_attackCooldownTimer = GetNode<Timer>("attack_cooldown_timer");
		_mouth = GetNode<Marker2D>("mouth");
		_playerDetectionArea = GetNode<Area2D>("player_detection_area");
		
		// Animation (using idle for now)
		_sprite.Play("idle");
		
		// Set mouth position
		_mouthPosition = _mouth.Position;
		
		// Area2D signal connections
		_enemyHurtbox.AreaEntered += HitByBullets;
		_enemyHurtbox.AreaExited += BulletsDestroyed;
		_playerDetectionArea.AreaEntered += PlayerEnteredDetectionArea;
		_playerDetectionArea.AreaExited += PlayerExitedDetectionArea;
		
		// Set timer values
		_attackDelayTimer.OneShot = true;
		_attackDelayTimer.WaitTime = AttackDelayTime;
		_attackDelayTimer.Timeout += AttackDelayTimerTimedOut;
		
		_attackCooldownTimer.OneShot = true;
		_attackCooldownTimer.WaitTime = AttackCooldownTime;
		_attackCooldownTimer.Timeout += AttackCooldownTimerTimeout;
	}

	public override void _Process(double delta)
	{
		// Die when health reaches 0
		if (_health <= 0)
		{
			//TODO: Play death animation
			GD.Print("enemy died, \r\n -> await animation finished before QueueFree");
			QueueFree();
		}
		
		// Detecting and attacking the player
		if (!_playerInRange) 
			return;
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
		SetDirectionToTarget(_playerGlobalPosition);

		if (!_attacking)
			return;
		SpawnProjectile();
	}
	
	// Timers
	private void AttackDelayTimerTimedOut()
	{
		GD.Print("enemy attacking");
		_attacking = true;
		
		// When attacking, start the attack cooldown
		_attackCooldownTimer.Start();
	}
	
	private void AttackCooldownTimerTimeout()
	{
		GD.Print("attack cooldown");
		_attacking = false;
		
		// When attack finished, wait for attack delay to start attacking again
		_attackDelayTimer.Start();
	}
	
	// Setting the directions
	private void SetDirectionToTarget(Vector2 target) // Setting the direction based on player's position 
	{
		// Get x location and translate that to self direction float
		if (GlobalPosition.DirectionTo(target).X < 0)
		{
			_direction = -1.0f;
		}
		else if (GlobalPosition.DirectionTo(target).X > 0)
		{
			_direction = 1.0f;
		}
	}
	
	// Getting hit by player's bullets
	private void HitByBullets(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_health -= 5;
			_hurt = true;
		}
	}
	
	private void BulletsDestroyed(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_hurt = false;
		}
	}

	
	// Detecting the player when they enter the detection range
	private void PlayerEnteredDetectionArea(Node2D area)
	{
		if (area.Name != "player_area") 
			return;

		_playerInRange = true;
		
		_attackDelayTimer.Start();
	}

	private void PlayerExitedDetectionArea(Node2D area)
	{
		if (area.Name != "player_area") 
			return;

		_playerInRange = false;
		
		_attackDelayTimer.Stop();
		_attackCooldownTimer.Stop();
	}
	
	// Spawning the projectiles to shoot at player
	private void SpawnProjectile()
	{
		var projectileInstance = (FattySpit)_fattySpit.Instantiate();
		projectileInstance.Direction = _direction;
		projectileInstance.GlobalPosition = _mouth.GlobalPosition;
		GetTree().Root.AddChild(projectileInstance);
	}
}
