using Godot;

namespace CoffeeCatProject.Components.Scripts;

[GlobalClass]
public partial class MovementComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;
	[Export] private CharacterBody2D _characterBody;
	[Export] private RayCast2D _leftWallDetector;
	[Export] private RayCast2D _rightWallDetector;
	
	private Vector2 _direction = Vector2.Zero;
	
	public void Run()
	{
		_direction = Input.GetVector(
			"move_left", 
			"move_right", 
			"move_up", 
			"move_down"
		);
		
		if (_direction.X != 0)
		{
			_velocityComponent.AccelerateToMaxRunVelocity(_direction.X);
		}
		else if (_direction.X == 0 && _characterBody.IsOnFloor())
		{
			_velocityComponent.DecelerateToZeroVelocity();
		}
		else if (_direction.X == 0)
		{
			_velocityComponent.DecelerateToZeroVelocity();
		}
	}

	public void Fall(float delta)
	{
		if (!_characterBody.IsOnFloor())
		{
			_velocityComponent.FallDueToGravity(delta);
		}
	}

	public void OnGround()
	{
		if (_characterBody.IsOnFloor())
		{
			_velocityComponent.IdleOnGroundVelocityY();
		}
	}

	public void Jump(float delta)
	{
		if (_characterBody.IsOnFloor() && Input.IsActionPressed("jump"))
		{
			_velocityComponent.JumpVelocity();
		}
	}

	public void WallJump(float delta)
	{
		var wallJumpDirection = 0.0f;
		
		if (!_characterBody.IsOnFloor() && 
		    (_leftWallDetector.IsColliding() || _rightWallDetector.IsColliding())
		   )
		{
			_velocityComponent.WallSlideVelocity(delta);
			
			if (Input.IsActionJustPressed("jump"))
			{
				if (_leftWallDetector.IsColliding())
				{
					wallJumpDirection = 1.0f;
				}
				else if (_rightWallDetector.IsColliding())
				{
					wallJumpDirection = -1.0f;
				}
				_velocityComponent.WallJumpVelocity(wallJumpDirection);
			}
		}
	}
}
