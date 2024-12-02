using Godot;
using CoffeeCatProject.GlobalScripts;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class EnemiesOverlord : Node
{
	private Vector2 _playerPosition;

	public override void _Ready()
	{
		// Connect to player variables signal
		PlayerVariables.Instance.PlayerPositionUpdated += OnPlayerPositionUpdated;
	}

	// Get the player's global position
	private void OnPlayerPositionUpdated()
	{
		_playerPosition = PlayerVariables.Instance.PlayerPosition;
		GD.Print("Enemy sees Player position as: " + _playerPosition);
	}
}

