using Godot;
using Godot.Collections;
using System;

public partial class PawnHeadMod : PassiveMod
{
	public override void _Ready()
	{
		base._Ready();
	}
	
	public override void OnPlayerTookDamage(PlayerController player, int damageAmount)
	{
		if (player.headMod == this)
		{
			player.health += damageAmount;
			player.Unequip(this);
		}
	}
}
