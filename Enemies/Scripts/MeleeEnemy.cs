using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class MeleeEnemy : CharacterBody2D
{
	// Consts
	private const float Speed = 250.0f;
	private const float Gravity = 750.0f;
	
	// Vars
	private int _health = 100;
	private Vector2 _velocity;
	
	// Nodes
	private Area2D _hitbox; 
	private AnimatedSprite2D _sprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_hitbox = GetNode<Area2D>("enemy_hitbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		
		_sprite.Play("idle");

		_hitbox.AreaEntered += HitByBullets;
		
		_velocity = Velocity;

	}
	
	private void HitByBullets(Area2D area)
	{
		if (area.GetMeta("role").ToString().ToLower() == "bullet")
		{
			_health -= 5;
		}
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
		
		Velocity = _velocity;
		MoveAndSlide();
	}

	
}
