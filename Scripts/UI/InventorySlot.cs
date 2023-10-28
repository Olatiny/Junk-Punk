using Godot;
using System;

public partial class InventorySlot : TextureRect
{
	private Mod containedMod;
	private bool isDragging = false;
	[Export] private int slotID = 0;
	
	// Called when the player initiates a drag on this control.
	public override Variant _GetDragData(Vector2 atPosition)
	{
		GD.Print(containedMod);
		
		if (containedMod == null)
			return this;
		
		isDragging = true;
		
		TextureRect previewTexture = new TextureRect();
		
		previewTexture.Texture = Texture;
		previewTexture.Size = new Vector2(16, 16);
		
		Control preview = new Control();
		preview.AddChild(previewTexture);
		
		SetDragPreview(preview);
		Texture = null;
		
		return this;
	}
	
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
		GD.Print("Data is:");
		GD.Print(data);
		
		InventorySlot incomingSlot = (InventorySlot) data;
		
		Mod swappedMod = SwapInMod(incomingSlot.containedMod);
		GD.Print("Swapped Mod was:");
		GD.Print(swappedMod);
		incomingSlot.SwapInMod(swappedMod);
		
		GD.Print("Drop Data Incoming");
		GD.Print(incomingSlot.containedMod);
		GD.Print(containedMod);
		GD.Print("Drop Data Finished!");
	}
	
	// Called when an engine notification occurs.
	public override void _Notification(int callback)
	{
		switch ((long)callback)
		{
			case NotificationDragEnd:
				{
					DragEndNotif();
					break;
				}
		}
	}
	
	private void DragEndNotif()
	{
		// Only process the drag if this script was the thing BEING dragged.
		if (!isDragging)
			return;
			
		isDragging = false;
		
		if (IsDragSuccessful())
		{
			GD.Print("Notification Fired!");
			// containedMod = null;
		}
		else
		{
			Texture = containedMod.icon;
		}
	}
	
	// Assigns the given mod to this slot, then returns whatever mod it previously
	// contained (for the purposes of swapping).
	
	public Mod SwapInMod(Mod newMod)
	{
		GD.Print("Swapping!");
		GD.Print(newMod);
		Mod oldMod = containedMod;
		containedMod = newMod;
		
		// This is placeholder, change when mods are more than just a Texture2D
		Texture = containedMod.icon;
		
		return oldMod;
	}
	
	public Mod GetContainedMod()
	{
		return containedMod;
	}
	
	public bool IsEmpty()
	{
		return containedMod == null;
	}
}