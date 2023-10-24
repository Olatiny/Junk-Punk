extends TextureRect

var containedMod: Mod
var isDragging: bool = false
@export var slotID: int

# Called when the player initiates a drag on this control.
func _get_drag_data(at_position):
	print(containedMod)
	if (!containedMod):
		return
	
	isDragging = true
	
	var previewTexture = TextureRect.new()
	
	previewTexture.texture = texture
	previewTexture.expand_mode = 1
	previewTexture.size = Vector2(16, 16)
	
	var preview = Control.new()
	preview.add_child(previewTexture)
	
	set_drag_preview(preview)
	texture = null
	
	return self

# Called when the current drag hovers over this control.
# Returns whether this control is a valid candidate for being dropped on.
func _can_drop_data(_pos, data):
	var canDrop: bool = !isDragging && data.is_in_group("INVENTORY")
	return canDrop

# Called when the player releases a drag on top of this control.
func _drop_data(_pos, data):
	print("Data is:")
	print(data)
	var swappedMod = SwapInMod(data.containedMod)
	print("Swapped Mod was:")
	print(swappedMod)
	data.SwapInMod(swappedMod)
	
	print("Drop Data Incoming")
	print(data.containedMod);
	print(containedMod);
	print("Drop Data Finished!")

# Called by every script when a notification occurs.
func _notification(callback):
	match callback:
		NOTIFICATION_DRAG_END:
			NotificationDragEnd()

func NotificationDragEnd():
	# Only process the drag if this script was the thing BEING dragged.
	if (!isDragging):
		return
	
	isDragging = false
	
	if (is_drag_successful()):
		print("Notification Fired!")
		# containedMod = null
	else:
		texture = containedMod.icon

# Assigns the given mod to this slot, then returns whatever mod it previously
# contained (for the purposes of swapping).
func SwapInMod(newMod):
	print("Swapping!")
	print(newMod)
	var oldMod = containedMod
	containedMod = newMod
	
	# This is placeholder, change when mods are more than just a Texture2D
	texture = containedMod.icon
	
	return oldMod
