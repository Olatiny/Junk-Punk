using Godot;
using System;

public partial class ShopSlot : Node
{
	private Mod containedMod;
	[Export] int slotID;
	[Export] ShopCollection parentShop;
	[Export] TextureRect modIcon;
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
