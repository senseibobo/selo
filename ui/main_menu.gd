extends Control


@export var gong: AudioStreamPlayer

# Called when the node enters the scene tree for the first time.
func _ready():
	Leaderboard.visible = false
	DeathOverlay.disable()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_play_pressed():
	gong.play()
	await Transition.transition(4.0)
	get_tree().change_scene_to_file("res://world/world.tscn")
