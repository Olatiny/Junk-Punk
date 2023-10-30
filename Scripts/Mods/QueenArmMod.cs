using Godot;
using Godot.Collections;
using System;

public partial class QueenArmMod : ArmMod
{
	public override bool PerformAttack(ChessBoard board, PlayerController player, Vector2I mouseMapCoordinates) {
		foreach (Vector2I validTile in board.validModTileCoords) {
			if (mouseMapCoordinates == validTile) {
				Node2D node = board.Grid[validTile.X, validTile.Y];
				
				if (node is PlayerController enemy)
				{
					// returns true if enemy is killed, they still take damage if false
					player.UpdateDirection(player.gridPosition, enemy.gridPosition);
					if(enemy.direction == player.direction)
					{
						enemy.TakeDamage(GetAttackDamage(board, player) + 1);	
					} else {
						enemy.TakeDamage(GetAttackDamage(board, player));	
					}
					
					return true;
				}

				if (node is Scrap scrap)
				{
					scrap.Harvest(player);
					player.UpdateDirection(player.gridPosition, scrap.gridPosition);
					return true;
				}
			}
		}
		
		return false;
	}
}
