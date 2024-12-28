using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

// This component handles all player inputs for all possible player movement options

[GlobalClass]
public partial class PlayerControllerComponent : Node2D
{
	[Export] private CharacterBody2D _playerCharacter;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private RayCast2D _leftWallDetector;
	[Export] private RayCast2D _rightWallDetector;
	[Export] private RunComponent _runComponent;
	
	private Vector2 _direction = Vector2.Zero;
	private float _wallJumpDirection;
	private bool _wallSlide;

	public void Fall(float delta)
	{
		if (!_playerCharacter.IsOnFloor())
		{
			_velocityComponent.FallDueToGravity(delta);
		}
		else
		{
			_velocityComponent.IdleOnGroundVelocityY();
		}
	}

	// public void Run(float delta)
	// {
	// 	if (_direction.X != 0)
	// 	{
	// 		_velocityComponent.AccelerateToMaxRunVelocity(_direction.X);
	// 	}
	// 	else if (_direction.X == 0)
	// 	{
	// 		_velocityComponent.DecelerateToZeroVelocity((float)delta);
	// 	}
	// }

	public void Jump(float delta)
	{
		if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor())
		{
			_velocityComponent.JumpVelocity();
		}
	}

	public void WallSlide(float delta)
	{
		if (!_playerCharacter.IsOnFloor() && 
		    (_leftWallDetector.IsColliding() || _rightWallDetector.IsColliding())
		   )
		{
			_wallSlide = true;
			_velocityComponent.WallSlideVelocity((float)delta);
		}
		else
		{
			_wallSlide = false;
		}
	}

	public void WallJump(float delta)
	{
		if (_wallSlide && Input.IsActionJustPressed("jump"))
		{
			if (_leftWallDetector.IsColliding())
			{
				_wallJumpDirection = 1.0f;
			}
			else if (_rightWallDetector.IsColliding())
			{
				_wallJumpDirection = -1.0f;
			}
			_velocityComponent.WallJumpVelocity(_wallJumpDirection);
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_direction = Input.GetVector(
			"move_left", 
			"move_right", 
			"move_up", 
			"move_down"
			);
		
		// Fall, run and jump are common to all players
		Fall((float)delta);
		_runComponent?.Run((float)delta, _direction);
		Jump((float)delta);
		
		// WallSlide((float)delta);
		// WallJump((float)delta);
		
	}
}
