using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

// This component handles all player inputs to allow control of a character

[GlobalClass]
public partial class PlayerControllerComponent : Node2D
{
	[Export] private CharacterBody2D _playerCharacter;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private RayCast2D _leftWallDetector;
	[Export] private RayCast2D _rightWallDetector;
	
	private Vector2 _direction = Vector2.Zero;

	public override void _PhysicsProcess(double delta)
	{
		_direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		if (!_playerCharacter.IsOnFloor())
		{
			_velocityComponent.FallDueToGravity((float)delta);
		}

		if (_direction.X != 0)
		{
			_velocityComponent.AccelerateToMaxRunSpeed(_direction.X);
		}
		else if (_direction.X == 0)
		{
			_velocityComponent.DecelerateToZeroVelocity((float)delta);
		}

		if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor())
		{
			_velocityComponent.Jump();
		}

		if (_leftWallDetector.IsColliding() || _rightWallDetector.IsColliding())
		{
			GD.Print("wallslide");
			_velocityComponent.WallSlide((float)delta);
		}
	}
}
