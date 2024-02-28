extends AudioStreamPlayer

var rain : CPUParticles2D

func _ready():
	rain = get_parent()
	
func _physics_process(delta):
	if (rain.emitting && !playing):
		play()
