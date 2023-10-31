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
				Node2D node = board.GetNodeAtTile(validTile);
				
				if (node is not Scrap)
					continue;
				
				Scrap scrap = node as Scrap;
				
				// GD.Print("Scrap pos before: " + (board.GetNodeAtTile(scrap.gridPosition) as Scrap).gridPosition);
				// GD.Print("Playe pos before: " + (board.GetNodeAtTile(player.gridPosition) as PlayerController).gridPosition);

				board.SwapNodes(player.gridPosition, scrap.gridPosition);

				// Node2D tileNode = board.GetNodeAtTile(scrap.gridPosition);
				// GD.Print("Scrap pos after: " + (tileNode as Scrap).gridPosition);
				
				// Node2D tileNode2 = board.GetNodeAtTile(player.gridPosition);
				// GD.Print("Playe pos after: " + (tileNode2 as PlayerController).gridPosition);
				return true;
			}
		}
		
		return false;
	}
}
