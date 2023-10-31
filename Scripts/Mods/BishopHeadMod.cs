using Godot;
using Godot.Collections;
using System;

public partial class BishopHeadMod : PassiveMod
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
			scrap.Modulate = new Color(0.5f, 0.5f, (float)0.8, 1);
			scrap.maxScrapValue = 0;
		}
		
		if(Player.playerId == 2)
		{
			scrap.Modulate = new Color((float)0.8, 0.5f, 0.5f, 1);
			scrap.maxScrapValue = 0;
			 
		}
	}
}
