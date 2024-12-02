using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class MeleeEnemy : CharacterBody2D
{
	// Consts
	private const float Speed = 250.0f;
	private const float Gravity = 750.0f;
	
	// Nodes
	private Area2D _hitbox; 
	private AnimatedSprite2D _sprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		_hitbox = GetNode<Area2D>("hitbox");
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		
		_sprite.Play("idle");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
