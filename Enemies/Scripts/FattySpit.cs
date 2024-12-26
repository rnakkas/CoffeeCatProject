using Godot;
using CoffeeCatProject.GlobalScripts;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class FattySpit : Area2D
{
	// Constants
	private const float Speed = 250f;
	
	// Nodes
	private AnimatedSprite2D _sprite;
	
	// Variables
	public Vector2 Target {get; set;}
	private bool _hitStatus;
	public Overlord.EnemyProjectileTypes ProjectileType {get; set;}
	private float _direction;
	
	public override void _Ready()
	{
		// Get the nodes
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		
		// Flip sprite based on direction
		FlipSprite();
		
		_sprite.Play("fly");

		// Area2D signals
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		
		// Set metadata for attack area
		SetMeta(
			Overlord.EnemyMetadataTypes.AttackType.ToString(), 
			Overlord.EnemyAttackTypes.ProjectileAttack.ToString()
			);
	}

	public override async void _PhysicsProcess(double delta)
	{
		if (!_hitStatus)
		{
			ProjectilesBehaviour((float)delta);
		}
		else
		{
			MoveLocalX(0);
			MoveLocalY(0);
			_sprite.Play("hit");
			// await ToSignal(_sprite, "animation_finished");
			QueueFree();
		}
	}
	
	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (Target.X < 0)
		{
			_sprite.FlipH = false;
			_direction = -1.0f;
		}

		if (Target.X > 0)
		{
			_sprite.FlipH = true;
			_direction = 1.0f;
		}
		
	}
	
	// Projectile's area2d signals
	private void OnBodyEntered(Node body)
	{
		if (body is TileMapLayer)
		{
			_hitStatus = true;
		}
	}

	private void OnAreaEntered(Node area)
	{
		if (area.Name == "player_area")
		{
			_hitStatus = true;
		}
	}
	
	// Projectiles behaviour based on type
	private void ProjectilesBehaviour(float delta)
	{
		switch (ProjectileType)
		{
			case Overlord.EnemyProjectileTypes.AttackProjectile:
				MoveLocalX(Speed * delta * Target.X);
				MoveLocalY(Speed * delta * Target.Y);
				break;
			case Overlord.EnemyProjectileTypes.DeathProjectile:
				MoveLocalX(Speed * delta * _direction);
				break;
		}
	}
}
