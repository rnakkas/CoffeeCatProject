using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles dealing damage to health when hit and healing (for player only) when picking up health items.

[GlobalClass]
public partial class HealthComponent : Node2D
{
    [Export] public int MaxHealth;
    [Export] public int CurrentHealth;

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        GD.Print("Health: " + CurrentHealth);
    }

    public void Heal(int heal)
    {
        // Clamp the current health between 0 and max health
        CurrentHealth =  Mathf.Clamp(CurrentHealth + heal, 0, MaxHealth);
        GD.Print("Health: " + CurrentHealth);
    }
}
