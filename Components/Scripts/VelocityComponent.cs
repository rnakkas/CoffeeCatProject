using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

// This component handles the velocity of player and enemy entities
[GlobalClass]
public partial class VelocityComponent : Node2D
{
	[Export] private float _maxRunSpeed;
	[Export] private float _minRunSpeed;
	[Export] private float _acceleration;
	[Export] private float _friction;
	[Export] private float _jumpVelocity;
	[Export] private float _gravity;
	[Export] private float _wallSlideGravity;
	[Export] private float _wallJumpVelocity;
	
	private Vector2 _velocity;

	public void FallDueToGravity(float delta)
	{
		_velocity.Y += _gravity * delta;
	}

	public void Jump()
	{
		_velocity.Y = _jumpVelocity;
	}

	public void WallSlide(float delta)
	{
		_velocity.Y += _wallSlideGravity * delta;
	}

	public void AccelerateToMaxRunSpeed(float delta, float direction)
	{
		_velocity.X = _velocity.MoveToward(new Vector2(_maxRunSpeed, _velocity.Y), _acceleration * delta * direction).X;
	}

	public void DecelerateToZeroVelocity(float delta)
	{
		_velocity.X = _velocity.MoveToward(new Vector2(0, _velocity.Y), _friction * delta).X;
	}

	public void DecelerateToMinRunSpeed(float delta)
	{
		_velocity.X = _velocity.MoveToward(new Vector2(_minRunSpeed, _velocity.Y), _friction * delta).X;
	}
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
	}
	
	
}
