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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		gameManager = GetParent<GameManager>();
		Grid = new Node2D[gridSize.X, gridSize.Y];
		validBasicTileCoords = new HashSet<Vector2I>();
		validModTileCoords = new HashSet<Vector2I>();
		rotatedAOECoords = new HashSet<Vector2I>();

		foreach (PlayerController player in gameManager.players)
		{
			SetNodeGridPosition(player, player.gridPosition);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// UpdateMouseOverHighlight();

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
			SetCell(1, tile, 1, new Vector2I(1, 0));
		}
	}

	public void GetValidMoves(PlayerController player)
	{
		ClearValidTiles();

		GetValidBasicMoveTiles(player);
		GetValidModMoveTiles(player);

		HighlightValidTiles();
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
		foreach (LegMod legMod in player.legMods)
		{
			if (legMod == null)
				continue;

			Array<Vector2I> modTiles = legMod?.GetValidMoveTiles(this, player);

			if (modTiles != null)
				for (int i = 0; i < modTiles.Count; validModTileCoords.Add(modTiles[i]), i++);
		}
	}


	public void GetValidAttacks(PlayerController player)
	{
		ClearValidTiles();

		GetValidBasicAttackTiles(player);
		GetValidModAttackTiles(player);

		HighlightValidTiles();
	}

	public void GetValidBasicAttackTiles(PlayerController player)
	{
		//left
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
			for (int i = 0; i < modTiles.Count; validModTileCoords.Add(modTiles[i]), i++);
	}

	private void HighlightValidTiles()
	{
		foreach (Vector2I tile in validModTileCoords)
		{
			SetCell(1, tile, 1, new Vector2I(1, 0));
		}

		foreach (Vector2I tile in validBasicTileCoords)
		{
			SetCell(1, tile, 1, new Vector2I(1, 1));
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
		EraseCell(2, mouseOverCell);
		mouseOverCell = LocalToMap(GetLocalMousePosition());
		TileData tileData = GetCellTileData(0, mouseOverCell);
		if (tileData != null && tileData.GetCustomData("highlightable").AsBool() && !Input.IsMouseButtonPressed(MouseButton.Left))
			SetCell(2, mouseOverCell, 1, new Vector2I(0, 1));
	}

	public bool RequestMove(PlayerController player, Vector2 mouseCoordinates)
	{
		GameManager gameManager = GetParent<GameManager>();
		if (!gameManager.IsPlayerTurn(player))
			return false;

		Vector2I mouseMapPos = LocalToMap(mouseCoordinates);

		foreach (Vector2I validTile in validBasicTileCoords)
		{
			if (mouseMapPos == validTile)
			{
				SetNodeGridPosition(player, player.gridPosition, mouseMapPos);
				ClearValidTiles();
				gameManager.PlayerMoved();
				return true;
			}
		}

		foreach (Vector2I validTile in validModTileCoords)
		{
			if (mouseMapPos == validTile)
			{
				SetNodeGridPosition(player, player.gridPosition, mouseMapPos);
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
				}

				// TODO: Implement Scrap
				// if (node is Scrap scrap)
				// 	player.HarvestScrap(scrap);

				ClearValidTiles();
				gameManager.PlayerActioned();
				return true;
			}
		}

		return false;
	}

	/**
	 * If newLocation is null, just set position globally and ensure node is in grid.
	 * 
	 * if newLocatrion is not null, update the position of the node to reflect new position in grid and in world. 
	 * and, if it's a player, update it's gridPosition field.
	 */
	private void SetNodeGridPosition(Node2D node, Vector2I oldLocation, Vector2I? newLocation = null)
	{
		if (!newLocation.HasValue)
			Grid[oldLocation.X, oldLocation.Y] = node;
		else
		{
			Grid[oldLocation.X, oldLocation.Y] = null;
			Grid[newLocation.Value.X, newLocation.Value.Y] = node;

			if (node is PlayerController player)
				player.gridPosition = newLocation.Value;
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
}
