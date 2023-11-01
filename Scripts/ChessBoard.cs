using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ChessBoard : TileMap
{
	public Node2D[,] Grid { get; private set; }

	[ExportCategory("Chess Board")]
	[ExportSubgroup("Grid Definitions")]
	[Export] public Vector2I gridSize = new(8, 8);
	[Export] public Vector2I tileSize = new(16, 13);
	[Export] public Vector2I tileOffset = new(8, 6);

	GameManager gameManager;

	public HashSet<Vector2I> validBasicTileCoords { get; private set; }
	public HashSet<Vector2I> validModTileCoords { get; private set; }
	public HashSet<Vector2I> rotatedAOECoords { get; private set; }
	Vector2I mouseOverCell;

	public PackedScene scrapPrefab;
	public HashSet<Vector2I> queuedScrap { get; private set; }
	public HashSet<Scrap> queuedPlayerSummonedScrap { get; private set; }
	public Array<Scrap> placedScrap { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		gameManager = GetParent<GameManager>();
		Grid = new Node2D[gridSize.X, gridSize.Y];
		validBasicTileCoords = new HashSet<Vector2I>();
		validModTileCoords = new HashSet<Vector2I>();
		rotatedAOECoords = new HashSet<Vector2I>();

		scrapPrefab = GD.Load<PackedScene>("res://Prefabs (wink)/Scrap.tscn");
		queuedScrap = new();
		queuedPlayerSummonedScrap = new();
		placedScrap = new();

		foreach (PlayerController player in gameManager.players)
		{
			SetNodeGridPosition(player, player.gridPosition);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		UpdateMouseOverHighlight();

		PlayerController player = gameManager.GetCurrentPlayer();
		if (player.primedToAttack && player?.GetActiveAttackMod()?.attackType == ArmMod.AttackType.aoe)
			UpdateAOEOrientation(player);
	}

	// Orients the AOE towards the mouse.	
	private void UpdateAOEOrientation(PlayerController player)
	{
		Vector2 playerWorldPos = player.Position;
		Vector2I pivot = player.gridPosition;
		Vector2 mousePos = GetLocalMousePosition() - playerWorldPos;
		float angle = (float)Math.Atan2(mousePos.Y - Vector2.Up.Y, mousePos.X - Vector2.Up.X) + (Mathf.Pi / 2.0f);

		angle = Mathf.Round(angle / (Mathf.Pi / 2.0f)) * (Mathf.Pi / 2.0f);

		float sin = (float)Math.Sin(angle);
		float cos = (float)Math.Cos(angle);

		foreach (Vector2I tile in rotatedAOECoords)
		{
			EraseCell(1, tile);
		}

		rotatedAOECoords.Clear();

		foreach (Vector2I tile in validModTileCoords)
		{
			// Vector2 worldSpaceTile = GetTileWorldPosition(tile);
			// worldSpaceTile -= playerWorldPos;
			// Vector2 newVec = new(worldSpaceTile.X * cos - worldSpaceTile.Y * sin, worldSpaceTile.X * sin + worldSpaceTile.Y * cos);
			// worldSpaceTile = newVec + playerWorldPos;
			// Vector2I rotatedTileCoords = LocalToMap(worldSpaceTile);
			Vector2 tilePos = tile;
			tilePos -= pivot;
			Vector2 newVec = new(tilePos.X * cos - tilePos.Y * sin, tilePos.X * sin + tilePos.Y * cos);
			newVec += pivot;
			Vector2I rotatedTileCoords = new(Mathf.RoundToInt(newVec.X), Mathf.RoundToInt(newVec.Y));
			if (IsTileInBounds(rotatedTileCoords))
				rotatedAOECoords.Add(rotatedTileCoords);
		}

		foreach (Vector2I tile in validModTileCoords)
		{
			EraseCell(1, tile);
		}

		foreach (Vector2I tile in rotatedAOECoords)
		{
			SetCell(1, tile, 6, new Vector2I(0, 0), 1);
		}

		switch (angle)
		{
			case 0:
				player.UpdateDirection(PlayerController.Direction.up);
				break;
			case Mathf.Pi / 2.0f:
				player.UpdateDirection(PlayerController.Direction.right);
				break;
			case Mathf.Pi:
				player.UpdateDirection(PlayerController.Direction.down);
				break;
			case Mathf.Pi * 3.0f / 2.0f:
				player.UpdateDirection(PlayerController.Direction.left);
				break;
		}
	}

	public void GetValidMoves(PlayerController player)
	{
		ClearValidTiles();

		GetValidBasicMoveTiles(player);
		GetValidModMoveTiles(player);

		HighlightValidMoveTiles();
	}

	private void GetValidBasicMoveTiles(PlayerController player)
	{
		//left
		if (IsTileAvailable(player.gridPosition - new Vector2I(1, 0)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(1, 0))));

		//right
		if (IsTileAvailable(player.gridPosition + new Vector2I(1, 0)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(1, 0))));

		//bottom
		if (IsTileAvailable(player.gridPosition + new Vector2I(0, 1)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(0, 1))));

		//top
		if (IsTileAvailable(player.gridPosition - new Vector2I(0, 1)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(0, 1))));
	}

	private void GetValidModMoveTiles(PlayerController player)
	{
		foreach (Mod mod in player.legMods)
		{
			if (mod is not LegMod)
				continue;
			
			if (mod == null)
				continue;

			Array<Vector2I> modTiles = ((LegMod)mod)?.GetValidMoveTiles(this, player);

			if (modTiles != null)
				for (int i = 0; i < modTiles.Count; validModTileCoords.Add(modTiles[i]), i++) ;
		}
	}


	public void GetValidAttacks(PlayerController player)
	{
		ClearValidTiles();

		if (player.GetActiveAttackMod() != null)
			GetValidModAttackTiles(player);
		else
			GetValidBasicAttackTiles(player);

		// GetValidBasicAttackTiles(player);
		// GetValidModAttackTiles(player);

		HighlightValidAttackTiles();
	}

	public void GetValidBasicAttackTiles(PlayerController player)
	{
		//left
		// SummonScrap(player, player.gridPosition - new Vector2I(1, 0));
		if (IsTileAttackable(player.gridPosition - new Vector2I(1, 0)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(1, 0))));

		//right
		if (IsTileAttackable(player.gridPosition + new Vector2I(1, 0)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(1, 0))));

		//bottom
		if (IsTileAttackable(player.gridPosition + new Vector2I(0, 1)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(0, 1))));

		//top
		if (IsTileAttackable(player.gridPosition - new Vector2I(0, 1)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(0, 1))));
	}

	public void GetValidModAttackTiles(PlayerController player)
	{
		Array<Vector2I> modTiles = player?.GetActiveAttackMod()?.GetValidAttackTiles(this, player);

		if (modTiles != null)
			for (int i = 0; i < modTiles.Count; validModTileCoords.Add(modTiles[i]), i++) ;
	}

	private void HighlightValidMoveTiles()
	{
		foreach (Vector2I tile in validModTileCoords)
		{
			SetCell(1, tile, 6, new Vector2I(0, 0), 2);
		}

		foreach (Vector2I tile in validBasicTileCoords)
		{
			SetCell(1, tile, 6, new Vector2I(0, 0), 2);
		}
	}

	private void HighlightValidAttackTiles()
	{
		foreach (Vector2I tile in validModTileCoords)
		{
			SetCell(1, tile, 6, new Vector2I(0, 0), 1);
		}

		foreach (Vector2I tile in validBasicTileCoords)
		{
			SetCell(1, tile, 6, new Vector2I(0, 0), 3);
		}
	}

	public void ClearValidTiles()
	{
		foreach (Vector2I tile in validModTileCoords)
		{
			EraseCell(1, tile);
		}

		foreach (Vector2I tile in validBasicTileCoords)
		{
			EraseCell(1, tile);
		}

		foreach (Vector2I tile in rotatedAOECoords)
		{
			EraseCell(1, tile);
		}

		validBasicTileCoords.Clear();
		validModTileCoords.Clear();
		rotatedAOECoords.Clear();
	}

	private void UpdateMouseOverHighlight()
	{
		if (mouseOverCell != null)
			EraseCell(3, mouseOverCell);
		mouseOverCell = LocalToMap(GetGlobalMousePosition());
		TileData tileData = GetCellTileData(0, mouseOverCell);
		if (gameManager?.gameState == GameManager.GameState.Playing && tileData != null && tileData.GetCustomData("highlightable").AsBool() && !Input.IsMouseButtonPressed(MouseButton.Left))
			SetCell(3, mouseOverCell, 6, new Vector2I(0, 0), 4);
	}

	public bool RequestMove(PlayerController player, Vector2 mouseCoordinates)
	{
		Vector2I originalPos = player.gridPosition;

		GameManager gameManager = GetParent<GameManager>();
		if (!gameManager.IsPlayerTurn(player))
			return false;

		Vector2I mouseMapPos = LocalToMap(mouseCoordinates);

		foreach (Mod mod in player.legMods)
		{
			if (mod?.buffType == Mod.BuffType.Movement)
			{
				LegMod legMod = mod as LegMod;
	
				if (legMod == null)
					continue;
					
				if (legMod.RequestMove(this, player, mouseMapPos))
				{
					ClearValidTiles();
					player.UpdateDirection(originalPos, player.gridPosition);
					gameManager.PlayerMoved();
					return true; // Note: True!
				}
			}
		}

		foreach (Vector2I validTile in validBasicTileCoords)
		{
			if (mouseMapPos == validTile)
			{
				SetNodeGridPosition(player, player.gridPosition, mouseMapPos);
				player.UpdateDirection(originalPos, player.gridPosition);
				ClearValidTiles();
				gameManager.PlayerMoved();
				return true;
			}
		}

		return false;
	}

	public bool RequestAttack(PlayerController player, Vector2 mouseCoordinates)
	{
		GameManager gameManager = GetParent<GameManager>();
		if (!gameManager.IsPlayerTurn(player))
			return false;

		Vector2I mouseMapPos = LocalToMap(mouseCoordinates);

		if (!IsTileInBounds(mouseMapPos))
			return false; // Note: False!

		if (player.GetActiveAttackMod() != null && player.GetActiveAttackMod().PerformAttack(this, player, mouseMapPos))
		{
			ClearValidTiles();
			gameManager.PlayerActioned();
			return true; // Note: True!
		}

		foreach (Vector2I validTile in validBasicTileCoords)
		{
			if (mouseMapPos == validTile)
			{
				Node2D node = Grid[validTile.X, validTile.Y];

				if (node is PlayerController enemy)
				{
					// returns true if enemy is killed, they still take damage if false
					if (enemy.TakeDamage(player.baseAttackDmg))
						gameManager.DeclareVictory();

					player.UpdateDirection(player.gridPosition, enemy.gridPosition);
				}

				if (node is Scrap scrap)
				{
					scrap.Harvest(player);
					player.UpdateDirection(player.gridPosition, scrap.gridPosition);
				}

				ClearValidTiles();
				gameManager.PlayerActioned();
				return true;
			}
		}

		return false;
	}

	public void GenerateNextScrapTiles()
	{
		ClearScrapTiles();

		ChooseNextScrapTiles();

		HighlightScrapTiles();
	}

	public void SummonScrap(PlayerController player, Vector2I tileLocation)
	{
		if (IsTileInBounds(tileLocation) && Grid?[tileLocation.X, tileLocation.Y] is not Scrap)
		{
			Scrap scrap = scrapPrefab.Instantiate() as Scrap;
			scrap.gridPosition = tileLocation;
			scrap.owner = player;
			queuedPlayerSummonedScrap.Add(scrap);
			RefreshPlayerScrapTiles();

			Globals globals = GetNode<Globals>("/root/Globals");
			globals.EmitSignal(Globals.SignalName.SummonedScrap, player, scrap);
		}
	}

	private void ChooseNextScrapTiles()
	{
		RandomNumberGenerator rand = new();
		rand.Randomize();
		Vector2I location1, location2;

		do
		{
			location1 = new(rand.RandiRange(0, gridSize.X - 1), rand.RandiRange(0, gridSize.Y) - 1);
		}
		while (!IsTileInBounds(location1) || Grid[location1.X, location1.Y] is Scrap);

		do
		{
			location2 = new(rand.RandiRange(0, gridSize.X - 1), rand.RandiRange(0, gridSize.Y) - 1);
		}
		while (!IsTileInBounds(location2) || Grid[location2.X, location2.Y] is Scrap || location2 == location1);

		queuedScrap.Add(location1);
		queuedScrap.Add(location2);

		HighlightScrapTiles();
	}

	private void HighlightScrapTiles()
	{
		foreach (Vector2I tile in queuedScrap)
		{
			SetCell(2, tile, 7, new Vector2I(0, 0));
		}
	}

	private void RefreshPlayerScrapTiles()
	{
		foreach (Scrap scrap in queuedPlayerSummonedScrap)
		{
			EraseCell(2, scrap.gridPosition);
		}

		foreach (Scrap scrap in queuedPlayerSummonedScrap)
		{
			SetCell(2, scrap.gridPosition, 7, new Vector2I(0, 0));
		}
	}

	private void ClearScrapTiles()
	{
		foreach (Vector2I tile in queuedScrap)
		{
			EraseCell(2, tile);
		}

		foreach (Scrap scrap in queuedPlayerSummonedScrap)
		{
			EraseCell(2, scrap.gridPosition);
		}

		queuedScrap.Clear();
		queuedPlayerSummonedScrap.Clear();
	}

	public void DropScrap()
	{
		foreach (Vector2I scrapTile in queuedScrap)
		{
			Scrap scrap = scrapPrefab.Instantiate() as Scrap;
			AddChild(scrap);
			placedScrap.Add(scrap);
			scrap.Drop(this, scrapTile);
		}

		foreach (Scrap scrap in queuedPlayerSummonedScrap)
		{
			AddChild(scrap);
			placedScrap.Add(scrap);
			scrap.Drop(this, scrap.gridPosition);
		}

		ClearScrapTiles();
	}

	public void CheckScrapDurabilities()
	{
		Array<Node2D> allNodes = GetAllNodesOnBoard();

		foreach (Node2D node in allNodes)
		{
			if (node is Scrap scrap)
				scrap.CheckDurability();
		}
	}

	/**
	 * If newLocation is null, just set position globally and ensure node is in grid.
	 * 
	 * if newLocatrion is not null, update the position of the node to reflect new position in grid and in world. 
	 * and, if it's a player, update it's gridPosition field.
	 */
	public void SetNodeGridPosition(Node2D node, Vector2I oldLocation, Vector2I? newLocation = null)
	{
		if (!newLocation.HasValue)
		{
			Grid[oldLocation.X, oldLocation.Y] = node;

			if (node is PlayerController player)
				player.gridPosition = oldLocation;

			if (node is Scrap scrap)
				scrap.gridPosition = oldLocation;
		}
		else
		{
			Grid[oldLocation.X, oldLocation.Y] = null;
			Grid[newLocation.Value.X, newLocation.Value.Y] = node;

			if (node is PlayerController player)
				player.gridPosition = newLocation.Value;

			if (node is Scrap scrap)
				scrap.gridPosition = newLocation.Value;
		}

		node.Position = newLocation.HasValue ? (newLocation.Value * tileSize) + tileOffset : (oldLocation * tileSize) + tileOffset;
	}

	public Array<Node2D> GetAllNodesOnBoard()
	{
		Array<Node2D> nodes = new();

		foreach (Node2D node in Grid)
		{
			if (node != null)
				nodes.Add(node);
		}

		return nodes;
	}

	private Vector2 GetTileWorldPosition(Vector2I cellLocation)
	{
		return cellLocation * tileSize + tileOffset;
	}

	public bool IsTileAvailable(Vector2I tileLocation)
	{
		if (!IsTileInBounds(tileLocation))
			return false;

		return Grid[tileLocation.X, tileLocation.Y] == null;
	}

	public bool IsTileAttackable(Vector2I tileLocation)
	{
		if (!IsTileInBounds(tileLocation))
			return false;

		// There's something there, therefore you can attack it!
		return Grid[tileLocation.X, tileLocation.Y] != null;
	}

	public bool IsTileInBounds(Vector2I tileLocation)
	{
		if (tileLocation.X < 0 || tileLocation.X >= gridSize.X)
			return false;

		if (tileLocation.Y < 0 || tileLocation.Y >= gridSize.Y)
			return false;

		return true;
	}

	public bool IsTileOccupied(Vector2I tileLocation)
	{
		if (!IsTileInBounds(tileLocation))
			return false;

		return Grid[tileLocation.X, tileLocation.Y] != null;
	}

	public Node2D GetNodeAtTile(Vector2I tileLocation)
	{
		if (!IsTileInBounds(tileLocation))
			return null;

		return Grid[tileLocation.X, tileLocation.Y];
	}

	public bool PlaceOnBoard(Node2D node, Vector2I tileLocation)
	{
		if (node is not PlayerController && node is not Scrap)
			return false;

		if (IsTileOccupied(tileLocation))
			return false;

		Grid[tileLocation.X, tileLocation.Y] = node;
		return true;
	}

	public bool RemoveFromBoard(PlayerController player)
	{
		if (!IsTileInBounds(player.gridPosition))
			return false;

		if (Grid[player.gridPosition.X, player.gridPosition.Y] == player)
		{
			Grid[player.gridPosition.X, player.gridPosition.Y] = null;
			player.QueueFree();
			return true;
		}

		return false;
	}

	public bool RemoveFromBoard(Scrap scrap)
	{
		if (!IsTileInBounds(scrap.gridPosition))
			return false;

		if (Grid[scrap.gridPosition.X, scrap.gridPosition.Y] == scrap)
		{
			Grid[scrap.gridPosition.X, scrap.gridPosition.Y] = null;
			scrap.QueueFree();
			return true;
		}

		return false;
	}
}
