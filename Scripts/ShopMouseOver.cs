using Godot;
using System;

public partial class ShopMouseOver : TextureButton
{
	[Export] TextureRect icon;
	[Export] Control price;
	[Export] ShopSlot slot;
	GameManager gameMana;

	bool mouseOver = false;

	public override void _Ready()
	{
		base._Ready();
		
		gameMana = GetNode<GameManager>("/root/InventoryShop/GameManager");
		price.Modulate = new(1, 1, 1, 0);
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		mouseOver = true;
		
		icon.Modulate = new(1, 1, 1, 0);
		price.Modulate = new(1, 1, 1, 1);
		
		Mod slotContainedMod = slot.GetContainedMod();
		
		if (slotContainedMod == null)
			return;
			
		GD.Print("Hello?");
		GodotObject go = (GodotObject)gameMana.tooltipWindow;
		go.Call("show_tooltip", true);
		GD.Print((object)slotContainedMod.description);
		go.Call("display_text", slotContainedMod.description + $"\n\nBattery: {slotContainedMod.durability}");
	}

	private void OnMouseExited()
	{
		mouseOver = false;
		icon.Modulate = new(1, 1, 1, 1);
		price.Modulate = new(1, 1, 1, 0);
		
		GodotObject go = (GodotObject)gameMana.tooltipWindow;
		go.Call("show_tooltip", false);
	}
}
