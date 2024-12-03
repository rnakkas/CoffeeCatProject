using Godot;
using CoffeeCatProject.GlobalScripts;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class EnemiesOverlord : Node
{
	private Vector2 _playerGlobalPosition;

	public override void _Ready()
	{
		// Connect to player variables signal
		PlayerVariables.Instance.PlayerGlobalPositionUpdated += OnPlayerGlobalPositionUpdated;
	}

	// Get the player's global position
	private void OnPlayerGlobalPositionUpdated()
	{
		_playerGlobalPosition = PlayerVariables.Instance.PlayerGlobalPosition;
		// GD.Print("Enemy sees Player position as: " + _playerGlobalPosition);
	}
	
	//TODO: Spawn enemies near player global position
	
}

