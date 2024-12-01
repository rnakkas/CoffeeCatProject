using CoffeeCatProject.Singletons;
using Godot;

namespace CoffeeCatProject.ItemPickups.WeaponPickups.Scripts;

public partial class ShotgunPickup : WeaponPickups
{
    //Nodes
    private AnimatedSprite2D _animation;
    
    public override void _Ready()
    {
        // Get nodes
        _animation = GetNode<AnimatedSprite2D>("sprite");
		
        _animation.Play("idle");
        _animation.FlipH = true;

        // Connect to signal when a body enters area
        BodyEntered += OnBodyEntered;
        
        // Set metadata of node
        SetMeta("role", WeaponTypes.PlayerWeaponTypes.Shotgun.ToString());
    }

    protected override void WeaponPickup()
    {
        Tween tween1 = GetTree().CreateTween();
        Tween tween2 = GetTree().CreateTween();

        tween1.TweenProperty(_animation, "modulate:a", 0, 0.4);
        tween2.TweenProperty(_animation, "position", _animation.Position - new Vector2(0, 50), 0.5);
        tween2.TweenCallback(Callable.From(QueueFree));
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body.GetMeta("role").ToString() == "Player")
        {
            WeaponPickup();
        }
    }
}