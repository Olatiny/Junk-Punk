using Godot;
using Godot.Collections;
using System;

public partial class GameManager : Node
{
	public enum GameState { Playing, Paused, GameOver };

	public enum TurnPhase { Upkeep, Setup, Actions };

	public GameState gameState { get; private set; }
	public TurnPhase turnPhase { get; private set; }

	[ExportSubgroup("Player Info")]
	[Export] public Array<PlayerController> players;
	[Export] public ChessBoard board;

	[ExportSubgroup("Level Objects")]
	[Export] Camera2D camera;
	[Export] Vector2I cameraStartPos;
	[Export] int cameraLerpDistance;

	[ExportCategory("UI")]
	[ExportGroup("Control Groups")]
	[Export] Control playerUI;
	[Export] Control pausedCtrl;
	[Export] Control gameOverCtrl;

	[ExportGroup("Buttons")]
	[Export] Button movementButton;
	[Export] Button actionButton;
	[Export] Button endTurnButton;

	[ExportGroup("Text")]
	[Export] RichTextLabel pausedText;
	[Export] RichTextLabel gameOverText;

	[ExportGroup("Scoreboard")]
	[Export] TextureRect turnIndicator;
	[Export] TextureRect leftOnes;
	[Export] TextureRect leftTens;
	[Export] TextureRect rightOnes;
	[Export] TextureRect rightTens;

	public int round { get; private set; } = 1;

	int currentPlayerIdx = 0;
	public bool movementUsed { get; private set; } = false;
	public bool actionUsed { get; private set; } = false;

	public override void _Ready()
	{
		base._Ready();

		gameState = GameState.Playing;
		turnPhase = TurnPhase.Upkeep;

		if (playerUI != null && pausedCtrl != null && gameOverCtrl != null)
		{
			playerUI.Visible = true;
			pausedCtrl.Visible = gameOverCtrl.Visible = false;
		}

		UpdateScoreBoard();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (gameState != GameState.Playing)
			return;

		if (playerUI != null)
		{
			bool player1turn = currentPlayerIdx == 0;

			if (player1turn)
			{
				playerUI.SetPosition(playerUI.Position.MoveToward(new (0, playerUI.Position.Y), 5));
				// camera.Position = camera.Position.MoveToward(cameraStartPos, 5);
				// playerUI.SetPosition(new Vector2((float) Mathf.Lerp(playerUI.Position.X, 0, delta * 4), playerUI.Position.Y));
				camera.Position = new Vector2((float) Mathf.Lerp(camera.Position.X, cameraStartPos.X, delta * 4), camera.Position.Y);
			}
			else
			{
				playerUI.SetPosition(playerUI.Position.MoveToward(new(-90, playerUI.Position.Y), 5));
				// camera.Position = camera.Position.MoveToward(cameraStartPos + new Vector2(cameraLerpDistance, 0), 5);
				// playerUI.SetPosition(new Vector2((float) Mathf.Lerp(playerUI.Position.X, -90, delta * 4), playerUI.Position.Y));
				camera.Position = new Vector2((float) Mathf.Lerp(camera.Position.X, cameraStartPos.X + cameraLerpDistance, delta * 4), camera.Position.Y);
			}
		}

		switch (turnPhase)
		{
			case TurnPhase.Upkeep:
				DoUpkeep();
				break;
			case TurnPhase.Setup:
				DoSetup();
				break;
			case TurnPhase.Actions:
				DoActions();
				break;
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
   
		if (Input.IsKeyPressed(Key.Escape))
			Pause();
	}

	/**
	 * Handles everything needed to do for upkeep. Should only take 1 loop.
	 */
	public void DoUpkeep()
	{
		movementUsed = actionUsed = false;

		if (movementButton != null)
			movementButton.Disabled = false;

		if (actionButton != null)
			actionButton.Disabled = false;

		players?[currentPlayerIdx].CheckModDurability();
		DropScrap();
		ShuffleShop();
		turnPhase = TurnPhase.Setup;
	}

	public void DropScrap()
	{
		// TODO: Implement this 
	}

	public void ShuffleShop()
	{
		// TODO: Implement this 
	}

	/**
	 * Code for setup phase during loop. This will run until the player discretely moves on to turn phase.
	 */
	public void DoSetup()
	{
		// TODO: Decide if this is actually necessary.
	}

	/**
	 * Code for action phase during loop. This will run until the player ends their turn.
	 */
	public void DoActions()
	{
		// TODO: Decide if this is actually necessary.
	}

	public bool IsPlayerTurn(PlayerController player)
	{
		return players[currentPlayerIdx] == player;
	}

	public void GetValidMoveTiles()
	{
		board.GetValidMoves(players[currentPlayerIdx]);
		players[currentPlayerIdx].PrimeToMove();
	}

	public void GetValidAttackTiles()
	{
		board.GetValidAttacks(players[currentPlayerIdx]);
		players[currentPlayerIdx].PrimeToAttack();
	}

	public void PlayerMoved()
	{
		movementUsed = true;
		movementButton.Disabled = true;

		UpdateScoreBoard();

		// if (actionUsed)
		// 	EndTurn();
	}

	public void PlayerActioned()
	{
		actionUsed = true;
		actionButton.Disabled = true;

		UpdateScoreBoard();
		// if (movementUsed)
		// 	EndTurn();
	}

	public void EndTurn()
	{
		players[currentPlayerIdx].UnPrime();
		currentPlayerIdx++;
		if (currentPlayerIdx >= players?.Count)
			currentPlayerIdx = 0;

		if (currentPlayerIdx == 0)
			round++;

		board.ClearValidTiles();
		UpdateScoreBoard();
		turnPhase = TurnPhase.Upkeep;
	}

	public PlayerController GetCurrentPlayer()
	{
		return players?[currentPlayerIdx];
	}

	public void UpdateScoreBoard()
	{
		if (leftOnes == null || rightOnes == null || players == null || turnIndicator == null)
			return;

		turnIndicator.FlipH = currentPlayerIdx == 0;

		int player1hp = players[0].health;
		int player2hp = players[1].health;

		int leftOnesPlace  = player1hp % 10;
		int leftTensPlace = player1hp / 10;

		int rightOnesPlace = player2hp % 10;
		int rightTensPlace = player2hp / 10;

		((AtlasTexture)leftOnes.Texture).Region = new Rect2(leftOnesPlace * 15, 0, 15, 16);
		((AtlasTexture)leftTens.Texture).Region = new Rect2(leftTensPlace * 15, 0, 15, 16);

		((AtlasTexture)rightOnes.Texture).Region = new Rect2(rightOnesPlace * 15, 0, 15, 16);
		((AtlasTexture)rightTens.Texture).Region = new Rect2(rightTensPlace * 15, 0, 15, 16);
	}

	public void Pause()
	{
		gameState = GameState.Paused;
		if (pausedText != null)
			pausedText.Text = $"[center]Player {currentPlayerIdx + 1}'s turn\n\n\nPaused[/center]";
		pausedCtrl.Visible = true;
		playerUI.Visible = gameOverCtrl.Visible = false;
	}

	public void Resume()
	{
		gameState = GameState.Playing;
		playerUI.Visible = true;
		pausedCtrl.Visible = gameOverCtrl.Visible = false;
	}

	public void DeclareVictory()
	{
		// TODO: GameOver victory/defeat screen!
		gameState = GameState.GameOver;
		gameOverText.Text = $"[center]Player {currentPlayerIdx + 1} Wins!\n\n\nGame Over[/center]";
		gameOverCtrl.Visible = true;
		pausedCtrl.Visible = playerUI.Visible = false;
	}

	public void ReturnToMain()
	{
		GetTree().ChangeSceneToFile("Scenes/MainMenu.tscn");
	}

	public void Restart()
	{
		GetTree().ChangeSceneToFile("Scenes/GameTestScene.tscn");
	}
}
