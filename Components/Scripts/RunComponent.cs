using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component allows characters to run

[GlobalClass]
public partial class RunComponent : Node2D
{
	[Export] private VelocityComponent _velocityComponent;
	
	public void Run(float delta, Vector2 direction)
	{
		if (direction.X != 0)
		{
			_velocityComponent.AccelerateToMaxRunVelocity(direction.X);
		}
		else if (direction.X == 0)
		{
			_velocityComponent.DecelerateToZeroVelocity((float)delta);
		}
	}
	
}
