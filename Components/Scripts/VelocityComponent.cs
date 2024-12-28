using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

// This component handles the velocity of player and enemy entities
[GlobalClass]
public partial class VelocityComponent : Node2D
{
	[Export] private CharacterBody2D _characterBody;
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

	public void IdleOnGroundVelocity()
	{
		_velocity.Y = 0;
	}

	public void JumpVelocity()
	{
		_velocity.Y = _jumpVelocity;
	}

	public void WallJumpVelocity(float wallJumpDirection)
	{
		_velocity.Y = _wallJumpVelocity;
		_velocity.X = wallJumpDirection * _maxRunSpeed;
	}

	public void WallSlideVelocity(float delta)
	{
		_velocity.Y += _wallSlideGravity * delta;
	}

	public void AccelerateToMaxRunVelocity(float direction)
	{
		_velocity = _velocity.MoveToward(new Vector2(_maxRunSpeed * direction, _velocity.Y), _acceleration);
	}

	public void DecelerateToZeroVelocity(float delta)
	{
		_velocity = _velocity.MoveToward(new Vector2(0, _velocity.Y), _friction);
	}

	public void DecelerateToMinRunVelocity(float direction)
	{
		_velocity = _velocity.MoveToward(new Vector2(_minRunSpeed * direction, _velocity.Y), _friction);
	}
	
	public override void _Ready()
	{
		_velocity = _characterBody.Velocity;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_characterBody.Velocity = _velocity;
	}
	
	
}
