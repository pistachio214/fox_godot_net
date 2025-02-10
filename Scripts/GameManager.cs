using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export]
	public PackedScene slimeScene;

	[Export]
	public int score = 0;

	private Label scoreLabel;
	private Label gameOverLabel;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("CanvasLayer/ScoreLabel");
		gameOverLabel = GetNode<Label>("CanvasLayer/GameOverLabel");

		if (gameOverLabel != null)
		{
			gameOverLabel.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// 每一帧都处理分数
		scoreLabel.Text = "Score: " + score;
	}

	public void AddScore(int i)
	{
		score += i;
	}

	public void ShowGameOver()
	{
		if (gameOverLabel != null)
		{
			gameOverLabel.Visible = true;
		}
	}

	// 链接生成怪物slime
	public void _SpawnSlime()
	{
		Area2D slimeNode = slimeScene.Instantiate<Area2D>();

		slimeNode.Position = Position with
		{
			X = 250,
			Y = new Random().Next(40, 101)
		};

		GetTree().CurrentScene.AddChild(slimeNode);
	}
}
