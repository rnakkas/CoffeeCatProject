using System;
using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

// Enemy continuously chases player, but only if they're on the ground
//TODO: create the animations for spawning, attacking, getting hurt and dying
public partial class MeleeEnemy : CharacterBody2D
{
	// Const
	private const float ChaseSpeed = 150.0f;
	private const float PatrolSpeed = 60.0f;
	private const float Gravity = 750.0f;
	private const float SlowdownRate = 80.0f;
	private const float DistanceFromPlayerForAttack = 25.0f;
	private const float PlayerDetectionRange = 300.0f;
	private const float ChaseTime = 3.0f;
	
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
	private Area2D _hitbox; 
	private AnimatedSprite2D _sprite;
	private RayCast2D _leftWallDetect, _rightWallDetect, _playerDetector;
	private Area2D _attackArea;
	private Timer _chaseTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_hitbox = GetNode<Area2D>("enemy_hitbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
		_rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
		_attackArea = GetNode<Area2D>("attack_area");
		_playerDetector = GetNode<RayCast2D>("player_detector");
		_chaseTimer = GetNode<Timer>("chase_timer");
		
		// Animation
		//TODO: on ready, play spawn animation instead
		_sprite.Play("idle");

		// Signal connects
		_hitbox.AreaEntered += HitByBullets;
		_hitbox.AreaExited += BulletsDestroyed;
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

	}

	private void ChaseTimerTimedOut()
	{
		_chasing = false;
	}
	
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

	private void PlayerEnteredAttackArea(Node2D body)
	{
		if (body.GetMeta("role").ToString().ToLower() != "player") 
			return;
		
		_attacking = true;
	}

	private void PlayerExitedAttackArea(Node2D body)
	{
		if (body.GetMeta("role").ToString().ToLower() != "player") 
			return;
		
		_attacking = false;
	}

	private void PlayerEnteredDetectionRange()
	{
		if (_playerDetector.IsColliding())
		{
			_chasing = true;
			_chaseTimer.Start();
		}
	}

	private void Patrolling()
	{
		ReboundFromWall();
		_velocity.X = _direction * PatrolSpeed;
	}

	private void Chasing()
	{
		ReboundFromWall();
		_velocity.X = _direction * ChaseSpeed;
		
	}

	// Set a direction float based on where the target/player is
	private void SetDirectionToTarget(Vector2 target)
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

	private void ReboundFromWall()
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

	// Flip the player detection raycast based on movement direction
	private void FlipPlayerDetector()
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
	
	public override void _Process(double delta)
	{
		FlipPlayerDetector();
		PlayerEnteredDetectionRange();
		
		if (!_chasing)
		{
			Patrolling();
		}
		else if (_chasing)
		{
			_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
			Chasing();
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
			GD.Print("enemy died");
			QueueFree();
		}
		
		
		//TODO: Add attack animation, hurt animation and death animation based on player interaction 
		if (_attacking)
		{
			// GD.Print("enemy attacking the player");
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
