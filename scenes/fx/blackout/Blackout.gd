extends ColorRect

export var speed : float = 1.0
var alpha : float = 1.0

func _ready():
	pass

func _process(delta):
	alpha -= speed * delta
	modulate = Color(1.0, 1.0, 1.0, alpha)
