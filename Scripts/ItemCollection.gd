class_name ItemCollection
extends Node

func player_collects_item(object: Object, animation: AnimatedSprite2D, game_manager: Node) -> void:
	_increase_score(object, game_manager);
	
	var tween_1: Tween = object.get_tree().create_tween();
	var tween_2: Tween = object.get_tree().create_tween();
	animation.play("collect");
	tween_1.tween_property(object, "modulate:a", 0, 1.6);
	tween_2.tween_property(object, "position", object.position - Vector2(0, 210), 0.45);
	tween_2.tween_callback(object.queue_free);

func _increase_score(object: Object, game_manager: Node) -> void:
	match object.identifier:
		"coffee":
			game_manager.increase_coffee_score(1);
		"coffee_pot":
			game_manager.increase_coffee_score(5);
