using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles dealing damage to health when hit and healing (for player only) when picking up health items.

[GlobalClass]
public partial class HealthComponent : Node2D
{
    
    [Export] private int _health;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        GD.Print("Health: " + _health);
    }

    public void Heal(int heal)
    {
        if (_health + heal >= _health)
            return;
        
        _health += heal;
        GD.Print("Health: " + _health);
    }
}
