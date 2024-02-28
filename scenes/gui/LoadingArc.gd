extends Node2D

func _process(delta):
	rotation_degrees += 360 * delta

func _draw():
	draw_arc(Vector2.ZERO, 16.0, 0.0, 5 * PI / 4, 32, Color(1, 1, 1, 1), 6.0)
