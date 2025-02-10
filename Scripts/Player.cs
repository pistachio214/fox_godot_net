using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	[Export]
	public float speed = 100;

	[Export]
	public PackedScene bulletScene;

	// private NodePath playerAnimatedPath;
	// 玩家动画控制
	private AnimatedSprite2D playerAnimated;
	// 玩家开枪音效
	private AudioStreamPlayer playerFireSound;
	// 玩家跑步音效
	private AudioStreamPlayer playerRunningSound;

	// 玩家跑步音效
	private AudioStreamPlayer gameOverSound;

	private Timer restartTimer;

	// 玩家开火控制
	private bool _isActionKeyPressedFire = false;
	// 判定玩家是否输掉游戏
	private bool _isGameOver = false;

	private GameManager gameManager;

	public override void _Ready()
	{
		playerAnimated = GetNode<AnimatedSprite2D>("PlayerAnimated");
		playerFireSound = GetNode<AudioStreamPlayer>("PlayerFireSound");
		playerRunningSound = GetNode<AudioStreamPlayer>("PlayerRunningSound");
		gameOverSound = GetNode<AudioStreamPlayer>("GameOverSound");

		restartTimer = GetNode<Timer>("RestartTimer");

		gameManager = (GameManager)GetTree().CurrentScene;
	}

	public override void _Process(double delta)
	{
		if (Velocity == Vector2.Zero || _isGameOver)
		{
			playerRunningSound.Stop();
		}
		else if (!playerRunningSound.Playing)
		{
			playerRunningSound.Play();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isGameOver)
		{
			return;
		}

		Velocity = Input.GetVector("left", "right", "up", "down") * speed;

		if (Velocity == Vector2.Zero)
		{
			playerAnimated.Play("idel");
		}
		else
		{
			playerAnimated.Play("run");
		}


		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		// 检查事件是否是按键事件
		if (@event is InputEventKey keyEvent)
		{
			// 检查是否是特定的动作键
			if (keyEvent.Keycode == Key.J)
			{
				// 如果按键被按下
				if (keyEvent.Pressed)
				{
					// 并且之前没有被按下过（避免连续触发）
					if (!_isActionKeyPressedFire)
					{
						_isActionKeyPressedFire = true; // 更新状态
						OnFire(); // 执行动作
					}
				}
				// 如果按键被释放
				else
				{
					_isActionKeyPressedFire = false; // 重置状态，为下一次按下做准备
				}
			}
		}
	}

	// 玩家按键开火
	private void OnFire()
	{
		// 实例化一个新的子弹对象
		var bulletNode = bulletScene.Instantiate<Area2D>();

		// 播放开枪音效
		playerFireSound.Play();

		// 将玩家的位置赋予子弹(给子弹添加一点便宜,使子弹从枪管发射)
		bulletNode.Position = Position with { X = Position.X + 16, Y = Position.Y + 6 };

		GetTree().CurrentScene.AddChild(bulletNode);
	}

	public void GameOver()
	{
		if (!_isGameOver)
		{
			_isGameOver = true;

			gameManager.ShowGameOver();

			playerAnimated.Play("death");
			gameOverSound.Play();

			restartTimer.Start();
		}
	}

	public void _ReloadScene()
	{
		GetTree().ReloadCurrentScene();
	}
}
