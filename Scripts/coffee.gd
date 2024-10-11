class_name Coffee 
extends Area2D

@onready var animation: AnimatedSprite2D = $sprite;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	animation.play("idle");


func _on_body_entered(body: Node2D) -> void:
	var tween_1: Tween = get_tree().create_tween();
	var tween_2: Tween = get_tree().create_tween();
	if body.name == "player":
		print("player enter");
		animation.play("collect");
		tween_1.tween_property(self, "modulate:a", 0, 1.5);
		tween_2.tween_property(self, "position", position - Vector2(0, 220), 0.4);
		tween_2.tween_callback(queue_free);
