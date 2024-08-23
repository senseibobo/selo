extends Node3D


@onready var label = $Sprite3D/Label3D
@onready var sprite = $Sprite3D


func _ready():
	var x = randi()%3
	var y = randi()%3
	sprite.region_rect = Rect2(x*160,y*160,160,160)
	var tween = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CUBIC)
	tween.tween_property(self, "scale", Vector3.ONE*1.3, 0.1)
	tween.tween_property(self, "scale", Vector3(), 1.0)
	tween.tween_callback(queue_free)


func _process(delta):
	var camera = get_viewport().get_camera_3d()
	if not camera: return
	var dir = sprite.global_position.direction_to(camera.global_position)
	label.global_position = sprite.global_position + dir*0.05
