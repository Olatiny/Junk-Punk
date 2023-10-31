using Godot;
using Godot.Collections;
using System;

public partial class KingHeadMod : PassiveMod
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
			Player.currentScrap += 10;
		}
	}
}
