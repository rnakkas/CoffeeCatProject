using Godot;
using System;

public partial class Items : Area2D
{
    AnimatedSprite2D animation;
    ScoreManager scoreManager;

    private NodePath scoreManagerNodePath = "../../score_manager";
    private int coffeeScore = 1;
    private int coffeePotScore = 10;

    String itemType;
    public override void _Ready()
    {
        // Get the score manager node
        scoreManager = GetNode<ScoreManager>(scoreManagerNodePath);

        animation = GetNode<AnimatedSprite2D>("sprite");
        animation.Play("idle");

        // Signal of OnBodyEntered
        this.BodyEntered += OnBodyEntered;

        // Get item type based on the parent node
        itemType = this.GetParent().Name;
    }

    // Godot signal when body enters area2D
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
        switch (itemType) // Logic based on type of item
        {
            case "items_coffee":
                scoreManager.IncreaseCoffeeScore(coffeeScore);
                break;
            case "items_coffee_pot":
                scoreManager.IncreaseCoffeeScore(coffeePotScore);
                break;
        }
    }
}
