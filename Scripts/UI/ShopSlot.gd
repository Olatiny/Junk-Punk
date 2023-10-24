extends Node

var containedMod: Mod
@export var slotID: int
@export var parentShop: Node

func _on_shop_button_pressed():
	parentShop.BuyShopItem(slotID, containedMod)
