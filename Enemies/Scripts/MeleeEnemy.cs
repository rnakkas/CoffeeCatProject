using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class MeleeEnemy : CharacterBody2D
{
	// Consts
	private const float Speed = 120.0f;
	private const float Gravity = 750.0f;
	private const float ChaseTime = 2.0f;
	
	// Vars
	private int _health = 100;
	private Vector2 _velocity;
	private Vector2 _playerGlobalPosition;
	private float _direction;
	private bool _chasing;
	
	
	// Nodes
	private Area2D _hitbox; 
	private AnimatedSprite2D _sprite;
	private Timer _chaseTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_hitbox = GetNode<Area2D>("enemy_hitbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_chaseTimer = GetNode<Timer>("chase_timer");
		
		_sprite.Play("idle");

		_hitbox.AreaEntered += HitByBullets;
		
		_velocity = Velocity;
		
		// Connect to player variables signal for player position
		PlayerVariables.Instance.PlayerGlobalPositionUpdated += OnPlayerGlobalPositionUpdated;
		
		// Connect to timer signal
		_chaseTimer.SetOneShot(true);
		_chaseTimer.SetWaitTime(ChaseTime);
		_chaseTimer.Timeout += ChaseTimeout;

	}

	private void ChaseTimeout()
	{
		_chasing = false;
		GD.Print("Chase Timeout");
	}
	
	private void HitByBullets(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_health -= 5;
		}
	}

	private void OnPlayerGlobalPositionUpdated()
	{
		_playerGlobalPosition = PlayerVariables.Instance.PlayerGlobalPosition;
	}

	private void MoveTowardsPlayer(Vector2 target, float delta)
	{
		// GlobalPosition += GlobalPosition.DirectionTo(target) * Speed * delta;

		if (GlobalPosition.Y <= target.Y)
		{
			GlobalPosition += GlobalPosition.DirectionTo(target) * Speed * delta;
			_chasing = true;
		}
		else if (GlobalPosition.Y > target.Y && _chasing)
		{
			GlobalPosition += GlobalPosition.DirectionTo(target) * Speed * delta;
		}
		
		_chaseTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
		
		MoveTowardsPlayer(_playerGlobalPosition, (float)delta);
		
		Velocity = _velocity;
		MoveAndSlide();
	}

	
}
