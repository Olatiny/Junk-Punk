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

	[ExportSubgroup("Player 1 Information")]
	[Export] Node2D Player1Node;
	[Export] Vector2I Player1Location = new(0, 0);

	[ExportSubgroup("Player 2 Information")]
	[Export] Node2D Player2Node;
	[Export] Vector2I Player2Location = new(0, 0);

	Array<Vector2I> validTileCoords;
	Vector2I mouseOverCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Grid = new Node2D[gridSize.X, gridSize.Y];
		validTileCoords = new Array<Vector2I>();

		Grid[Player1Location.X, Player1Location.Y] = Player1Node;
		SetNodePosition(Player1Node, Player1Location);

		Grid[Player2Location.X, Player2Location.Y] = Player2Node;
		SetNodePosition(Player2Node, Player2Location);
	}

	public override void _Process(double delta)
	{
		UpdateMouseOverHighlight();
	}

	public void GetValidMoves()
	{
		ClearValidTiles();

		GetValidBasicMoveTiles();

		HighlightValidTiles();
	}

	private void GetValidBasicMoveTiles()
	{
		//left
		if (CanMoveToTile(Player1Location - new Vector2I(1, 0)))
			validTileCoords.Add(LocalToMap(GetCellPosition(Player1Location - new Vector2I(1, 0))));

		//right
		if (CanMoveToTile(Player1Location + new Vector2I(1, 0)))
			validTileCoords.Add(LocalToMap(GetCellPosition(Player1Location + new Vector2I(1, 0))));

		//bottom
		if (CanMoveToTile(Player1Location + new Vector2I(0, 1)))
			validTileCoords.Add(LocalToMap(GetCellPosition(Player1Location + new Vector2I(0, 1))));

		//top
		if (CanMoveToTile(Player1Location - new Vector2I(0, 1)))
			validTileCoords.Add(LocalToMap(GetCellPosition(Player1Location - new Vector2I(0, 1))));
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

	public void RequestMove(PlayerController player, Vector2 mouseCoordinates)
	{
		Vector2I mouseMapPos = LocalToMap(mouseCoordinates);

		foreach (Vector2I validTile in validTileCoords)
		{
			if (mouseMapPos.X == validTile.X && mouseMapPos.Y == validTile.Y)
			{
				Grid[Player1Location.X, Player1Location.Y] = null;
				Grid[mouseMapPos.X, mouseMapPos.Y] = Player1Node;
				Player1Location = mouseMapPos;
				SetNodePosition(Player1Node, Player1Location);
				ClearValidTiles();
				break;
			}
		}
	}

	private void SetNodePosition(Node2D node, Vector2I cellLocation)
	{
		node.Position = (cellLocation * tileSize) + tileOffset;
	}

	private Vector2 GetCellPosition(Vector2I cellLocation)
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
