using Godot;
using System;

public partial class InventorySlot : TextureRect
{
	private Mod containedMod;
	private bool isDragging = false;
	[Export] private int slotID = 0;
	GameManager gameMana;

	bool mouseOver = false;

	public override void _Ready()
	{
		base._Ready();

		gameMana = GetNode<GameManager>("/root/InventoryShop/GameManager");
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	// Called when the player initiates a drag on this control.
	public override Variant _GetDragData(Vector2 atPosition)
	{
		GD.Print(containedMod == null);
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
		Node dataNode = (Node)data;
		bool canDrop = !isDragging && dataNode.IsInGroup("INVENTORY");
		return canDrop;
	}

	// Called when the player releases a drag on top of this control.
	public override void _DropData(Vector2 atPosition, Variant data)
	{
		InventorySlot incomingSlot = (InventorySlot)data;

		Mod swappedMod = SwapInMod(incomingSlot.containedMod);

		incomingSlot.SwapInMod(swappedMod);
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

		if (!IsDragSuccessful())
		{
			GD.Print("Drag Failed. Restoring Inventory Slot's texture.");
			Texture = containedMod.icon;
		}
	}

	// Assigns the given mod to this slot, then returns whatever mod it previously
	// contained (for the purposes of swapping).

	public Mod SwapInMod(Mod newMod)
	{
		Mod oldMod = containedMod;
		containedMod = newMod;
		if (containedMod != null)
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

	private void OnMouseEntered()
	{
		mouseOver = true;

		Mod slotContainedMod = GetContainedMod();

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
}
