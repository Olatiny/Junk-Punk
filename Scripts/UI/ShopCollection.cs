using Godot;
using System;

public partial class ShopCollection : Node
{
	[Export] private ShopSlot[] shopSlots = { null, null, null, null, null };
	[Export] private GameManager gameMana;
	private InventoryCollection invCol;

	private ModDatabase modDatabase;

	public override void _Ready()
	{
		base._Ready();
		modDatabase = GetNode<ModDatabase>("/root/ModDatabase");
	}

	public void RandomizeShop()
	{
		invCol = gameMana.GetCurrentPlayer().inventoryCollection;
		
		for (int i = 0; i < shopSlots?.Length; i++)
		{
			Mod targetMod = modDatabase.GetRandomMod();
			shopSlots?[i].SetContainedMod(targetMod);
		}
	}

	public void SelectShopItem(int slotID, Mod modBought)
	{
		PlayerController playerCon = gameMana.GetCurrentPlayer();
		
		if (playerCon.currentScrap < modBought.cost)
		{
			GD.Print("Not enough Scrap!");
			CancelPurchase(slotID, modBought);
			return;
		}
		
		int openSlotIdx = invCol.FirstFreeSpace();
		if (openSlotIdx < 0)
		{
			GD.Print("No Empty Slots to Buy into!");
			CancelPurchase(slotID, modBought);
			return;
		}
		
		ConfirmPurchase(slotID, modBought, openSlotIdx);
	}
	
	private void ConfirmPurchase(int slotID, Mod modBought, int invSlotID)
	{
		// Purchase was successful.
		GD.Print("Purchase Successful!");
		gameMana.GetCurrentPlayer().SpendScrap(modBought.cost);
		invCol.InsertModAt(modBought, invSlotID);
		shopSlots[slotID].ClearContainedMod();
		gameMana.UpdateScoreBoard();
	}
	
	private void CancelPurchase(int slotID, Mod modBought)
	{
		// Purchase failed.
		GD.Print("Purchase Failed...");
	}
}
