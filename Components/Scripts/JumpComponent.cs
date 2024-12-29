using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component allows characters to jump

[GlobalClass]
public partial class JumpComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;

	public bool IsJumping;
	public bool IsFalling;
	
	public void JumpAndFall(float delta, CharacterBody2D characterBody)
	{
		if (Input.IsActionPressed("jump") && characterBody.IsOnFloor())
		{
			_velocityComponent.JumpVelocity();
			IsJumping = true;

			Fall(delta, characterBody);
			
			// if (!characterBody.IsOnFloor())
			// {
			// 	_velocityComponent.FallDueToGravity(delta);
			//
			// 	if (characterBody.Velocity.Y > 0)
			// 	{
			// 		_velocityComponent.FallDueToGravity(delta);
			// 		IsJumping = false;
			// 		IsFalling = true;
			// 	}
			// }
			// else if (characterBody.IsOnFloor() && !Input.IsActionPressed("jump"))
			// {
			// 	_velocityComponent.IdleOnGroundVelocityY();
			// 	IsFalling = false;
			// }
			
		}
		else
		{
			Fall(delta, characterBody);
		}
	}
	
	public void Fall(float delta, CharacterBody2D characterBody)
	{
		if (!characterBody.IsOnFloor())
		{
			_velocityComponent.FallDueToGravity(delta);

			if (characterBody.Velocity.Y > 0)
			{
				IsFalling = true;
				IsJumping = false;
			}
		}
		// else if (characterBody.IsOnFloor() && !Input.IsActionPressed("jump"))
		// {
		// 	_velocityComponent.IdleOnGroundVelocityY();
		// 	IsFalling = false;
		// }
	}
}
