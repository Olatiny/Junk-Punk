using Godot;
using Godot.Collections;
using System;
using System.Collections;

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
	Vector2I cameraStartPos;
	[Export] int cameraLerpDistance;
	[Export] Sprite2D background;
	Vector2I backgroundStartPos;
	[Export] int backgroundLerpDist;

	[ExportCategory("UI")]
	[ExportGroup("Control Groups")]
	[Export] Control playerUI;
	[Export] Control setupUI;
	[Export] Control actionsUI;
	[Export] Control pausedCtrl;
	[Export] Control gameOverCtrl;
	[Export] ShopCollection shopPanel;
	[Export] ScoreBoard scoreBoard;
	[Export] public Control tooltip;

	[ExportGroup("Buttons")]
	[Export] Button movementButton;
	[Export] Button actionButton;
	[Export] Button endTurnButton;

	[ExportGroup("Text")]
	[Export] RichTextLabel pausedText;
	[Export] RichTextLabel gameOverText;

	public int round { get; private set; } = 1;

	int currentPlayerIdx = 0;
	public bool movementUsed { get; private set; } = false;
	public bool actionUsed { get; private set; } = false;

	public override void _Ready()
	{
		base._Ready();

		gameState = GameState.Playing;
		turnPhase = TurnPhase.Upkeep;
		cameraStartPos = (Vector2I) camera.Position;
		backgroundStartPos = (Vector2I) background.Position;

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
				playerUI.SetPosition(playerUI.Position.MoveToward(new(0, playerUI.Position.Y), 5));
				camera.Position = new Vector2((float)Mathf.Lerp(camera.Position.X, cameraStartPos.X, delta * 4), camera.Position.Y);
				background.Position = new Vector2((float)Mathf.Lerp(background.Position.X, backgroundStartPos.X, delta * 4), background.Position.Y);
			}
			else
			{
				playerUI.SetPosition(playerUI.Position.MoveToward(new(-90, playerUI.Position.Y), 5));
				camera.Position = new Vector2((float)Mathf.Lerp(camera.Position.X, cameraStartPos.X + cameraLerpDistance, delta * 4), camera.Position.Y);
				background.Position = new Vector2((float)Mathf.Lerp(background.Position.X, backgroundStartPos.X + backgroundLerpDist, delta * 4), background.Position.Y);
			}
		}

		if (turnPhase == TurnPhase.Upkeep)
			DoUpkeep();
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

		Globals globals = GetNode<Globals>("/root/Globals");
		globals.EmitSignal(Globals.SignalName.Upkeep, GetCurrentPlayer());

		if (GetCurrentPlayer().health <= 0)
		{
			// Go back, last player killed 'em.
			currentPlayerIdx--;
			if (currentPlayerIdx < 0)
				currentPlayerIdx = players.Count - 1;

			DeclareVictory();
			return;
		}

		if (movementButton != null)
			movementButton.Disabled = false;

		if (actionButton != null)
			actionButton.Disabled = false;

		players?[currentPlayerIdx].CheckModDurability();

		if (players != null && players[currentPlayerIdx] != null)
		{
			players[currentPlayerIdx].currentScrap += players[currentPlayerIdx].scrapIncome;
		}

		DropScrap();
		ShuffleShop();
		UpdateScoreBoard();
		setupUI.Show();
		actionsUI.Hide();
		turnPhase = TurnPhase.Setup;
	}

	public void DropScrap()
	{
		board.CheckScrapDurabilities();
		board.DropScrap();

		if (round == 1) // Don't drop scrap during first round.
			return;

		if ((round * 2 + currentPlayerIdx) % 3 == 0)
			board.GenerateNextScrapTiles();
	}

	public void ShuffleShop()
	{
		shopPanel?.RandomizeShop();
	}

	public void EndSetup()
	{
		if (turnPhase == TurnPhase.Setup)
		{
			setupUI.Hide();
			actionsUI.Show();
			turnPhase = TurnPhase.Actions;
		}
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
		setupUI.Show();
		actionsUI.Hide();
		turnPhase = TurnPhase.Upkeep;
	}

	public PlayerController GetCurrentPlayer()
	{
		return players?[currentPlayerIdx];
	}

	public void UpdateScoreBoard()
	{
		scoreBoard.UpdateScoreBoard(currentPlayerIdx, players);
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
		GetNode<Globals>("/root/Globals").victoryIndex = currentPlayerIdx;
		gameState = GameState.GameOver;
		GetTree().ChangeSceneToFile("Scenes/VictoryScreen.tscn");
		// // gameOverText.Text = $"[center]Player {currentPlayerIdx + 1} Wins!\n\n\nGame Over[/center]";s
		// gameOverCtrl.Visible = true;
		// pausedCtrl.Visible = playerUI.Visible = false;
	}

	public void ReturnToMain()
	{
		GetTree().ChangeSceneToFile("Scenes/MainMenu.tscn");
	}

	public void Restart()
	{
		GetTree().ChangeSceneToFile("Scenes/InventoryShop.tscn");
	}

	public void ChangeActiveEquip(int attackIdx)
	{
		players?[currentPlayerIdx].SetActiveAttackMod(attackIdx);
	}
}
