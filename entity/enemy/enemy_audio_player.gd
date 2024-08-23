extends AudioStreamPlayer3D



const sounds = [
	preload("res://sound/vwoman1.ogg"),
	preload("res://sound/vwoman3.ogg"),
	preload("res://sound/vwoman12.ogg"),
	preload("res://sound/vman1.ogg"),
	preload("res://sound/vman2.ogg"),
	preload("res://sound/vgrandpa1.ogg"),
]


func play_sound():
	stream = sounds.pick_random()
	play()
