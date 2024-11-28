using System;
using Godot;

namespace CoffeeCatProject.Items.Scripts;

public partial class Items : Area2D
{
    private AnimatedSprite2D _animation;
    private ScoreManager _scoreManager;

    private NodePath _scoreManagerNodePath = "../../score_manager";
    private int _coffeeScore = 1;
    private int _coffeePotScore = 10;

    private String _itemType;
    public override void _Ready()
    {
        // Get the score manager node
        _scoreManager = GetNode<ScoreManager>(_scoreManagerNodePath);

        _animation = GetNode<AnimatedSprite2D>("sprite");
        _animation.Play("idle");

        // Signal of OnBodyEntered
        this.BodyEntered += OnBodyEntered;

        // Get item type based on the parent node
        _itemType = this.GetParent().Name;
    }

    // Godot signal when body enters area2D
    private void OnBodyEntered(Node2D body)
    {
        if (body.Name.ToString().ToLower().Contains("player"))
        {
            PlayerCollectsItem();
        }
    }

    private void PlayerCollectsItem()
    {
        IncreaseScore();

        Tween tween1 = this.GetTree().CreateTween();
        Tween tween2 = this.GetTree().CreateTween();

        tween1.TweenProperty(this, "modulate:a", 0, 0.4);
        tween2.TweenProperty(this, "position", this.Position - new Vector2(0, 50), 0.5);
        tween2.TweenCallback(Callable.From(this.QueueFree));
    }

    private void IncreaseScore()
    {
        switch (_itemType) // Logic based on type of item
        {
            case "items_coffee":
                _scoreManager.IncreaseCoffeeScore(_coffeeScore);
                break;
            case "items_coffee_pot":
                _scoreManager.IncreaseCoffeeScore(_coffeePotScore);
                break;
        }
    }
}