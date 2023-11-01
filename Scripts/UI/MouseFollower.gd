extends Node

@export var textLbl : RichTextLabel

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var tooltipOff = Vector2(0, 0)
	var padding = Vector2(5, -5)
	var mousePos = get_viewport().get_mouse_position()
	var screenSize = get_viewport().get_visible_rect().size

	if mousePos.x + self.size.x > screenSize.x:
		# Hug Left
		#tooltipOff.x = screenSize.x - (mousePos.x + self.size.x)
		tooltipOff.x = -self.size.x
	if mousePos.y + self.size.y > screenSize.y:
		# Hug Top
		tooltipOff.y = -self.size.y
	self.position = mousePos + tooltipOff + padding

func display_text(dispTxt : String):
	if (dispTxt != null):
		print("HELLO I AM GOING INSANE")
		print(textLbl)
		
		textLbl.text = dispTxt

func show_tooltip(doShow : bool):
	self.visible = doShow
	print("  Hello!")
