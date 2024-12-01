using Godot;

namespace CoffeeCatProject.ItemPickups.WeaponPickups.Scripts;
public partial class WeaponPickups : Area2D
{
	//Nodes
	private AnimatedSprite2D _animation;
	
	public override void _Ready()
	{
		// Get nodes
		_animation = GetNode<AnimatedSprite2D>("sprite");
		
		_animation.Play("idle");

		// Signal of OnBodyEntered
		BodyEntered += OnBodyEntered;
	}

	// OnBodyEntered signal
	private void OnBodyEntered(Node2D body)
	{
		if (body.GetMeta("role").ToString() == "Player")
		{
			Tween tween1 = GetTree().CreateTween();
			Tween tween2 = GetTree().CreateTween();

			tween1.TweenProperty(_animation, "modulate:a", 0, 0.4);
			tween2.TweenProperty(_animation, "position", _animation.Position - new Vector2(0, 50), 0.5);
			tween2.TweenCallback(Callable.From(QueueFree));
		}
	}
}
