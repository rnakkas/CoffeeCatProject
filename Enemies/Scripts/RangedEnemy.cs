using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

// Enemy is stationary, shoots projectiles in a straight line towards player's position
//TODO:
// -- only tracking player and shoot at player when within a certain range, use an area2d to detect when player
//		enters this range.
// -- Then use the GlobalPosition.DistanceTo and AngleTo methods to determine direction and angle for shooting
// -- Use the GlobalPosition.AngleTo(player) to get the angle to player to rotate the mouth to shoot.
// -- shooting projectile at player location, projectiles can move through platforms but not through walls
// -- dying when health reaches 0
// -- hurt player if it gets too close, for example if trying to wall jump and this enemy is on the wall
public partial class RangedEnemy : CharacterBody2D
{
	// Consts
	private const float AttackDelayTime = 0.3f;
	private const float AttackCooldownTime = 0.2f;
	
	// Vars
	private int _health = 50;
	private Vector2 _playerGlobalPosition, _mouthPosition;
	private float _direction;
	
	// Statuses
	private bool _attacking;
	private bool _hurt;
	
	// Nodes
	private AnimatedSprite2D _sprite;
	private Area2D _enemyHurtbox;
	private Timer _attackDelayTimer, _attackCooldownTimer;
	private Marker2D _mouth;
	
	public override void _Ready()
	{
		// Get the nodes
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_enemyHurtbox = GetNode<Area2D>("enemy_hurtbox");
		_attackDelayTimer = GetNode<Timer>("attack_delay_timer");
		_attackCooldownTimer = GetNode<Timer>("attack_cooldown_timer");
		_mouth = GetNode<Marker2D>("mouth");
		
		// Animation (using idle for now)
		_sprite.Play("idle");
		
		// Set mouth position
		_mouthPosition = _mouth.Position;
		
		// Get player's position on ready and set direction
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
		SetDirectionToTarget(_playerGlobalPosition);
		GD.Print("ranged enemy sees player location: " + _playerGlobalPosition);
		
		// Area2D signal connections
		_enemyHurtbox.AreaEntered += HitByBullets;
		_enemyHurtbox.AreaExited += BulletsDestroyed;
		
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
}
