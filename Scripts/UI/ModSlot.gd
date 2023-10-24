extends TextureRect

var containedMod = null
var isDragging = false

# Called when the current drag hovers over this control.
# Returns whether this control is a valid candidate for being dropped on.
func _can_drop_data(_pos, data):
	var can_drop: bool = !isDragging && data.is_in_group("INVENTORY")
	return can_drop

# Called when the player releases a drag on top of this control.
func _drop_data(_pos, data):
	containedMod = data.containedMod
	texture = containedMod
	
	print("Drop Data Incoming")
	print(data.containedMod);
	print(containedMod);
	print("Drop Data Finished!")
