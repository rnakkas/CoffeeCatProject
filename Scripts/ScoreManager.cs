using Godot;
using System;

public partial class ScoreManager : Node
{
    public int score = 0;

    public void IncreaseCoffeeScore(int count)
    {
        score += count;
        GD.Print("Score: " + score);
    }
}
