extends CanvasLayer

@onready var coffee_score: Node = $coffee_score;
@onready var level_manager: Node = %level_manager;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	coffee_score.text = str(level_manager.coffee_score);
