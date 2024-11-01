using Godot;

namespace CoffeeCatProject.Items.Scripts;

public partial class ScoreManager : Node
{
    public int Score;

    public void IncreaseCoffeeScore(int count)
    {
        Score += count;
        GD.Print("Score: " + Score);
    }
}