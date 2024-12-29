using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component allows characters to fall due to gravity

[GlobalClass]
public partial class FallComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;

	public bool IsFalling;
	
	public void Fall(float delta, CharacterBody2D characterBody)
	{
		if (!characterBody.IsOnFloor())
		{
			_velocityComponent.FallDueToGravity(delta);
			IsFalling = true;
		}
		else if (characterBody.IsOnFloor() && !Input.IsActionPressed("jump"))
		{
			_velocityComponent.IdleOnGroundVelocityY();
			IsFalling = false;
		}
	}
}
