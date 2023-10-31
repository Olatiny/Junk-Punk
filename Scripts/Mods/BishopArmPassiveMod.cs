using Godot;
using Godot.Collections;
using System;

public partial class BishopArmPassiveMod : PassiveMod
{
	public override void OnUpkeep(PlayerController player) {
		if(player?.armMods?[0] == this && player?.armMods?[1] != null) {
			player.armMods[1].durability++;
		} else if(player?.armMods?[1] == this && player?.armMods?[0] != null){
			player.armMods[0].durability++;
		}
	}
}
