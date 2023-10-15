using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ChessBoard : TileMap
{
	Node2D[,] Grid;

	[ExportCategory("Chess Board")]
	[ExportSubgroup("Grid Definitions")]
	[Export] Vector2I gridSize = new(8, 8);
	[Export] Vector2I tileSize = new(16, 13);
	[Export] Vector2I tileOffset = new(8, 6);

	[ExportSubgroup("Player Information")]
	[Export] Array<PlayerController> playerList;

	HashSet<Vector2I> validTileCoords;
	int validPlayerID = -1;

	Vector2I mouseOverCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Grid = new Node2D[gridSize.X, gridSize.Y];
		validTileCoords = new HashSet<Vector2I>();

		foreach (PlayerController player in playerList)
		{
			SetNodeGridPosition(player, player.gridPosition);
		}
	}

	public override void _Process(double delta)
	{
		UpdateMouseOverHighlight();
	}

	public void GetValidMoves(PlayerController player)
	{
		ClearValidTiles();
		validPlayerID = player.playerId;

		GetValidBasicMoveTiles(player);
		GetValidModMoveTiles(player);

		HighlightValidTiles();
	}

	private void GetValidBasicMoveTiles(PlayerController player)
	{
		//left
		if (CanMoveToTile(player.gridPosition - new Vector2I(1, 0)))
			validTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(1, 0))));

		//right
		if (CanMoveToTile(player.gridPosition + new Vector2I(1, 0)))
			validTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(1, 0))));

		//bottom
		if (CanMoveToTile(player.gridPosition + new Vector2I(0, 1)))
			validTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(0, 1))));

		//top
		if (CanMoveToTile(player.gridPosition - new Vector2I(0, 1)))
			validTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(0, 1))));
	}

	private void GetValidModMoveTiles(PlayerController player)
	{
		foreach (Array<string> path in player.modMovement)
		{
			foreach (string direction in path)
			{

			}
		}
	}

	private void HighlightValidTiles()
	{
		foreach (Vector2I tile in validTileCoords)
		{
			SetCell(1, tile, 1, new Vector2I(1, 0));
		}
	}

	public void ClearValidTiles()
	{
		validPlayerID = -1;

		foreach (Vector2I tile in validTileCoords)
		{
			EraseCell(1, tile);
		}

		validTileCoords.Clear();
	}

	private void UpdateMouseOverHighlight()
	{
		EraseCell(2, mouseOverCell);
		mouseOverCell = LocalToMap(GetLocalMousePosition());
		TileData tileData = GetCellTileData(0, mouseOverCell);
		if (tileData != null && tileData.GetCustomData("highlightable").AsBool() && !Input.IsMouseButtonPressed(MouseButton.Left))
			SetCell(2, mouseOverCell, 1, new Vector2I(0, 0));
	}

	public bool RequestMove(PlayerController player, Vector2 mouseCoordinates)
	{
		if (player.playerId != validPlayerID)
			return false;

		Vector2I mouseMapPos = LocalToMap(mouseCoordinates);

		foreach (Vector2I validTile in validTileCoords)
		{
			if (mouseMapPos.X == validTile.X && mouseMapPos.Y == validTile.Y)
			{
				SetNodeGridPosition(player, player.gridPosition, mouseMapPos);
				ClearValidTiles();
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

	private Vector2 GetTileWorldPosition(Vector2I cellLocation)
	{
		return cellLocation * tileSize + tileOffset;
	}

	private bool CanMoveToTile(Vector2I tileLocation)
	{
		if (tileLocation.X < 0 || tileLocation.X >= gridSize.X)
			return false;

		if (tileLocation.Y < 0 || tileLocation.Y >= gridSize.Y)
			return false;

		return Grid[tileLocation.X, tileLocation.Y] == null;
	}
}
