using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class FattySpit : Area2D
{
	// Constants
	private const float Speed = 200f;
	
	// Nodes
	private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		// Get the nodes
		_sprite = GetNode<AnimatedSprite2D>("sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		
	}
}
