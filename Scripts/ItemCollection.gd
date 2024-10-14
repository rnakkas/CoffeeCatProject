class_name ItemCollection
extends Node

# When player collects item, increase score and play animation
func player_collects_item(object: Object, _animation: AnimatedSprite2D, level_manager: Node) -> void:
	_increase_score(object, level_manager);
	
	var tween_1: Tween = object.get_tree().create_tween();
	var tween_2: Tween = object.get_tree().create_tween();
	#animation.play("collect");
	tween_1.tween_property(object, "modulate:a", 0, 0.4);
	tween_2.tween_property(object, "position", object.position - Vector2(0, 150), 0.5);
	tween_2.tween_callback(object.queue_free);

func _increase_score(object: Object, level_manager: Node) -> void:
	match object.identifier: #Gets the object identifier field
		"coffee":
			level_manager.increase_coffee_score(1);
		"coffee_pot":
			level_manager.increase_coffee_score(5);
