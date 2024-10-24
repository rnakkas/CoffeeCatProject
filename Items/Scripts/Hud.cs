using Godot;

namespace CoffeeCatProject.Items.Scripts;

public partial class Hud : CanvasLayer
{
	private Label _score;
	private ScoreManager _scoreManager;

	private NodePath _scoreManagerNodePath = "../score_manager";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_score = GetNode<Label>("coffee_score");
		_scoreManager = GetNode<ScoreManager>(_scoreManagerNodePath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_score.Text = _scoreManager.Score.ToString();
	}
}