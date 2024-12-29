using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component allows characters to jump

[GlobalClass]
public partial class JumpComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;
	
	public void Jump(float delta, CharacterBody2D characterBody)
	{
		if (Input.IsActionPressed("jump") && characterBody.IsOnFloor())
		{
			_velocityComponent.JumpVelocity();
		}
	}
}
