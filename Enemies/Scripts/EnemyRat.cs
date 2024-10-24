namespace CoffeeCatProject.Enemies.Scripts;

public partial class EnemyRat : Enemies
{
	public override void _Ready()
	{
		Animation = GetSprite();
		SetState(State.Idle);
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateState((float)delta);
	}
}