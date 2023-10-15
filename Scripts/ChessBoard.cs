using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Diagnostics;

public partial class ChessBoard : TileMap
{
	Node2D[,] Grid;

	[ExportCategory("Chess Board")]
	[ExportSubgroup("Grid Definitions")]
	[Export] Vector2I gridSize = new(8, 8);
	[Export] Vector2I tileSize = new(16, 13);
	[Export(PropertyHint.None, "Offset so that objects are placed at the center of tiles")] Vector2I tileOffset = new Vector2I(8, 6);

	[ExportSubgroup("Player Information")]
	[Export] Array<PlayerController> playerList;

	Array<Vector2I> validTileCoords;
	Vector2I mouseOverCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Grid = new Node2D[gridSize.X, gridSize.Y];
		validTileCoords = new Array<Vector2I>();

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

		GetValidBasicMoveTiles(player);

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

	private void HighlightValidTiles()
	{
		foreach (Vector2I tile in validTileCoords)
		{
			SetCell(1, tile, 1, new Vector2I(1, 0));
		}
	}

	public void ClearValidTiles()
	{
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
		if (GetCellTileData(0, mouseOverCell) != null && !Input.IsMouseButtonPressed(MouseButton.Left))
			SetCell(2, mouseOverCell, 1, new Vector2I(0, 0));
	}

	public bool RequestMove(PlayerController player, Vector2 mouseCoordinates)
	{
		Vector2I mouseMapPos = LocalToMap(mouseCoordinates);

		foreach (Vector2I validTile in validTileCoords)
		{
			if (mouseMapPos.X == validTile.X && mouseMapPos.Y == validTile.Y)
			{
				Vector2I playerPos = player.gridPosition;
				Grid[playerPos.X, playerPos.Y] = null;
				Grid[mouseMapPos.X, mouseMapPos.Y] = player;
				player.gridPosition = mouseMapPos;

				SetNodeGridPosition(player, player.gridPosition);
				ClearValidTiles();
				return true;
			}
		}

		return false;
	}

	private void SetNodeGridPosition(Node2D node, Vector2I cellLocation)
	{
		node.Position = (cellLocation * tileSize) + tileOffset;
	}

	private Vector2 GetTileWorldPosition(Vector2I cellLocation)
	{
		return cellLocation * tileSize + tileOffset;
	}

	private bool CanMoveToTile(Vector2I cellLocation)
	{
		if (cellLocation.X < 0 || cellLocation.X >= gridSize.X)
			return false;

		if (cellLocation.Y < 0 || cellLocation.Y >= gridSize.Y)
			return false;

		return Grid[cellLocation.X, cellLocation.Y] == null;
	}
}
