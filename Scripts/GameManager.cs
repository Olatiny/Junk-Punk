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

	[ExportSubgroup("UI")]
	[Export] Button movementButton;
	[Export] Button actionButton;
	[Export] Button endTurnButton;
	[Export] RichTextLabel roundText;

	public int round { get; private set; } = 1;

	int currentPlayerIdx = 0;
	public bool movementUsed { get; private set; } = false;
	public bool actionUsed { get; private set; } = false;

	public override void _Ready()
	{
		base._Ready();

		gameState = GameState.Playing;
		turnPhase = TurnPhase.Upkeep;

		roundText.Text = $"Player: {players?[currentPlayerIdx].playerId}\nRound: {round}";
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (gameState != GameState.Playing)
			return;

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

	public void PlayerMoved()
	{
		movementUsed = true;
		movementButton.Disabled = true;

		if (actionUsed)
			EndTurn();
	}

	public void PlayerActioned()
	{
		actionUsed = true;
		actionButton.Disabled = true;

		if (movementUsed)
			EndTurn();
	}

	public void EndTurn()
	{
		currentPlayerIdx++;
		if (currentPlayerIdx >= players?.Count)
			currentPlayerIdx = 0;

		if (currentPlayerIdx == 0)
			round++;

		roundText.Text = $"Player: {players?[currentPlayerIdx].playerId}\nRound: {round}";
		turnPhase = TurnPhase.Upkeep;
	}
}
