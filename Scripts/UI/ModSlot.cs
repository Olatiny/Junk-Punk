using Godot;
using System;

public partial class ModSlot : TextureRect
{
	[Export] private PlayerController player;
	private Mod containedMod = null;
	private bool isDragging = false;

	// Called when the current drag hovers over this control.
	// Returns whether this control is a valid candidate for being dropped on.
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		Node dataNode = (Node) data;
		bool canDrop = !isDragging && dataNode.IsInGroup("INVENTORY");
		return canDrop;
	}

	// Called when the player releases a drag on top of this control.
	public override void _DropData(Vector2 atPosition, Variant data)
	{
		InventorySlot incomingSlot = (InventorySlot) data;
		
		containedMod = incomingSlot.GetContainedMod();
		Texture = containedMod.bigSpriteBlue;
		
		EquipInSlot();
	}
	
	private void EquipInSlot()
	{
		
	}
}
