extends Area2D

@onready var animation: AnimatedSprite2D = $sprite;
@onready var level_manager: Node = %level_manager;

var identifier: String;

func _ready() -> void:
	identifier = "coffee_pot";
	animation.play("idle");

func _on_body_entered(body: Node2D) -> void:
	if body.name == "player":
		var item_collection: ItemCollection = ItemCollection.new()
		item_collection.player_collects_item(self, animation, level_manager);
