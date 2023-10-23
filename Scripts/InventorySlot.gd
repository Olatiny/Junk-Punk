extends TextureRect

var containedMod
var isDragging = false

func _ready():
	if (texture):
		containedMod = texture

# Called when the player initiates a drag on this control.
func _get_drag_data(at_position):
	print(containedMod)
	if (!containedMod):
		return
	
	isDragging = true
	
	var preview_texture = TextureRect.new()
	
	preview_texture.texture = texture
	preview_texture.expand_mode = 1
	preview_texture.size = Vector2(16, 16)
	
	var preview = Control.new()
	preview.add_child(preview_texture)
	
	set_drag_preview(preview)
	texture = null
	
	return self

# Called when the current drag hovers over this control.
# Returns whether this control is a valid candidate for being dropped on.
func _can_drop_data(_pos, data):
	var can_drop: bool = !isDragging && data.is_in_group("INVENTORY")
	return can_drop

# Called when the player releases a drag on top of this control.
func _drop_data(_pos, data):
	var temp = containedMod
	containedMod = data.containedMod
	data.containedMod = temp
	
	texture = containedMod
	data.texture = data.containedMod
	
	print("Drop Data Incoming")
	print(data.containedMod);
	print(containedMod);
	print("Drop Data Finished!")
	

# Called by every script when a notification occurs.
func _notification(what):
	match what:
		NOTIFICATION_DRAG_END:
			notification_drag_end()

func notification_drag_end():
	# Only process the drag if this script was the thing BEING dragged.
	if (!isDragging):
		return
	
	isDragging = false
	
	if (is_drag_successful()):
		print("Notification Fired!")
		# containedMod = null
	else:
		texture = containedMod
