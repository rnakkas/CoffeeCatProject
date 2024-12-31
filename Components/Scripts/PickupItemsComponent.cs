using Godot;
using CoffeeCatProject.GlobalScripts;

namespace CoffeeCatProject.Components.Scripts;

// This component is for pickup items areas such as coffee, weapons, ammo etc

[GlobalClass]
public partial class PickupItemsComponent : Area2D
{
    [Export] public Overlord.PickupItemTypes ItemType;
    [Export] public Overlord.PickupItemNames ItemName;
    [Export] private AnimatedSprite2D _sprite;
    
    public override void _Ready()
    {
        _sprite.Play("idle");
        _sprite.FlipH = true;

        // Connect to signal when a body enters area
        BodyEntered += OnBodyEntered;
    }

    private void WeaponPickup()
    {
        Tween tween1 = GetTree().CreateTween();
        Tween tween2 = GetTree().CreateTween();

        tween1.TweenProperty(_sprite, "modulate:a", 0, 0.4);
        tween2.TweenProperty(_sprite, "position", _sprite.Position - new Vector2(0, 50), 0.5);
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