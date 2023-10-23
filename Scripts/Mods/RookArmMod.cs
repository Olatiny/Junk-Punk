using Godot;
using Godot.Collections;
using System;

public partial class RookArmMod : ArmMod
{
    public override int GetAttackDamage(ChessBoard board, PlayerController player)
    {
        if (player.gridPosition.X == 0 || player.gridPosition.X == board.gridSize.X - 1)
            return player.baseAttackDmg * 2;

        if (player.gridPosition.Y == 0 || player.gridPosition.Y == board.gridSize.Y - 1)
            return player.baseAttackDmg * 2;

        return player.baseAttackDmg;
    }
}