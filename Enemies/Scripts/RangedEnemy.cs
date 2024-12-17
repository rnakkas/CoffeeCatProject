using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

// Enemy is stationary, shoots projectiles in a straight line towards player's position
//TODO:
// -- projectiles can move through platforms but not through walls
// -- hurt and knock back player if it gets too close, for example if trying to wall jump and this enemy is on the wall,
//		the player will get knocked back and not be able to wall jump
public partial class RangedEnemy : CharacterBody2D
{
	// Consts
	private const float AttackDelayTime = 0.8f;
	private const float AttackCooldownTime = 1.0f;
	private const float DeathProjectileAngle = 45.0f;
	private const int DeathProjectileCount = 10;
	
	// Vars
	private int _health = 50;
	private Vector2 _playerHeadTargetGlobalPosition, _mouthPosition, _deathExplosionPointPosition;
	private float _direction;
	
	// Statuses
	private bool _playerInRange;
	private bool _attacking;
	private bool _hurt;
	private bool _exploding;
	
	// Nodes
	private AnimatedSprite2D _sprite;
	private Area2D _enemyHurtbox, _playerDetectionArea;
	private Timer _attackDelayTimer, _attackCooldownTimer;
	private Marker2D _mouth, _deathExplosionPoint;
	
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
		_deathExplosionPoint = GetNode<Marker2D>("death_explosion_point");
		
		// Animation (using idle for now)
		_sprite.Play("idle");
		
		// Set mouth position
		_mouthPosition = _mouth.Position;
		
		// Set the death explosion point position
		_deathExplosionPointPosition = _deathExplosionPoint.Position;
		
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
			_exploding = true;
			//TODO: Play death animation
			GD.Print("enemy died, \r\n -> await animation finished before QueueFree");
			if (_exploding)
			{
				_exploding = false;
				SpawnDeathProjectiles();
			}
			
			QueueFree();
		}
		
		// Detecting and attacking the player
		if (!_playerInRange) 
			return;
		_playerHeadTargetGlobalPosition = Overlord.Instance.PlayerHeadTargetGlobalPosition;

		if (_attacking)
		{
			_attacking = false;
			SpawnAttackProjectiles();
		}
		else if (_hurt)
		{
			GD.Print("fatty got hit by player's bullets");
		}
		
	}
	
	// Timers
	private void AttackDelayTimerTimedOut()
	{
		GD.Print("ranged enemy attacking");
		_attacking = true;
		
		// When attacking, start the attack cooldown
		_attackCooldownTimer.Start();
	}
	
	private void AttackCooldownTimerTimeout()
	{
		GD.Print("ranged attack cooldown");
		_attacking = false;
		
		// When attack finished, wait for attack delay to start attacking again
		_attackDelayTimer.Start();
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
	private void SpawnAttackProjectiles()
	{
		var projectileInstance = (FattySpit)_fattySpit.Instantiate();
		projectileInstance.ProjectileType = Overlord.EnemyProjectileTypes.AttackProjectile;
		projectileInstance.Target = GlobalPosition.DirectionTo(_playerHeadTargetGlobalPosition);
		projectileInstance.GlobalPosition = _mouth.GlobalPosition;
		GetTree().Root.AddChild(projectileInstance);
	}

	// Spawning projectiles on death
	private void SpawnDeathProjectiles()
	{
		var rng = new RandomNumberGenerator();
		
		for (int i = 0; i < DeathProjectileCount; i++)
		{
			var projectileInstance = (FattySpit)_fattySpit.Instantiate();
			projectileInstance.ProjectileType = Overlord.EnemyProjectileTypes.DeathProjectile;
			projectileInstance.Target = GlobalPosition.DirectionTo(_playerHeadTargetGlobalPosition);
			projectileInstance.GlobalPosition = _deathExplosionPoint.GlobalPosition;
			projectileInstance.RotationDegrees = rng.RandfRange(-DeathProjectileAngle, DeathProjectileAngle);
			
			GetTree().Root.AddChild(projectileInstance);
		}
	}
}
