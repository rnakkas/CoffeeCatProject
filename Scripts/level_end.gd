extends Area2D

@onready var animation: AnimatedSprite2D = $sprite;

var player_entered: bool;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	animation.play("idle");

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	_player_interaction();

func _on_body_entered(body: Node2D) -> void:
	if body.name == "player":
		player_entered = true;
		animation.play("player_entered");

func _on_body_exited(body: Node2D) -> void:
	if body.name == "player":
		player_entered = false;
		animation.play("idle");

func _player_interaction() -> void:
	if player_entered && Input.is_action_just_pressed("move_up"):
		print("entered");
		## TODO: scene transition to level complete scene
