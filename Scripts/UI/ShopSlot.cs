using Godot;
using System;

public partial class ShopSlot : Node
{
	private Mod containedMod;
	[Export] int slotID;
	[Export] ShopCollection parentShop;
	[Export] TextureRect modIcon;
	[Export] Label priceText;
	
	public void ClearContainedMod()
	{
		containedMod = null;
		modIcon.Texture = null;
		priceText.Text = "";
	}
	
	public void SetContainedMod(Mod targetMod)
	{
		containedMod = targetMod;
		modIcon.Texture = targetMod.icon;
		priceText.Text = targetMod.cost.ToString();
	}
	
	private void _OnShopButtonPressed()
	{
		if (containedMod != null)
			parentShop.SelectShopItem(slotID, containedMod);
	}
}
