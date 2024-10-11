class_name Player extends CharacterBody2D

const SPEED: float = 700.0;
const JUMP_VELOCITY: float = -850.0;
const GRAVITY: float = 1800.0;
const WALL_SLIDE_GRAVITY: float = 1000.0;
const WALL_JUMP_VELOCITY: float = -750.0;

@onready var animation: AnimatedSprite2D = $sprite;
@onready var left_wall_detect: RayCast2D = $left_wall_detect;
@onready var right_wall_detect: RayCast2D = $right_wall_detect;

enum STATE {IDLE, RUN, CAFFEINATED, JUMP, WALL_SLIDE, FALL, WALL_JUMP};
var current_state;
var wall_jump_direction: float;

func _ready() -> void:
	_set_state(STATE.IDLE);

# State machine
func _set_state(new_state: STATE) -> void:
	if new_state == current_state:
		return;
	
	_exit_state();
	current_state = new_state;
	_enter_state();

func _exit_state() -> void:
	match current_state:
		STATE.IDLE:
			pass;
		STATE.RUN:
			pass;
		STATE.CAFFEINATED:
			pass;
		STATE.JUMP:
			pass;
		STATE.WALL_SLIDE:
			pass;
		STATE.FALL:
			pass;
		STATE.WALL_JUMP:
			pass;
	
func _enter_state() -> void:
	match current_state:
		STATE.IDLE:
			#velocity.x = move_toward(velocity.x, 0, SPEED);
			animation.play("idle");
		STATE.RUN:
			animation.play("run");
		STATE.CAFFEINATED:
			animation.play("caffeinated");
		STATE.JUMP:
			velocity.y = JUMP_VELOCITY
			animation.play("jump");
		STATE.WALL_SLIDE:
			print("wall slide");
			animation.play("wall_slide");
		STATE.FALL:
			animation.play("fall");
		STATE.WALL_JUMP:
			print("wall jump");
			velocity.y = WALL_JUMP_VELOCITY;
			animation.play("jump");

func _update_state(delta: float) -> void:
	var direction : float = Input.get_axis("move_left", "move_right");
	match current_state:
		STATE.IDLE:
			if direction:
				_set_state(STATE.RUN);
			elif Input.is_action_just_pressed("jump") && is_on_floor():
				_set_state(STATE.JUMP);
			elif !is_on_floor():
				_set_state(STATE.FALL);
			
		STATE.RUN:
			velocity.x = direction * SPEED;
			_flip_sprite(direction);
			
			if !direction:
				_set_state(STATE.IDLE);
			elif Input.is_action_just_pressed("jump") && is_on_floor():
				_set_state(STATE.JUMP);
			elif !is_on_floor():
				_set_state(STATE.FALL);
			
			move_and_slide()
		
		STATE.JUMP:
			velocity.x = direction * SPEED;
			_flip_sprite(direction);
			
			if !is_on_floor():
				velocity.y += GRAVITY * delta;
				if velocity.y > 0:
					_set_state(STATE.FALL);
			elif is_on_wall():
				_set_state(STATE.WALL_SLIDE);	
				
			move_and_slide()
			
		STATE.FALL:
			velocity.x = direction * SPEED;
			_flip_sprite(direction);
			
			if is_on_floor():
				_set_state(STATE.IDLE);
			elif is_on_wall():
				_set_state(STATE.WALL_SLIDE);
			else:
				velocity.y += GRAVITY * delta;

			move_and_slide()

		STATE.WALL_SLIDE:
			velocity.y += WALL_SLIDE_GRAVITY * delta;
			
			## Flip sprite to face opposite of wall
			#if left_wall_detect.is_colliding():
				#animation.flip_h = false;
			#elif right_wall_detect.is_colliding():
				#animation.flip_h = true;
			
			if Input.is_action_just_pressed("jump"):
				if left_wall_detect.is_colliding():
					wall_jump_direction = 1.0;
				elif right_wall_detect.is_colliding():
					wall_jump_direction = -1.0;
				
				_set_state(STATE.WALL_JUMP);

			elif is_on_floor():
				_set_state(STATE.IDLE);

			move_and_slide();
		
		STATE.WALL_JUMP:
			velocity.x = wall_jump_direction * SPEED;
			_flip_sprite(wall_jump_direction);
			
			if !is_on_floor():
				velocity.y += GRAVITY * delta;
				if velocity.y > 0:
					_set_state(STATE.FALL);
			
			move_and_slide();

func _flip_sprite(direction: float) -> void:
	if direction < 0:
		animation.flip_h = false;
	elif direction > 0:
		animation.flip_h = true;

func _physics_process(delta: float) -> void:
	_update_state(delta);
