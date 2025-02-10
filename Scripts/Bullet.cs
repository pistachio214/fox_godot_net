using Godot;
using System;

public partial class Bullet : Area2D
{
	private float bullet_speed = 200;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{

		// 超出次范围就销毁
		if (Position.X > 250)
		{
			QueueFree();
		}

		Position = Position with { X = Position.X + bullet_speed * (float)delta };

	}
}
