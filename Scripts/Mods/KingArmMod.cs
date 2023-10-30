using Godot;
using Godot.Collections;
using System;

public partial class KingArmMod : ArmMod
{
	public override bool PerformAttack(ChessBoard board, PlayerController player, Vector2I mouseMapCoordinates) {
			
		if (attackType == AttackType.aoe)
		{
			bool scrapValid = false;
			foreach (Vector2I validTile in board.rotatedAOECoords)
			{
				Node2D node = board.Grid[validTile.X, validTile.Y];

				if (!(node is Scrap scrap))
				{
					board.SummonScrap(player, validTile);
					scrapValid = true;
				}
			}

			return scrapValid;
		}

		return false;
	}
}
