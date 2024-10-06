class_name StateMachine 
extends Node

@export var current_state: State;
var states: Dictionary = {};

func _ready() -> void:
	for child in get_children():
		if child is State:
			# Add the state to the dictionary using its name
			states[child.name] = child;
			
			# Connect the state machine to the transitioned signal of all children
			child.transitioned.connect(on_child_transitioned);
		else:
			push_warning("State machine contains child which is not in 'State'");
	
	# Start execution of the initial state
	current_state.enter();

func on_child_transitioned(new_state_name: StringName) -> void:
	# Get the next state from the dictionary
	var new_state = states.get(new_state_name);
	
	if new_state != null:
		if new_state != current_state:
			# Exit the current state
			current_state.exit();
			
			# Enter the new state
			new_state.enter();
			
			# Update current state to new state
			current_state = new_state;
	else:
		push_warning("Called transition to a state that does not exist");

func _process(delta) -> void:
	current_state.update(delta);

func _physics_process(delta: float) -> void:
	current_state.physics_update(delta);
