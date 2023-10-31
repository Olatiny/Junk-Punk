using Godot;
using System;

public partial class ShopSlot : Node
{
	private Mod containedMod;
	[Export] int slotID;
	[Export] ShopCollection parentShop;
	[Export] TextureRect modIcon;
	[Export] TextureRect hundreds;
	[Export] TextureRect tens;
	[Export] TextureRect ones;

	public void ClearContainedMod()
	{
		containedMod = null;
		modIcon.Texture = null;
	}

	public void SetContainedMod(Mod targetMod)
	{
		containedMod = targetMod;
		modIcon.Texture = targetMod.icon;
		SetPrice(targetMod.cost);
	}

	private void _OnShopButtonPressed()
	{
		if (containedMod != null)
			parentShop.SelectShopItem(slotID, containedMod);
	}

	public void SetPrice(int price)
	{
		if (hundreds == null || tens == null || ones == null)
			return;

		int onesPlace = price % 10;
		price /= 10;

		int tensPlace = price % 10;
		int hundredsPlace = price / 10;

		((AtlasTexture)ones.Texture).Region = new Rect2(onesPlace * 4, 0, 4, 6);
		((AtlasTexture)tens.Texture).Region = new Rect2(tensPlace * 4, 0, 4, 6);
		((AtlasTexture)hundreds.Texture).Region = new Rect2(hundredsPlace * 4, 0, 4, 6);
	}
}
