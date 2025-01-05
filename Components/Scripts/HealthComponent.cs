using CoffeeCatProject.GlobalScripts;
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

    public void Heal(PickupItemsComponent pickupItemsComponent)
    {
        // If current health is less than max health, pick up the health item
        if (CurrentHealth < MaxHealth)
        {
            pickupItemsComponent.ItemGetsPickedUp();
        }
        
        // If current health exceeds max health after heal, set the current health to max health
        CurrentHealth += pickupItemsComponent.HealAmount;
        CurrentHealth =  Mathf.Min(CurrentHealth, MaxHealth);
        GD.Print("Health: " + CurrentHealth);
    }
}
