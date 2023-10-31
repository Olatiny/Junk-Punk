using Godot;
using Godot.Collections;
using System;

public partial class QueenLegMod : LegMod
{
	public override Array<Vector2I> GetValidMoveTiles(ChessBoard board, PlayerController player)
	{
		Array<Vector2I> validScrapTiles = new();
		
		Array<Node2D> allNodes = board.GetAllNodesOnBoard();
		
		foreach(Node2D node in allNodes) {
			if(node is Scrap scrap) {
				validScrapTiles.Add(scrap.gridPosition);
			}
		}

		return validScrapTiles;
	}
	
	public override bool RequestMove(ChessBoard board, PlayerController player, Vector2I mouseMapPos)
	{
		foreach(Vector2I validTile in board.validModTileCoords)
		{
			if(mouseMapPos == validTile)
			{
				Vector2I playerPosition = player.gridPosition;
				Node2D node = board.GetNodeAtTile(validTile);
				
				if (node is not Scrap)
					continue;
				
				Scrap scrap = node as Scrap;
				
				board.SetNodeGridPosition(player, playerPosition, scrap.gridPosition);
				board.SetNodeGridPosition(scrap, scrap.gridPosition, playerPosition);
				return true;
			}
		}
		
		return false;
	}
}
