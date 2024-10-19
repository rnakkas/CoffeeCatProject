using Godot;
using System;

public partial class Items : Area2D
{
	private void PlayerCollectsItem(Node2D objectNode, AnimatedSprite2D animation, Node levelManager)
    {
        IncreaseScore(objectNode, levelManager);

        Tween Tween_1 = objectNode.GetTree().CreateTween();
        Tween Tween_2 = objectNode.GetTree().CreateTween();

        //animation.Play("collect");

        Tween_1.TweenProperty(objectNode, "modulate:a", 0, 0.4);
        Tween_2.TweenProperty(objectNode, "position", objectNode.Position - new Vector2(0, 50), 0.5);
        Tween_2.TweenCallback(Callable.From(objectNode.QueueFree));
    }

    private void IncreaseScore(Node2D objectNode, Node levelManager)
    {
        switch (objectNode.identifier)
        {
            case "coffee":
                levelManager.IncreaseCoffeeScore(1);
                break;
            case "coffee_pot":
                levelManager.IncreaseCoffeeScore(5); 
                break;
        }
    }
}
