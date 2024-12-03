using Godot;

namespace CoffeeCatProject.GlobalScripts;
public partial class PlayerVariables: Node
{
    public static PlayerVariables Instance { get; private set; }
    public Vector2 PlayerGlobalPosition {get; private set;}
    
    [Signal] public delegate void PlayerGlobalPositionUpdatedEventHandler();
    
    public void UpdatePlayerGlobalPosition(Vector2 globalPosition)
    {
        PlayerGlobalPosition =  globalPosition;
        EmitSignal(SignalName.PlayerGlobalPositionUpdated);
    }
    
    public override void _Ready()
    {
        Instance = this;
    }
    
}