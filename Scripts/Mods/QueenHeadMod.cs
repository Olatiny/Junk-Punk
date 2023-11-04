using Godot;
using Godot.Collections;
using System;

public partial class QueenHeadMod : PassiveMod
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}
	
	public override void OnUpkeep(PlayerController Player)
	{
		if(Player.headMod == this)
		{
			foreach(Mod mod in Player.armMods)
			{
				if(mod != null && mod.buffType != BuffType.Passive) {
					mod.durability++;
				}
			}
			
			foreach(Mod mod in Player.legMods)
			{
				if(mod != null && mod.buffType != BuffType.Passive)
				{
					mod.durability++;
				}
			}
		}
	}
}
