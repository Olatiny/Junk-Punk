using Godot;
using System;

public partial class InventoryCollection : Node
{
	[Export] private InventorySlot[] invSlots = { null, null, null, null, null };
	[Export] private PlayerController playerCon;
	
	public void InsertModAt(Mod mod, int idx)
	{
		invSlots[idx].SwapInMod(mod);
	}
	
	public int FirstFreeSpace()
	{
		for (int i = 0; i < invSlots.Length; i++)
		{
			if (invSlots[i].IsEmpty())
				return i;
		}
		return -1;
	}
}
