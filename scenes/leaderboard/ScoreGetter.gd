extends Node

var scores

var finished_refresh : bool

func _ready():
	finished_refresh = false
	pass

func refresh():
	finished_refresh = false
	scores = yield(SilentWolf.Scores.get_high_scores(), "sw_scores_received")
	finished_refresh = true
