class_name Player extends CharacterBody2D

const SPEED: float = 400.0;
const JUMP_VELOCITY: float = -500.0;
const GRAVITY: float = 1000.0;
const WALL_SLIDE_GRAVITY: float = 800.0;
const WALL_JUMP_VELOCITY: float = -400.0;
const WALL_JUMP_VELOCITY_BAD: float = -200.0;

@onready var animation: AnimatedSprite2D = $sprite;
@onready var left_wall_detect: RayCast2D = $left_wall_detect;
@onready var right_wall_detect: RayCast2D = $right_wall_detect;

enum STATE {IDLE, RUN, RUN_FAST, JUMP, WALL_SLIDE, FALL, WALL_JUMP, WALL_JUMP_BAD};
var current_state;

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
		STATE.RUN_FAST:
			pass;
		STATE.JUMP:
			pass;
		STATE.WALL_SLIDE:
			pass;
		STATE.FALL:
			pass;
		STATE.WALL_JUMP:
			pass;
		STATE.WALL_JUMP_BAD:
			pass;
	
func _enter_state() -> void:
	match current_state:
		STATE.IDLE:
			#velocity.x = move_toward(velocity.x, 0, SPEED);
			animation.play("idle");
		STATE.RUN:
			animation.play("run");
		STATE.RUN_FAST:
			animation.play("run_fast");
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
		STATE.WALL_JUMP_BAD:
			print("wall jump bad")
			velocity.y = WALL_JUMP_VELOCITY_BAD;
			animation.play("jump");

func _update_state(delta: float) -> void:
	var direction := Input.get_axis("move_left", "move_right");
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
				#velocity.y = move_toward(velocity.y, 0, WALL_SLIDE_GRAVITY)
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
			
			# Flip sprite to face opposite of wall
			if left_wall_detect.is_colliding():
				animation.flip_h = false;
			elif right_wall_detect.is_colliding():
				animation.flip_h = true;
			
			if Input.is_action_just_pressed("jump"):
				_set_state(STATE.WALL_JUMP_BAD);
			
			if (left_wall_detect.is_colliding() && direction > 0 && Input.is_action_just_pressed("jump")) \
				|| \
				(right_wall_detect.is_colliding() && direction < 0 && Input.is_action_just_pressed("jump")):
					_set_state(STATE.WALL_JUMP);
			elif is_on_floor():
				_set_state(STATE.IDLE);

			move_and_slide();
		
		STATE.WALL_JUMP:
			velocity.x = direction * SPEED;
			_flip_sprite(direction);
			
			if !is_on_floor():
				velocity.y += GRAVITY * delta;
				if velocity.y > 0:
					_set_state(STATE.FALL);
			
			move_and_slide();
			
		STATE.WALL_JUMP_BAD:
			velocity.x = direction * SPEED;
			_flip_sprite(direction);
			
			if !is_on_floor():
				velocity.y += GRAVITY * delta;
				if velocity.y > 0:
					_set_state(STATE.FALL);
			
			move_and_slide();

func _flip_sprite(direction: float) -> void:
	if direction < 0:
		animation.flip_h = true;
	elif direction > 0:
		animation.flip_h = false;

func _physics_process(delta: float) -> void:
	_update_state(delta);
