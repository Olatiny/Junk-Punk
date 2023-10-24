extends Node

@export var invSlots = [null, null, null, null, null]
@export var playerCon: Node

func InsertModAt(mod, idx: int):
	invSlots.SwapInMod(mod)

func FirstFreeSpace():
	for i in invSlots.size:
		if (!invSlots[i]):
			return i
	return -1
