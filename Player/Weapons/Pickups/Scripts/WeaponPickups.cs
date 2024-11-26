using Godot;

namespace CoffeeCatProject.Player.Weapons.Pickups.Scripts;
public partial class WeaponPickups : Area2D
{
	//Nodes
	private AnimatedSprite2D _animation;
	private Node _parentNode;
	
	public override void _Ready()
	{
		// Get nodes
		_animation = GetNode<AnimatedSprite2D>("../sprite");
		_parentNode = GetParent();
		
		_animation.Play("idle");

		// Signal of OnBodyEntered
		BodyEntered += OnBodyEntered;
	}

	// OnBodyEntered signal
	private void OnBodyEntered(Node2D body)
	{
		if (body.Name.ToString().ToLower().Contains("player"))
		{
			Tween tween1 = GetTree().CreateTween();
			Tween tween2 = GetTree().CreateTween();

			tween1.TweenProperty(_animation, "modulate:a", 0, 0.4);
			tween2.TweenProperty(_animation, "position", _animation.Position - new Vector2(0, 50), 0.5);
			tween2.TweenCallback(Callable.From(_parentNode.QueueFree));
		}
	}
}
