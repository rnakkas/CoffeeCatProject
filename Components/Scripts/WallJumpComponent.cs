using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component allows characters to wall slide

[GlobalClass]
public partial class WallJumpComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;
	[Export] private RayCast2D _leftWallDetector;
	[Export] private RayCast2D _rightWallDetector;

	private bool _wallSlide;
	private float _wallJumpDirection;
	
	public void WallJump(float delta, CharacterBody2D characterBody)
	{
		if (!characterBody.IsOnFloor() && 
		    (_leftWallDetector.IsColliding() || _rightWallDetector.IsColliding())
		   )
		{
			_wallSlide = true;
			_velocityComponent.WallSlideVelocity(delta);
			
			if (Input.IsActionJustPressed("jump"))
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
		else
		{
			_wallSlide = false;
		}
	}
	
	// public void WallJump(float delta)
	// {
	// 	if (_wallSlide && Input.IsActionJustPressed("jump"))
	// 	{
	// 		if (_leftWallDetector.IsColliding())
	// 		{
	// 			_wallJumpDirection = 1.0f;
	// 		}
	// 		else if (_rightWallDetector.IsColliding())
	// 		{
	// 			_wallJumpDirection = -1.0f;
	// 		}
	// 		_velocityComponent.WallJumpVelocity(_wallJumpDirection);
	// 	}
	// }
}
