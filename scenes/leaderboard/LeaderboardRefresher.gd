extends Node

func _ready():
	SilentWolf.Scores.connect("sw_save_score_complete", self, "refresh")
	pass # Replace with function body.

func refresh():
	get_parent().Refresh()
