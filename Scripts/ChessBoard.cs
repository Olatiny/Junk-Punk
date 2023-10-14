using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;

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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Grid = new Node2D[gridSize.X, gridSize.Y];

		Grid[Player1Location.X, Player1Location.Y] = Player1Node;
		UpdateNodePosition(Player1Node, Player1Location);

		Grid[Player2Location.X, Player2Location.Y] = Player2Node;
		UpdateNodePosition(Player2Node, Player2Location);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{

		}
	}

	public void RequestMove(String direction)
	{
		switch (direction)
		{
			case "Up":
				if (!(Player1Location.Y - 1 < 0 || IsCellOccupied(new Vector2I(Player1Location.X, Player1Location.Y - 1))))
				{
					Grid[Player1Location.X, Player1Location.Y] = null;
					Grid[Player1Location.X, Player1Location.Y - 1] = Player1Node;
					Player1Location.Y--;
				}

				break;
			case "Down":
				if (!(Player1Location.Y + 1 >= gridSize.Y || IsCellOccupied(new Vector2I(Player1Location.X, Player1Location.Y + 1))))
				{
					Grid[Player1Location.X, Player1Location.Y] = null;
					Grid[Player1Location.X, Player1Location.Y + 1] = Player1Node;
					Player1Location.Y++;
				}

				break;
			case "Left":
				if (!(Player1Location.X - 1 < 0 || IsCellOccupied(new Vector2I(Player1Location.X - 1, Player1Location.Y))))
				{
					Grid[Player1Location.X, Player1Location.Y] = null;
					Grid[Player1Location.X - 1, Player1Location.Y] = Player1Node;
					Player1Location.X--;
				}

				break;
			case "Right":
				if (!(Player1Location.X + 1 >= gridSize.X || IsCellOccupied(new Vector2I(Player1Location.X + 1, Player1Location.Y))))
				{
					Grid[Player1Location.X, Player1Location.Y] = null;
					Grid[Player1Location.X + 1, Player1Location.Y] = Player1Node;
					Player1Location.X++;
				}

				break;
			case "Default":
				break;
		}

		// Move player to new position, calculated as cell coords * tile-size, and add offset to ensure centered on cell.
		UpdateNodePosition(Player1Node, Player1Location);
	}

	private void UpdateNodePosition(Node2D node, Vector2I cellLocation)
	{
		node.Position = (cellLocation * tileSize) + tileOffset;
	}

	private bool IsCellOccupied(Vector2I cellLocation)
	{
		return Grid[cellLocation.X, cellLocation.Y] != null;
	}
}
