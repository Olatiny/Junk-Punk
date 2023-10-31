using Godot;
using Godot.Collections;
using System;

public partial class RookHeadMod : PassiveMod
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public override void OnSummonedScrap(PlayerController Player, Scrap scrap)
	{
		if(Player.playerId == 1)
		{
			scrap.Modulate = new Color(0, 0, (float)0.8, 1);
			scrap.armor = 2;
		}
		
		if(Player.playerId == 2)
		{
			scrap.Modulate = new Color((float)0.8, 0, 0, 1);
			scrap.armor = 2;
		}
	}
}