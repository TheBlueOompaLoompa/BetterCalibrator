extends GridContainer

@export var popup: Window
@export var text_box: LineEdit

const target = preload("res://target.tscn")
const EMPTY_CURSOR = preload("uid://xj6v2nvetfyy")

var density := 400.0
var targets: Array[CenterContainer] = []
var current_target = 0

var data = {
	cols = 0,
	rows = 0,
	offsets = []
}

func reset():
	data = {
		cols = 0,
		rows = 0,
		offsets = []
	}
	
	current_target = 0
	targets = []
	
	for child in get_children():
		remove_child(child)
	
	var col_count = ceili(get_viewport_rect().size.x / density)
	var row_count = ceili(get_viewport_rect().size.y / density)
	
	columns = col_count
	data.cols = col_count
	data.rows = row_count
	
	for i in range(0, col_count*row_count):
		var t = target.instantiate()
		targets.push_back(t)
		add_child(t)
	
	Input.set_custom_mouse_cursor(EMPTY_CURSOR, Input.CURSOR_FORBIDDEN)
	mouse_default_cursor_shape = CURSOR_FORBIDDEN

# Called when the node enters the scene tree for the first time.
func _ready():
	reset()
	
	get_viewport().connect("size_changed", func():
		reset()
	)

func _process(_delta):
	for i in len(targets):
		if i == current_target:
			targets[i].show_target()
		else:
			targets[i].hide_target()

var just_pressed = false


func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.get_key_label_with_modifiers() == KEY_ESCAPE:
			_on_close_button_pressed()


func _on_gui_input(event):
	if event is InputEventMouseButton && event.is_pressed() && event.get_button_index() == MOUSE_BUTTON_MIDDLE:
		if DisplayServer.window_get_current_screen() >= DisplayServer.get_screen_count() - 1:
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
	
func save_offset(t: Target, pos: Vector2):
	var center = t.position
	center.x += t.size.x/2
	center.y += t.size.y/2
	var offset = center - pos
	data.offsets.push_back([offset.x, offset.y])


var text_content := ""


func save_offset_file():
	hide()
	mouse_default_cursor_shape = Control.CURSOR_ARROW
	text_content = JSON.stringify(data)
	text_box.text = text_content
	DisplayServer.clipboard_set(text_content)
	DisplayServer.clipboard_set_primary(text_content)
	popup.show()


func _on_close_button_pressed() -> void:
	get_tree().quit()


func _on_line_edit_text_changed(_new_text: String) -> void:
	text_box.text = text_content
