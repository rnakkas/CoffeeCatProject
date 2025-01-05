using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component is the Area2D for pickup items like coffee, weapons, keys etc.

[GlobalClass]
public partial class PickupItemsComponent : Area2D
{
    [Export] public Overlord.PickupItemTypes ItemType;
    [Export] public Overlord.PickupItemNames ItemName;
    [Export] public int HealAmount;
    [Export] private AnimatedSprite2D _sprite;
    
    public override void _Ready()
    {
        _sprite.Play("idle");
        _sprite.FlipH = true;

        // Connect to signal when a body enters area
        BodyEntered += OnBodyEntered;
    }

    private void ItemGetsPickedUp()
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
            ItemGetsPickedUp();
        }
    }
}