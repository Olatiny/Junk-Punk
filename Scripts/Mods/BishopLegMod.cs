using Godot;
using Godot.Collections;
using System;

public partial class BishopLegMod : PassiveMod
{
	public override void OnUpkeep(PlayerController player) {
		if(player?.legMods?[0] == this && player?.legMods?[1] != null) {
			player.legMods[1].durability++;
		} else if(player?.legMods?[1] == this && player?.legMods?[0] != null) {
			player.legMods[0].durability++;
		}
	}
}
