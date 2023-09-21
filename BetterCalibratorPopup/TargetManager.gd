extends GridContainer

var density := 400.0
const target = preload("res://target.tscn")

var targets: Array[CenterContainer] = []

var current_target = 0

var data = {
	cols = 0,
	rows = 0,
	offsets = []
}

# Called when the node enters the scene tree for the first time.
func _ready():
	var col_count = ceili(get_viewport_rect().size.x / density)
	var row_count = ceili(get_viewport_rect().size.y / density)
	
	columns = col_count
	data.cols = col_count
	data.rows = row_count
	
	for i in range(0, col_count*row_count):
		var t = target.instantiate()
		targets.push_back(t)
		add_child(t)

func _process(_delta):
	for i in len(targets):
		if i == current_target:
			targets[i].show_target()
		else:
			targets[i].hide_target()

var just_pressed = false

func _on_gui_input(event):
	if event is InputEventMouseButton && event.is_pressed() && event.get_button_index() == MOUSE_BUTTON_MIDDLE:
		if DisplayServer.window_get_current_screen() == DisplayServer.get_screen_count() - 1:
			DisplayServer.window_set_current_screen(0)
		else:
			DisplayServer.window_set_current_screen(DisplayServer.window_get_current_screen() + 1)
	if event is InputEventMouseButton && event.get_button_index() != MOUSE_BUTTON_MIDDLE:
		if not(just_pressed):
			just_pressed = true
			save_offset(targets[current_target], event.position)
			current_target += 1
		just_pressed = event.is_pressed()
	if current_target >= len(targets):
		save_offset_file()
	
func save_offset(target, pos: Vector2):
	var center = target.position
	center.x += target.size.x/2
	center.y += target.size.y/2
	var offset = center - pos
	data.offsets.push_back([offset.x, offset.y])

func save_offset_file():
	hide()
	var file = FileAccess.open("offsets.json", FileAccess.WRITE_READ)
	file.store_string(JSON.stringify(data))
	get_tree().quit()
