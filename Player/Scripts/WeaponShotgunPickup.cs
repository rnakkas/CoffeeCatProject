using Godot;

namespace CoffeeCatProject.Player.Scripts;

public partial class WeaponShotgunPickup : Area2D
{
	//Nodes
	private AnimatedSprite2D _animation;
	
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("sprite");
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

			tween1.TweenProperty(this, "modulate:a", 0, 0.4);
			tween2.TweenProperty(this, "position", this.Position - new Vector2(0, 50), 0.5);
			tween2.TweenCallback(Callable.From(QueueFree));
		}
	}
}
