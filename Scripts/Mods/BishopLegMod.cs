using Godot;
using Godot.Collections;
using System;

public partial class BishopLegMod : LegMod
{
	public void IncreaseEnergy(PlayerController player) {
		if(player.legMods[0].uid == "BishopLeg" && player.legMods[1] != null) {
			player.legMods[1].durability++;
		} else if(player.legMods[1].uid == "BishopLeg" && player.legMods[0] != null) {
			player.legMods[0].durability++;
		}
	}
}
