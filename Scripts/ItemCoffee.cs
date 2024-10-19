using Godot;
using System;

public partial class ItemCoffee : Items
{
	AnimatedSprite2D animation;
	Node levelManager;

	String identifier;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		identifier = "coffee";
		animation = GetNode<AnimatedSprite2D>("sprite");
		animation.Play("idle");
		
		// Connect signal
        Connect("body_entered", this, nameof(OnBodyEntered));

    }

    private object OnBodyEntered(Node2D body)
    {
        if (body.Name == "player")
		{
			
		}
				
    }
}
