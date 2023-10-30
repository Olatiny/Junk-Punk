using Godot;
using System;

public partial class ModSlot : TextureRect
{
	[Export] private PlayerController player;
	[Export] private Mod.BodyPart modType;
	[Export] private bool slotIsLeft;
	private Mod containedMod = null;

	// Called when the current drag hovers over this control.
	// Returns whether this control is a valid candidate for being dropped on.
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if (!InventorySlot.IsInstanceOfType(data))
			return false;
		
		InventorySlot draggingSlot = (InventorySlot) data;
			
		bool canDrop = draggingSlot.GetContainedMod().bodyPart == modType;
		return canDrop;
	}

	// Called when the player releases a drag on top of this control.
	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (!InventorySlot.IsInstanceOfType(data))
			return;
		
		InventorySlot incomingSlot = (InventorySlot) data;
		
		containedMod = incomingSlot.GetContainedMod();
		Texture = containedMod.bigSpriteBlue;
		
		player.Equip(containedMod, modType, slotIsLeft ? 0 : 1);
	}
}
