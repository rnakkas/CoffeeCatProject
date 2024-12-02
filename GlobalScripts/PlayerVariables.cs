using Godot;

namespace CoffeeCatProject.GlobalScripts;
public partial class PlayerVariables: Node
{
    public static PlayerVariables Instance { get; private set; }
    public Vector2 PlayerPosition {get; private set;}
    
    [Signal] public delegate void PlayerPositionUpdatedEventHandler();
    
    public void UpdatePlayerPosition(Vector2 position)
    {
        PlayerPosition =  position;
        EmitSignal(SignalName.PlayerPositionUpdated);
    }
    
    public override void _Ready()
    {
        Instance = this;
    }
    
}