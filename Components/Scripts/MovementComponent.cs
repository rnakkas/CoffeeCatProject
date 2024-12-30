using Godot;

namespace CoffeeCatProject.Components.Scripts;

[GlobalClass]
public partial class MovementComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;
	[Export] private CharacterBody2D _characterBody;
	[Export] private RayCast2D _leftWallDetector;
	[Export] private RayCast2D _rightWallDetector;
	
	public Vector2 Direction = Vector2.Zero;
	
	public void Run()
	{
		Direction = Input.GetVector(
			"move_left", 
			"move_right", 
			"move_up", 
			"move_down"
		);
		
		if (Direction.X != 0)
		{
			_velocityComponent.AccelerateToMaxRunVelocity(Direction.X);
		}
		else if (
			(Direction.X == 0 && _characterBody.IsOnFloor()) ||
			Direction.X == 0
			)
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

	public void Idle()
	{
		if (_characterBody.IsOnFloor())
		{
			_velocityComponent.IdleOnGroundVelocityY();
		}
	}

	public void Jump()
	{
		if (_characterBody.IsOnFloor() && Input.IsActionPressed("jump"))
		{
			_velocityComponent.JumpVelocity();
		}
	}

	public void WallJump(float delta)
	{
		if (!_characterBody.IsOnFloor() && 
		    (_leftWallDetector.IsColliding() || _rightWallDetector.IsColliding())
		   )
		{
			_velocityComponent.WallSlideVelocity(delta);
			
			if (Input.IsActionJustPressed("jump"))
			{
				if (_leftWallDetector.IsColliding())
				{
					Direction.X = 1.0f;
				}
				else if (_rightWallDetector.IsColliding())
				{
					Direction.X = -1.0f;
				}
				_velocityComponent.WallJumpVelocity(Direction.X);
			}
		}
	}
}
