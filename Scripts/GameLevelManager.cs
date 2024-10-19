using Godot;
using System;

public partial class GameLevelManager : Node
{
    int coffeeScore = 0;

    public void IncreaseCoffeeScore(int count)
    {
        coffeeScore += count;
        GD.Print("Score: " + coffeeScore);
    }

 //   // Called when the node enters the scene tree for the first time.
 //   public override void _Ready()
	//{
	//}

	
}
