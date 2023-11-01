using Godot;
using System;

public partial class InventoryMouseOver : Control
{
	GameManager gameMana;

	bool mouseOver = false;

	public override void _Ready()
	{
		base._Ready();
		
		gameMana = GetNode<GameManager>("/root/InventoryShop/GameManager");
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		mouseOver = true;

        if (GetParent<InventorySlot>() is not InventorySlot slot)
            return;
		
		Mod slotContainedMod = slot.GetContainedMod();
		
		if (slotContainedMod == null)
			return;
			
		GD.Print("Hello?");
		GodotObject go = (GodotObject)gameMana.tooltipWindow;
		go.Call("show_tooltip", true);
		GD.Print((object)slotContainedMod.description);
		go.Call("display_text", slotContainedMod.description);
	}

	private void OnMouseExited()
	{
		mouseOver = false;
		
		GodotObject go = (GodotObject)gameMana.tooltipWindow;
		go.Call("show_tooltip", false);
	}
}
