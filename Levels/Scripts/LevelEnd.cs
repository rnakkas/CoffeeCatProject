using Godot;
using System;

public partial class LevelEnd : Area2D
{
	AnimatedSprite2D animation;

	bool playerEntered;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animation = GetNode<AnimatedSprite2D>("sprite");
		animation.Play("idle");

		// Signals
		this.BodyEntered += OnBodyEntered;
		this.BodyExited += OnBodyExited;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        PlayerInteraction();
    }

    //Signal
    private void OnBodyEntered(Node2D body)
    {
        if (body.Name == "player")
		{
			playerEntered = true;
			animation.Play("player_entered");
		}
    }

	// Signal
    private void OnBodyExited(Node2D body)
    {
		if (body.Name == "player")
		{
			playerEntered = false;
			animation.Play("idle");
		}
    }

    private void PlayerInteraction()
    {
       if (playerEntered && Input.IsActionJustPressed("move_up"))
		{
			GD.Print("entered");
			///TODO: scene transition to level complete scene
		}
    }
}