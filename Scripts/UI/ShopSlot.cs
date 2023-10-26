using Godot;
using System;

public partial class ShopSlot : Node
{
	private Mod containedMod;
	[Export] int slotID;
	[Export] Node parentShop;
	
	private void _OnShopButtonPressed()
	{
		parentShop.BuyShopItem(slotID, containedMod);
	}
}
