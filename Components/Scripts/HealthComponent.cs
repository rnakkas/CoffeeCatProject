using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

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
        _health += heal;
        GD.Print("Health: " + _health);
    }
}
