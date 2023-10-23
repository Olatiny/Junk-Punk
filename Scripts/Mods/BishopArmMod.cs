using Godot;
using Godot.Collections;
using System;

public partial class BishopArmMod : ArmMod
{
	public double moneySteal = .10;
	
	public override bool PerformAttack(ChessBoard board, PlayerController player, Vector2I mouseMapCoordinates) {
		foreach (Vector2I validTile in board.validModTileCoords) {
			if (mouseMapCoordinates == validTile) {
				Node2D node = board.Grid[validTile.X, validTile.Y];
				
				if (node is PlayerController enemy) {
					int scrapToSteal = (int)(enemy.currentScrap * moneySteal);
					enemy.currentScrap -= scrapToSteal; 
					player.currentScrap += scrapToSteal;
					return true;
				}
			}
		}
		
		return false;
	}
}
