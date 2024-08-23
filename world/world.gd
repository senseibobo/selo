extends Node3D


const music: Array[AudioStream] = [
		preload("res://sound/music/bossbudgenrun.ogg"),
		preload("res://sound/music/jacoblizottezeropoint.ogg"),
		preload("res://sound/music/jinglepunkscataclysmicmoltencore.ogg"),
		preload("res://sound/music/makaisymphonydragoncastle.ogg"),
]


# Called when the node enters the scene tree for the first time.
func _ready():
	Leaderboard.visible = true
	DeathOverlay.disable()
	$MusicPlayer.stream = music.pick_random()
	$MusicPlayer.play()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
