extends CharacterBody2D

var speedVec = Vector2.ZERO
var direction = Vector2.ZERO

var tileWidth = 16
var tileHeight = 13
var inputs = {"Right": Vector2.RIGHT,
			"Left": Vector2.LEFT,
			"Up": Vector2.UP,
			"Down": Vector2.DOWN}

func _ready():
	speedVec.x = tileWidth
	speedVec.y = tileHeight

func _unhandled_input(event):
	for dir in inputs.keys():
		if event.is_action_pressed(dir):
			move(dir)

func move(dir):
	position += inputs[dir] * speedVec
