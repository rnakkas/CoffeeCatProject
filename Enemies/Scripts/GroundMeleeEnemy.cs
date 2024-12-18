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
	private const float AttackDelayTime = 0.3f;
	private const float AttackCooldownTime = 0.2f;
	private const float SlowDownRate = 2.7f;
	
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
	private Area2D _enemyHurtbox, _attackArea, _attackHitbox; 
	private AnimatedSprite2D _sprite;
	private RayCast2D _leftWallDetect, _rightWallDetect, _playerDetector;
	private Timer _chaseTimer, _attackDelayTimer, _attackCooldownTimer;
	private CollisionShape2D _attackHitboxCollider;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_enemyHurtbox = GetNode<Area2D>("enemy_hurtbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		_leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
		_rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
		_attackArea = GetNode<Area2D>("attack_area");
		_attackHitbox = GetNode<Area2D>("attack_hitbox");
		_attackHitboxCollider = GetNode<CollisionShape2D>("attack_hitbox/attack_hitbox_collider");
		_playerDetector = GetNode<RayCast2D>("player_detector");
		_chaseTimer = GetNode<Timer>("chase_timer");
		_attackDelayTimer = GetNode<Timer>("attack_delay_timer");
		_attackCooldownTimer = GetNode<Timer>("attack_cooldown_timer");
		
		// Animation
		//TODO: on ready, play spawn animation instead
		_sprite.Play("idle");

		// Area2D signal connections
		_enemyHurtbox.AreaEntered += HitByBullets;
		_enemyHurtbox.AreaExited += BulletsDestroyed;
		_attackArea.AreaEntered += PlayerEnteredAttackArea;
		_attackArea.AreaExited += PlayerExitedAttackArea;
		
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
		
		_attackCooldownTimer.OneShot = true;
		_attackCooldownTimer.WaitTime = AttackCooldownTime;
		_attackCooldownTimer.Timeout += AttackCooldownTimerTimeout;
		
		// Set metadata for attack area
		_attackHitbox.SetMeta(
			Overlord.EnemyMetadataTypes.AttackType.ToString(), 
			Overlord.EnemyAttackTypes.MeleeAttack.ToString()
			);
	}

	public override void _PhysicsProcess(double delta)
	{
		ReboundFromWall();
		FlipSpriteAndDetectors();
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
			_attackHitboxCollider.Disabled = false;
		}
		else if (!_attacking)
		{
			_attackHitboxCollider.Disabled = true;
		}
		else if (_hurt)
		{
			GD.Print("enemy got hit by player's bullets");
		}
		
		Velocity = _velocity;
		MoveAndSlide();
	}

	// Timers
	private void ChaseTimerTimedOut()
	{
		_chasing = false;
	}
	
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
	private void FlipSpriteAndDetectors() // Flip the player detector raycast based on movement direction
	{
		if (_direction < 0)
		{
			_sprite.FlipH = false;
			_playerDetectorTargetPosition = new Vector2(-PlayerDetectionRange, 0);
			_attackArea.Scale = new Vector2(1.0f, 1.0f);
			_attackHitbox.Scale = new Vector2(1.0f, 1.0f);
		}
		else if (_direction > 0)
		{
			_sprite.FlipH = true;
			_playerDetectorTargetPosition = new Vector2(PlayerDetectionRange, 0);
			_attackArea.Scale = new Vector2(-1.0f, 1.0f);
			_attackHitbox.Scale = new Vector2(-1.0f, 1.0f);
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
	private void PlayerEnteredAttackArea(Node2D area)
	{
		if (area.Name != "player_area") 
			return;

		_attackDelayTimer.Start();
	}
	
	private void PlayerExitedAttackArea(Node2D area)
	{
		if (area.Name != "player_area") 
			return;
		
		// If player quickly jumps out of attack area before attack delay timer runs out, the timer will be stopped
		// So that enemy doesn't get stuck in attack mode if the player is not in its attack area
		_attackDelayTimer.Stop();
		_attackCooldownTimer.Stop();
		_attacking = false;
	}
	
	// Logic for chasing the player
	private void ChaseThePlayer()
	{
		_playerGlobalPosition = Overlord.Instance.PlayerGlobalPosition;
		SetDirectionToTarget(_playerGlobalPosition);
		
		// If player jumps or standing on a platform above enemy during chase, slowdown to zero until chase timer runs out
		// If player jumps over the enemy during chase, the enemy will "skid" and turn around to continue chasing
		if (_playerGlobalPosition.Y < GlobalPosition.Y)
		{
			_velocity = _velocity.MoveToward(Vector2.Zero, SlowDownRate);
		}
		else
		{
			_velocity.X = _direction * ChaseSpeed;
			
			// Stop at a certain distance from player to attack
			if (GlobalPosition.DistanceTo(_playerGlobalPosition) <= DistanceFromPlayerForAttack)
			{
				_velocity.X = 0;
			}
		}
	}
}
