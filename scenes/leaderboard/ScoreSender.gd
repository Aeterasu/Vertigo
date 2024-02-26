extends Node

var score_id

func _ready():
	pass # Replace with function body.

func send_score(player_name, score):
	score_id = yield(SilentWolf.Scores.persist_score(player_name, score), "sw_score_posted")
