extends CharacterBody2D

var speedVec = Vector2.ZERO
var direction = Vector2.ZERO

var tileWidth = 16
var tileHeight = 13

func _ready():
	speedVec.x = tileWidth
	speedVec.y = tileHeight

func get_input():
	direction = Input.get_vector("Left", "Right", "Up", "Down")

func _unhandled_input(event):
	direction = Input.get_vector("Left", "Right", "Up", "Down")
	move(direction)

func move(dir):
	if (dir.x != 0 && dir.y != 0):
		position += dir * speedVec * sqrt(2)
	else:
		position += dir * speedVec