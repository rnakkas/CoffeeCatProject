extends Node

var coffee_score: int = 0;

func increase_coffee_score(count: int) -> void:
	coffee_score += count;
	print("Score: " + str(coffee_score));
