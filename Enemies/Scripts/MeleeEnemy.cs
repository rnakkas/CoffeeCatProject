using System.Numerics;
using CoffeeCatProject.GlobalScripts;
using Godot;
using Vector2 = Godot.Vector2;

namespace CoffeeCatProject.Enemies.Scripts;

// Enemy continuously chases player, but only if they're on the ground
public partial class MeleeEnemy : CharacterBody2D
{
	// Const
	private const float Speed = 120.0f;
	private const float MinSpeed = 50.0f;
	private const float Gravity = 750.0f;
	private const float SlowdownRate = 80.0f;
	private const float DistanceFromPlayerForAttack = 25.0f;
	
	// Vars
	private int _health = 100;
	private Vector2 _velocity;
	private Vector2 _playerGlobalPosition;
	private float _direction;
	private bool _attacking;
	
	
	// Nodes
	private Area2D _hitbox; 
	private AnimatedSprite2D _sprite;
	private RayCast2D _leftWallDetect, _rightWallDetect;
	private Area2D _attackArea;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_hitbox = GetNode<Area2D>("enemy_hitbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
		_rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
		_attackArea = GetNode<Area2D>("attack_area");
		
		// Animation
		_sprite.Play("idle");

		// Signal connects
		_hitbox.AreaEntered += HitByBullets;
		_attackArea.BodyEntered += PlayerEnteredAttackArea;
		_attackArea.BodyExited += PlayerExitedAttackArea;
		
		// Get player's position on ready
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
		
		// Velocity
		_velocity = Velocity;

	}
	
	private void HitByBullets(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_health -= 5;
		}
	}

	private void PlayerEnteredAttackArea(Node2D body)
	{
		if (body.GetMeta("role").ToString().ToLower() != "player") 
			return;
		
		_attacking = true;
		GD.Print("attacking the player: " +_attacking);
	}

	private void PlayerExitedAttackArea(Node2D body)
	{
		if (body.GetMeta("role").ToString().ToLower() != "player") 
			return;
		
		_attacking = false;
		GD.Print("attacking the player: " +_attacking);
	}

	private void MoveTowardsPlayer(Vector2 target, float delta)
	{
		SetSelfDirection(target);
		
		// Chase player if player is on floor or below self
		if (target.Y >= GlobalPosition.Y)
		{
			_velocity.X = _direction * Speed;
			Velocity = _velocity;
		}
		// Slow down to zero if player is above self and rebound if hitting wall
		else if (target.Y < GlobalPosition.Y)
		{
			ReboundFromWall();
			_velocity = _velocity.MoveToward(Vector2.Zero, delta * SlowdownRate);
			Velocity = _velocity;
		}
		
		// Stop a certain distance from player for attacking
		if (GlobalPosition.DistanceTo(target) < DistanceFromPlayerForAttack)
		{
			_velocity.X = 0.0f;
			Velocity = _velocity;
		}
	}

	private void SetSelfDirection(Vector2 target)
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
			_velocity.X = _direction * MinSpeed;
		}
		if (_rightWallDetect.IsColliding())
		{
			_velocity.X = -_direction * MinSpeed;
		}
	}
	
	public override void _Process(double delta)
	{
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
		
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
		MoveTowardsPlayer(_playerGlobalPosition, (float)delta);
		
		Velocity = _velocity;
		MoveAndSlide();
	}

	
}
