using Godot;
using System;

public partial class Hud : CanvasLayer
{
	Label score;
	ScoreManager scoreManager;

	private NodePath scoreManagerNodePath = "../score_manager";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		score = GetNode<Label>("coffee_score");
		scoreManager = GetNode<ScoreManager>(scoreManagerNodePath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		score.Text = scoreManager.score.ToString();
	}
}
