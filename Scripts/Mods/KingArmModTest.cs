using Godot;
using Godot.Collections;
using System;

public partial class KingArmModTest : ArmMod
{
    public override bool PerformAttack(ChessBoard board, PlayerController player, Vector2I mouseMapCoordinates)
    {
        if (attackType == AttackType.aoe)
        {
            bool hitSomething = false;
            foreach (Vector2I validTile in board.rotatedAOECoords)
            {
                Node2D node = board.Grid[validTile.X, validTile.Y];

                if (node is not Scrap scrap)
                {
                    board.SummonScrap(player, validTile);
                    hitSomething = true;
                }
            }

            return hitSomething;
        }

        return false;
    }
}