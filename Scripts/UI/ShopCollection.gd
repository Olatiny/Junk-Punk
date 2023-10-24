extends TextureRect

@export var shopSlots = [null, null, null, null, null]
@export var gameMana: Node

func BuyShopItem(slotID: int, modBought: Mod):
	var playerCon = gameMana.GetCurrentPlayer()
	if (playerCon.currentScrap >= modBought.cost):
		# Purchase was successful.
		
	else:
		# Purchase failed.
