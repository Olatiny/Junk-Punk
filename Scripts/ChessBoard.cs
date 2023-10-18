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

	HashSet<Vector2I> validBasicTileCoords;
	HashSet<Vector2I> validModTileCoords;
	int validPlayerID = -1;

	Vector2I mouseOverCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Grid = new Node2D[gridSize.X, gridSize.Y];
		validBasicTileCoords = new HashSet<Vector2I>();
		validModTileCoords = new HashSet<Vector2I>();

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
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(1, 0))));

		//right
		if (CanMoveToTile(player.gridPosition + new Vector2I(1, 0)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(1, 0))));

		//bottom
		if (CanMoveToTile(player.gridPosition + new Vector2I(0, 1)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition + new Vector2I(0, 1))));

		//top
		if (CanMoveToTile(player.gridPosition - new Vector2I(0, 1)))
			validBasicTileCoords.Add(LocalToMap(GetTileWorldPosition(player.gridPosition - new Vector2I(0, 1))));
	}

	private void GetValidModMoveTiles(PlayerController player)
	{
		LegMod legMod = player.legMod;

		foreach (Array<string> path in player.legMod.modPaths)
		{
			bool canGoUp = (legMod.pathDirections & 1) != 0, 
			canGoDown = (legMod.pathDirections & 2) != 0,
			canGoLeft = (legMod.pathDirections & 4) != 0, 
			canGoRight = (legMod.pathDirections & 8) != 0;

			bool isPathContinuous = legMod.isPathContinuous;
			
			Array<Vector2I> currUpLocations = new() {player.gridPosition}, 
			currDownLocations = new() {player.gridPosition},
			currLeftLocations = new() {player.gridPosition}, 
			currRightLocations = new() {player.gridPosition};

			foreach (string direction in path)
			{
				//Check going up
				if (canGoUp && !UseDirectionIfValid(direction, Vector2I.Up, Vector2I.Right, Vector2I.Left, ref currUpLocations))
					canGoUp = false;

				//Check going down
				if (canGoDown && !UseDirectionIfValid(direction, Vector2I.Down, Vector2I.Left, Vector2I.Right, ref currDownLocations))
					canGoDown = false;

				//Check going left
				if (canGoLeft && !UseDirectionIfValid(direction, Vector2I.Left, Vector2I.Down, Vector2I.Up, ref currLeftLocations))
					canGoLeft = false;

				//Check going right
				if (canGoRight && !UseDirectionIfValid(direction, Vector2I.Right, Vector2I.Up, Vector2I.Down, ref currRightLocations))
					canGoRight = false;
			}

			if (isPathContinuous)
			{
				for (int i = 1; i < currUpLocations.Count; validModTileCoords.Add(currUpLocations[i]), i++);

				for (int i = 1; i < currDownLocations.Count; validModTileCoords.Add(currDownLocations[i]), i++);

				for (int i = 1; i < currLeftLocations.Count; validModTileCoords.Add(currLeftLocations[i]), i++);

				for (int i = 1; i < currRightLocations.Count; validModTileCoords.Add(currRightLocations[i]), i++);
			}
			else
			{
				if (canGoUp)
					validModTileCoords.Add(currUpLocations[^1]);

				if (canGoDown)
					validModTileCoords.Add(currDownLocations[^1]);

				if (canGoLeft)
					validModTileCoords.Add(currLeftLocations[^1]);

				if (canGoRight)
					validModTileCoords.Add(currRightLocations[^1]);
			}
		}

		foreach (Vector2I jump in player.legMod.modJumps)
		{
			Vector2I gridPos = player.gridPosition + jump;

			if (CanMoveToTile(gridPos))
				validModTileCoords.Add(gridPos);
		}
	}

	private bool UseDirectionIfValid(String direction, Vector2I forwardDirection, Vector2I leftDirection, Vector2I rightDirection, ref Array<Vector2I> outCurrLocations)
	{
        Vector2I currLocation = new()
        {
            X = outCurrLocations[^1].X,
            Y = outCurrLocations[^1].Y
        };

        switch (direction)
		{
			case "Forward":
				if (CanMoveToTile(currLocation += forwardDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "Left":
				if (CanMoveToTile(currLocation += leftDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;
			
			case "LeftDiagonal":
				if (CanMoveToTile(currLocation += leftDirection + forwardDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "Right":
				if (CanMoveToTile(currLocation += rightDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "RightDiagonal":
				if (CanMoveToTile(currLocation += rightDirection + forwardDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			default:
				return false;
		}
	}

	private void HighlightValidTiles()
	{
		foreach (Vector2I tile in validBasicTileCoords)
		{
			SetCell(1, tile, 1, new Vector2I(1, 0));
		}

		foreach (Vector2I tile in validModTileCoords)
		{
			SetCell(1, tile, 1, new Vector2I(0, 0), 1);
		}
	}

	public void ClearValidTiles()
	{
		validPlayerID = -1;

		foreach (Vector2I tile in validBasicTileCoords)
		{
			EraseCell(1, tile);
		}

		foreach (Vector2I tile in validModTileCoords)
		{
			EraseCell(1, tile);
		}

		validBasicTileCoords.Clear();
		validModTileCoords.Clear();
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

		foreach (Vector2I validTile in validBasicTileCoords)
		{
			if (mouseMapPos.X == validTile.X && mouseMapPos.Y == validTile.Y)
			{
				SetNodeGridPosition(player, player.gridPosition, mouseMapPos);
				ClearValidTiles();
				return true;
			}
		}

		foreach (Vector2I validTile in validModTileCoords)
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
