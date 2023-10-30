using Godot;
using Godot.Collections;
using System;

public partial class LegMod : Mod
{
	// [Export] public bool isPathContinuous = false;

	public enum PathProperty
	{
		None = 0, UP = 1, DOWN = 2, LEFT = 4, RIGHT = 8, CONTINUOUS = 16, KEEPLAST = 32
	}

	[Export(PropertyHint.Flags, "Up:1,Down:2,Left:4,Right:8,Continuous:16,Keep Last:32")] public uint pathFlags = 0;
	[Export] public Array<Array<String>> modPaths { get; set; }
	[Export] public Array<Vector2I> modJumps { get; set; }

	public LegMod() : base()
	{ }

	public LegMod(String modUID, int modDurability) : base(modUID, modDurability)
	{
		bodyPart = BodyPart.Leg;
		modPaths = new();
		modJumps = new();

		LegMod m = new();
	}

	public bool CheckFlags(PathProperty propertyFlag)
	{
		return (PathProperty)(pathFlags & (uint)propertyFlag) == propertyFlag;
	}

	public virtual Array<Vector2I> GetValidMoveTiles(ChessBoard board, PlayerController player)
	{
		Array<Vector2I> validModMoveTiles = new();

		GetValidModMovePaths(board, player, ref validModMoveTiles);

		GetValidModMoveJumps(board, player, ref validModMoveTiles);

		return validModMoveTiles;
	}

	private void GetValidModMovePaths(ChessBoard board, PlayerController player, ref Array<Vector2I> validModMoveTiles)
	{
		foreach (Array<string> path in modPaths)
		{
			bool canGoUp = CheckFlags(PathProperty.UP),
			canGoDown = CheckFlags(PathProperty.DOWN),
			canGoLeft = CheckFlags(PathProperty.LEFT),
			canGoRight = CheckFlags(PathProperty.RIGHT);

			bool isPathContinuous = CheckFlags(PathProperty.CONTINUOUS);
			bool keepLast = CheckFlags(PathProperty.KEEPLAST);

			Array<Vector2I> currUpLocations = new() { player.gridPosition },
			currDownLocations = new() { player.gridPosition },
			currLeftLocations = new() { player.gridPosition },
			currRightLocations = new() { player.gridPosition };

			foreach (string direction in path)
			{
				//Check going up
				if (canGoUp && !UseDirectionIfValid(board, direction, Vector2I.Up, Vector2I.Left, Vector2I.Right, ref currUpLocations))
					canGoUp = false;

				//Check going down
				if (canGoDown && !UseDirectionIfValid(board, direction, Vector2I.Down, Vector2I.Right, Vector2I.Left, ref currDownLocations))
					canGoDown = false;

				//Check going left
				if (canGoLeft && !UseDirectionIfValid(board, direction, Vector2I.Left, Vector2I.Down, Vector2I.Up, ref currLeftLocations))
					canGoLeft = false;

				//Check going right
				if (canGoRight && !UseDirectionIfValid(board, direction, Vector2I.Right, Vector2I.Up, Vector2I.Down, ref currRightLocations))
					canGoRight = false;
			}

			if (isPathContinuous)
			{
				// Add recorded paths in all directions (funny for loops)
				for (int i = 1; i < currUpLocations.Count; validModMoveTiles.Add(currUpLocations[i]), i++) ;

				for (int i = 1; i < currDownLocations.Count; validModMoveTiles.Add(currDownLocations[i]), i++) ;

				for (int i = 1; i < currLeftLocations.Count; validModMoveTiles.Add(currLeftLocations[i]), i++) ;

				for (int i = 1; i < currRightLocations.Count; validModMoveTiles.Add(currRightLocations[i]), i++) ;
			}
			else
			{
				if (keepLast || canGoUp)
					validModMoveTiles.Add(currUpLocations[^1]);

				if (keepLast || canGoDown)
					validModMoveTiles.Add(currDownLocations[^1]);

				if (keepLast || canGoLeft)
					validModMoveTiles.Add(currLeftLocations[^1]);

				if (keepLast || canGoRight)
					validModMoveTiles.Add(currRightLocations[^1]);
			}
		}
	}

	private void GetValidModMoveJumps(ChessBoard board, PlayerController player, ref Array<Vector2I> validModMoveTiles)
	{
		if (modJumps == null)
			return;

		foreach (Vector2I jump in modJumps)
		{
			Vector2I gridPos = player.gridPosition + jump;

			if (board.IsTileAvailable(gridPos))
				validModMoveTiles.Add(gridPos);
		}
	}

	private static bool UseDirectionIfValid(ChessBoard board, String direction, Vector2I forwardDirection, Vector2I leftDirection, Vector2I rightDirection, ref Array<Vector2I> outCurrLocations)
	{
		Vector2I currLocation = new()
		{
			X = outCurrLocations[^1].X,
			Y = outCurrLocations[^1].Y
		};

		switch (direction)
		{
			case "Forward":
				if (board.IsTileAvailable(currLocation += forwardDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "Left":
				if (board.IsTileAvailable(currLocation += leftDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "LeftDiagonal":
				if (board.IsTileAvailable(currLocation += leftDirection + forwardDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "Right":
				if (board.IsTileAvailable(currLocation += rightDirection))
				{
					outCurrLocations.Add(currLocation);
					return true;
				}
				else
					return false;

			case "RightDiagonal":
				if (board.IsTileAvailable(currLocation += rightDirection + forwardDirection))
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

	public virtual bool RequestMove(ChessBoard board, PlayerController player, Vector2I mouseMapPos)
	{
		foreach (Vector2I validTile in board.validModTileCoords)
		{
			if (mouseMapPos == validTile)
			{
				board.SetNodeGridPosition(player, player.gridPosition, mouseMapPos);
				return true;
			}
		}

		return false;
	}

	public override Mod Clone()
	{
		Mod modClone = base.Clone();
		modClone.SetScript(GetScript());

		LegMod legModClone = (LegMod)modClone;
		legModClone.pathFlags = pathFlags;
		legModClone.modPaths = modPaths;
		legModClone.modJumps = modJumps;

		return legModClone;
	}
}
