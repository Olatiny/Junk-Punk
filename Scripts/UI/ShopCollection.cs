using Godot;
using System;

public partial class ShopCollection : Node
{
	[Export] private ShopSlot[] shopSlots = { null, null, null, null, null };
	[Export] private Node gameMana;

	private void BuyShopItem(int slotID, Mod modBought)
	{
		PlayerController playerCon = gameMana.GetCurrentPlayer();
		if (playerCon.currentScrap >= modBought.cost)
		{
			// Purchase was successful.
		}
		else
		{
			// Purchase failed.
		}
	}
}
