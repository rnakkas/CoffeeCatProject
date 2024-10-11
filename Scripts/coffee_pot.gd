class_name CoffeePot
extends Area2D

@onready var animation : AnimatedSprite2D = $sprite;

var collect_item : ItemCollection = ItemCollection.new();

func _ready() -> void:
	animation.play("idle");

func _on_body_entered(body: Node2D) -> void:
	if body.name == "player":
		collect_item.player_collects_item(self, animation);
