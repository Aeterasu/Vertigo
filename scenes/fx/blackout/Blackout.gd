extends ColorRect

export var speed : float = 1.0
var alpha : float = 1.0

func _ready():
	pass

func _process(delta):
	alpha = max(alpha - speed * delta, 0.0)
	modulate = Color(1.0, 1.0, 1.0, alpha)
