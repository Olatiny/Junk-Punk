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
	[Export] Label priceText;
	GameManager gameMana;
	
	public override void _Ready()
	{
		base._Ready();
		gameMana = GetNode<GameManager>("/root/InventoryShop/GameManager");
	}
	
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
	
	public void OnMouseEnter()
	{
		if (containedMod == null)
			return;
			
		GD.Print("Hello?");
		GodotObject go = (GodotObject)gameMana.tooltip;
		go.Call("show_tooltip", true);
		GD.Print((object)containedMod.description);
		go.Call("display_text", containedMod.description);
	}
	
	public void OnMouseExit()
	{
		GodotObject go = (GodotObject)gameMana.tooltip;
		go.Call("show_tooltip", false);
	}
}
