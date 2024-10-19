using Godot;
using System;

public partial class Items : Area2D
{
    AnimatedSprite2D animation;
    GameLevelManager gameLevelManager;

    String itemType;
    public override void _Ready()
    {
        gameLevelManager = GetNode<GameLevelManager>("../../../level_manager");
        animation = GetNode<AnimatedSprite2D>("sprite");
        animation.Play("idle");

        this.BodyEntered += OnBodyEntered;

        // Get item type based on the parent node
        itemType = this.GetParent().Name;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.Name == "player")
        {
            PlayerCollectsItem();
        }
    }

    private void PlayerCollectsItem()
    {
        IncreaseScore();

        Tween Tween_1 = this.GetTree().CreateTween();
        Tween Tween_2 = this.GetTree().CreateTween();

        Tween_1.TweenProperty(this, "modulate:a", 0, 0.4);
        Tween_2.TweenProperty(this, "position", this.Position - new Vector2(0, 50), 0.5);
        Tween_2.TweenCallback(Callable.From(this.QueueFree));
    }

    private void IncreaseScore()
    {
        switch (itemType) // Logic based on name of item
        {
            case "items_coffee":
                gameLevelManager.IncreaseCoffeeScore(1);
                break;
            case "items_coffee_pot":
                gameLevelManager.IncreaseCoffeeScore(10);
                break;
        }
    }
}
