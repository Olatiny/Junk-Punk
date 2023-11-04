using Godot;
using System;

public partial class ModSlot : ColorRect
{
	[Export] private PlayerController player;
	[Export] private Mod.BodyPart modType;
	[Export] private bool slotIsLeft;
	[Export] private TextureRect bodySprite;
	[Export] private Texture2D baseTexture;
	private Mod containedMod = null;

	GameManager gameMana;

	bool mouseOver = false;

	public override void _Ready()
	{
		base._Ready();

		gameMana = GetNode<GameManager>("/root/InventoryShop/GameManager");
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		GetNode<Globals>("/root/Globals").ModUnequip += OnUnequip;
	}

	// Called when the current drag hovers over this control.
	// Returns whether this control is a valid candidate for being dropped on.
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		InventorySlot draggingSlot = (InventorySlot)data;

		bool canDrop = draggingSlot.GetContainedMod().bodyPart == modType;
		return canDrop;
	}

	// Called when the player releases a drag on top of this control.
	public override void _DropData(Vector2 atPosition, Variant data)
	{
		InventorySlot incomingSlot = (InventorySlot)data;

		containedMod = incomingSlot.SwapInMod(null);
		bodySprite.Texture = player.playerId == 1 ? containedMod.bigSpriteBlue : containedMod.bigSpriteRed;

		GD.Print("Calling slot is " + this);
		GD.Print("slotIsLeft is " + slotIsLeft);
		player.Equip(containedMod, modType, slotIsLeft ? 0 : 1);
	}

	private void OnMouseEntered()
	{
		mouseOver = true;

		Mod slotContainedMod = containedMod;

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

		GodotObject go = (GodotObject)gameMana.tooltipWindow;
		go.Call("show_tooltip", false);
	}

	private void OnUnequip(PlayerController player, Mod mod)
	{
		if (containedMod == mod)
			bodySprite.Texture = baseTexture;
	}
}
