using Godot;
using Godot.Collections;
using System;

public partial class ArmMod : Mod
{
	public enum AttackType { damage, range, aoe, special }

	[Export] public AttackType attackType { get; private set; }
	[Export] public int bonusRange { get; set; } = new();
	[Export] public Array<Vector2I> aoe { get; set; } = new();
	[Export] public int bonusDmg { get; set; }

	public ArmMod() : base()
	{ }

	public virtual bool PerformAttack(ChessBoard board, PlayerController player, Vector2I mouseMapCoordinates)
	{
		if (attackType == AttackType.aoe)
		{
			bool hitSomething = false;
			foreach (Vector2I validTile in board.rotatedAOECoords)
			{
				Node2D node = board.Grid[validTile.X, validTile.Y];

				if (node is PlayerController enemy)
				{
					// returns true if enemy is killed, they still take damage if false
					enemy.TakeDamage(GetAttackDamage(board, player));
					hitSomething = true;
				}

				if (node is Scrap scrap)
				{
					scrap.Harvest(player);
					hitSomething = true;
				}
			}

			return hitSomething;
		}
		else if (attackType != AttackType.special)
		{
			foreach (Vector2I validTile in board.validModTileCoords)
			{
				if (mouseMapCoordinates == validTile)
				{
					Node2D node = board.Grid[validTile.X, validTile.Y];

					if (node is PlayerController enemy)
					{
						// returns true if enemy is killed, they still take damage if false
						enemy.TakeDamage(GetAttackDamage(board, player));
						player.UpdateDirection(player.gridPosition, enemy.gridPosition);
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
		}

		return false;
	}

	public virtual int GetAttackDamage(ChessBoard board, PlayerController player)
	{
		return player.baseAttackDmg + bonusDmg;
	}

	public Array<Vector2I> GetValidAttackTiles(ChessBoard board, PlayerController player)
	{
		Array<Vector2I> validModTileCoords = new();

		if (attackType == AttackType.aoe)
		{
			foreach (Vector2I aoeTile in aoe)
			{
				validModTileCoords.Add(player.gridPosition + aoeTile);
			}

			return validModTileCoords;
		}

		Array<Node2D> possibleTargets = board.GetAllNodesOnBoard();

		foreach (Node2D possibleTarget in possibleTargets)
		{
			if (possibleTarget is PlayerController otherPlayer && otherPlayer.playerId != player.playerId)
			{
				Vector2I dist = otherPlayer.gridPosition - player.gridPosition;
				if (dist.Length() <= player.baseAttackRange + bonusRange)
					validModTileCoords.Add(otherPlayer.gridPosition);
			}

			if (possibleTarget is Scrap scrap)
			{
				Vector2I dist = scrap.gridPosition - player.gridPosition;
				if (dist.Length() <= player.baseAttackRange + bonusRange)
					validModTileCoords.Add(scrap.gridPosition);
			}
		}

		return validModTileCoords;
	}

	public override Mod Clone()
	{
		Mod modClone = base.Clone();
		modClone.SetScript(GetScript());

		ArmMod armModClone = (ArmMod)modClone;
		armModClone.attackType = attackType;
		armModClone.bonusRange = bonusRange;
		armModClone.bonusDmg = bonusDmg;
		armModClone.aoe = aoe;

		return armModClone;
	}
}
