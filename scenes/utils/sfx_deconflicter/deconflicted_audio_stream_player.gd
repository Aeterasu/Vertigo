extends AudioStreamPlayer
## AudioStreamPlayer which ensures multiple copies of the same sound don't play simultaneously.

func _ready() -> void:
	add_to_group("DestroyOnRoomUpdate");
	connect("finished", self, "queue_free")

func play(from_position = 0.0) -> void:
	if SfxDeconflicter.should_play(self):
		.play(from_position)
