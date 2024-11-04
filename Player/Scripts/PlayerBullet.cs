using Godot;
using System;

namespace CoffeeCatProject.Player.Scripts;

public partial class PlayerBullet : CharacterBody2D
{
	private const float BulletSpeed = 500f;
	
	private AnimatedSprite2D _animation;
	private Vector2 _velocity = Vector2.Zero;

	private float _direction;

	public float Direction
	{
		get => _direction;
		set => _direction = value;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("sprite");
		
		_velocity = Velocity;
		
		FloorMaxAngle = 0;
		
		// Flip sprite based on direction
		FlipSprite();
		
		_animation.Play("fly");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _PhysicsProcess(double delta)
	{
		// Flip sprite based on direction
		FlipSprite();

		// If bullet hits floor or wall, bullet disappears
		if (IsOnFloor() || IsOnWall())
		{
			_animation.Play("hit");
			await ToSignal(_animation, "animation_finished");
			QueueFree();
		}

		MoveLocalX(BulletSpeed * (float)delta * _direction);		
		Velocity = _velocity;
		MoveAndSlide();
	}

	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (_direction < 0)
		{
			_animation.FlipH = false;
		}

		if (_direction > 0)
		{
			_animation.FlipH = true;
		}
	} 
}
