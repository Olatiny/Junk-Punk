using Godot;
using Godot.Collections;
using System;

public partial class KnightArmMod : PassiveMod
{
    public override void OnCollectedScrap(PlayerController Player, Scrap scrap)
    {
        if (Player == myPlayer)
            Player.currentScrap += scrap.currentScrapValue;
    }
}