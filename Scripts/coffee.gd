class_name Coffee 
extends Area2D

@onready var animation: AnimatedSprite2D = $sprite;

var collect_item : ItemCollection = ItemCollection.new();

func _ready() -> void:
	animation.play("idle");

func _on_body_entered(body: Node2D) -> void:
	## Remove
	#var tween_1: Tween = get_tree().create_tween();
	#var tween_2: Tween = get_tree().create_tween();
	if body.name == "player":
		## Remove
		#print("player enter");
		#animation.play("collect");
		#tween_1.tween_property(self, "modulate:a", 0, 1.5);
		#tween_2.tween_property(self, "position", position - Vector2(0, 220), 0.4);
		#tween_2.tween_callback(queue_free);
		collect_item.player_collects_item(self, animation);
