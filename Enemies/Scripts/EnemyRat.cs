using Godot;
using System;

public partial class EnemyRat : Enemies
{
    public override void _Ready()
    {
		animation = GetSprite();
		SetState(State.IDLE);
    }

    public override void _PhysicsProcess(double delta)
	{
		UpdateState((float)delta);
	}
}
