using System;
using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

// Enemy continuously chases player, but only if they're on the ground
//TODO: create the animations for spawning, attacking, getting hurt and dying
public partial class GroundMeleeEnemy : CharacterBody2D
{
	// Const
	private const float ChaseSpeed = 150.0f;
	private const float PatrolSpeed = 60.0f;
	private const float Gravity = 750.0f;
	private const float DistanceFromPlayerForAttack = 30.0f;
	private const float PlayerDetectionRange = 300.0f;
	private const float ChaseTime = 3.0f;
	private const float AttackDelayTime = 0.8f;
	
	// Vars
	private int _health = 100;
	private Vector2 _velocity;
	private Vector2 _playerGlobalPosition;
	private float _direction;
	private Vector2 _playerDetectorTargetPosition;
	
	// Statuses
	private bool _attacking;
	private bool _hurt;
	private bool _chasing;
	
	// Nodes
	private Area2D _enemyHurtbox; 
	private AnimatedSprite2D _sprite;
	private RayCast2D _leftWallDetect, _rightWallDetect, _playerDetector;
	private Area2D _attackArea;
	private Timer _chaseTimer;
	private Timer _attackDelayTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_enemyHurtbox = GetNode<Area2D>("enemy_hurtbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
		_rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
		_attackArea = GetNode<Area2D>("attack_area");
		_playerDetector = GetNode<RayCast2D>("player_detector");
		_chaseTimer = GetNode<Timer>("chase_timer");
		_attackDelayTimer = GetNode<Timer>("attack_delay_timer");
		
		// Animation
		//TODO: on ready, play spawn animation instead
		_sprite.Play("idle");

		// Signal connects
		_enemyHurtbox.AreaEntered += HitByBullets;
		_enemyHurtbox.AreaExited += BulletsDestroyed;
		_attackArea.BodyEntered += PlayerEnteredAttackArea;
		_attackArea.BodyExited += PlayerExitedAttackArea;
		
		// Get player's position on ready and set direction
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
		SetDirectionToTarget(_playerGlobalPosition);
		
		// Velocity
		_velocity = Velocity;
		
		//Player detector target position
		_playerDetectorTargetPosition = _playerDetector.TargetPosition;
		
		// Set timer values
		_chaseTimer.OneShot = true;
		_chaseTimer.WaitTime = ChaseTime;
		_chaseTimer.Timeout += ChaseTimerTimedOut;
		_attackDelayTimer.OneShot = true;
		_attackDelayTimer.WaitTime = AttackDelayTime;
		_attackDelayTimer.Timeout += AttackDelayTimerTimedOut;

	}

	// Timers
	private void ChaseTimerTimedOut()
	{
		_chasing = false;
	}
	private void AttackDelayTimerTimedOut()
	{
		_attacking = true;
	}
	
	// Getting hit by player's bullets
	private void HitByBullets(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_health -= 5;
			_hurt = true;
			
			// If hurt, look in the player's direction
			_playerDetectorTargetPosition = Overlord.Instance.PlayerGlobalPosition;
			SetDirectionToTarget(_playerDetectorTargetPosition);
		}
	}
	private void BulletsDestroyed(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_hurt = false;
		}
	}

	
	// Detecting the player for chasing
	private void FlipPlayerDetector() // Flip the player detector raycast based on movement direction
	{
		if (_direction < 0)
		{
			_playerDetectorTargetPosition = new Vector2(-PlayerDetectionRange, 0);
		}
		else if (_direction > 0)
		{
			_playerDetectorTargetPosition = new Vector2(PlayerDetectionRange, 0);
		}
		
		_playerDetector.TargetPosition = _playerDetectorTargetPosition;
	}
	private void PlayerEnteredDetectionRange()
	{
		if (_playerDetector.IsColliding())
		{
			_chasing = true;
			_chaseTimer.Start();
		}
	}
	
	// Detecting when the player enters the attacking range
	private void PlayerEnteredAttackArea(Node2D body)
	{
		if (body.GetMeta("role").ToString().ToLower() != "player") 
			return;
		
		_attackDelayTimer.Start();
	}
	private void PlayerExitedAttackArea(Node2D body)
	{
		if (body.GetMeta("role").ToString().ToLower() != "player") 
			return;
		
		_attacking = false;
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
	private void ReboundFromWall() // Flipping the direction if colliding with a wall 
	{
		if (_leftWallDetect.IsColliding())
		{
			_direction = 1.0f;
		}
		if (_rightWallDetect.IsColliding())
		{
			_direction = -1.0f;
		}
	}
	
	// Logic for chasing the player
	private void ChaseThePlayer()
	{
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;

		if (_playerGlobalPosition.Y >= GlobalPosition.Y)
		{
			SetDirectionToTarget(_playerGlobalPosition);
		}
			
		_velocity.X = _direction * ChaseSpeed;
			
		// If player position reached while chasing, stop a certain distance from the player for attacks
		if (GlobalPosition.DistanceTo(_playerGlobalPosition) <= DistanceFromPlayerForAttack)
		{
			// _velocity = _velocity.MoveToward(Vector2.Zero, SlowdownRate);
			_velocity.X = 0;
		}
	}
	
	public override void _Process(double delta)
	{
		ReboundFromWall();
		FlipPlayerDetector();
		PlayerEnteredDetectionRange();

		// Patrolling and chasing player
		switch (_chasing)
		{
			case true:
			{
				ChaseThePlayer();
				break;
			}
			
			case false: 
				_velocity.X = _direction * PatrolSpeed;
				break;
		}
		
		// Fall if in the air
		if (!IsOnFloor())
		{
			_velocity.Y += Gravity * (float)delta;
		}
		
		// Die when health reaches 0
		if (_health <= 0)
		{
			//TODO: Play death animation
			GD.Print("enemy died, \r\n await animation finished before QueueFree");
			QueueFree();
		}
		
		//TODO: Add attack animation, hurt animation and death animation based on player interaction 
		if (_attacking)
		{
			GD.Print("enemy attacking the player");
		}
		else if (!_attacking)
		{
			// GD.Print("enemy stopped attacking the player");
		}
		else if (_hurt)
		{
			// GD.Print("enemy got hit by player's bullets");
		}
		
		Velocity = _velocity;
		MoveAndSlide();
	}

	
}
