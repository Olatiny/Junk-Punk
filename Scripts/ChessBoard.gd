extends TileMap

var grid = []
var gridWidth = 8
var gridHeight = 8

var tileWidth = 16
var tileHeight = 13

# Called when the node enters the scene tree for the first time.
func _ready():
	for i in gridWidth:
		grid.append([])
		for j in gridHeight:
			grid[i].append(null)
	pass # Replace with function body.

# func RequestMove(obj, dir):
# # Called every frame. 'delta' is the elapsed time since the previous frame.
# #func _process(delta):
# 	pass