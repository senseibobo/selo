extends CanvasLayer


@export var colorrect: ColorRect


func transition(time: float = 1.0):
	var tween1 = create_tween()
	tween1.tween_property(colorrect, "color", Color.BLACK, time)
	tween1.tween_property(colorrect, "color", Color(0,0,0,0), time)
	var tween2 = create_tween()
	tween2.tween_method(set_volume, 1.0, 0.0, time)
	tween2.tween_method(set_volume, 0.0, 1.0, time)
	await tween1.step_finished


func set_volume(volume: float):
	print(linear_to_db(volume))
	AudioServer.set_bus_volume_db(0, linear_to_db(volume))
