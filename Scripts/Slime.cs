using Godot;
using System;

public partial class Slime : Area2D
{
	[Export]
	public float slimeSpeed = -50;

	public AnimatedSprite2D slimeAnimated;
	public CollisionShape2D slimeCollision;
	private AudioStreamPlayer slimeDeathSound;

	private GameManager gameManager;

	private bool isDead = false;

	public override void _Ready()
	{
		slimeAnimated = GetNode<AnimatedSprite2D>("SlimeAnimatedSprite");
		slimeCollision = GetNode<CollisionShape2D>("SlimeCollisionShape");
		slimeDeathSound = GetNode<AudioStreamPlayer>("SlimeDeathSound");

		gameManager = (GameManager)GetTree().CurrentScene;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Position.X < -245)
		{
			QueueFree();
		}

		// 怪物死亡，取消移动
		if (isDead)
		{
			return;
		}

		Position = Position with
		{
			X = Position.X + slimeSpeed * (float)delta
		};
	}

	// 碰撞到子弹(子弹类型<Area2D>)
	public void OnAreaExtered(Area2D area)
	{
		if (area.IsInGroup("bullet"))
		{
			slimeAnimated.Play("death");
			slimeDeathSound.Play();
			isDead = true;

			gameManager.AddScore(1);

			// 摧毁子弹
			area.QueueFree();
			// 将怪物的碰撞墙销毁，避免后面的子弹无法通过该区域
			slimeCollision.QueueFree();
		}
	}

	// 播放完动画后执行
	public void OnAnimatedSpriteFinished()
	{
		// 判断当前播放的动画是否为die，如果是就播放完成后销毁该对象
		if (slimeAnimated.Animation == "death")
		{
			QueueFree();
		}
	}

	// 碰撞到玩家(玩家类型<CharacterBody2D>)
	public void OnBodyExtered(Node2D body)
	{
		if (body is Player characterBody && !isDead)
		{
			characterBody.GameOver();
		}
	}
}
