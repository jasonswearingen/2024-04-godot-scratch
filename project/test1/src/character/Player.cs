using Godot;
using GodotEx.Hosting;
using System;
using NotNot;
using test1;
using GodotEx;

[SceneTree]
[Tool]
public partial class Player : CharacterBody3D
{
	public Player()
	{
      this._TryHotReload();
   }

	public static string _SceneFileName = "res://src/character/Player.tscn";

	public static Player Instantiate()
   {
      var playerScene = ResourceLoader.Load<PackedScene>(_SceneFileName);
      var player = playerScene.Instantiate<Player>();
		return player;
   }

   
	//Node3D modelInstance;

   public override void _Ready()
   {
      base._Ready();


		var collisionShape = new CollisionShape3D();
		var cylinder = new CylinderShape3D();
		cylinder.Radius = 0.3f;		
		collisionShape.Shape = cylinder;
      collisionShape.Position = new Vector3(0, 1, 0);		
      AddChild(collisionShape);

		//var charModelScene = ResourceLoader.Load<PackedScene>("res://assets/models/mixamo_base.glb");
		//var modelInstance = charModelScene.Instantiate<Node3D>();      
		//AddChild(modelInstance);
		var modelInstance = _.visuals.mixamo_base.Get();

      //godot "forward" is +Z, so rotate the model to face -Z
      modelInstance.RotateY(Mathf.DegToRad(180));

		//set idle anim by default
      _.visuals.mixamo_base.AnimationPlayer.Play(AnimationNames.Idle);

      cameraMount = new Node3D();
		cameraMount.Name = "CameraMount";
		cameraMount.Position = new Vector3(0, 1.4f, 0);
		AddChild(cameraMount);

		//var animationPlayer = modelInstance._FindChild<AnimationPlayer>();

		//"@CharacterBody3D@10/mixamo_base/AnimationPlayer";



      camera = new Camera3D();
      camera.Position = new Vector3(0.75f, 0.25f,- 1.75f);
		camera.RotateY(Mathf.DegToRad(180));
		cameraMount.AddChild(camera);


		//handle mouse
		if (Engine.IsEditorHint() is false)
		{
			//Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		
   }

	


   public override void _Input(InputEvent @event)
   {
      base._Input(@event);
		if(@event is InputEventMouseMotion mouseMotion)
		{
			RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * GameSettings.player_rotate_mouse_sensitivity.X));
         cameraMount.RotateX(Mathf.DegToRad(mouseMotion.Relative.Y * GameSettings.player_rotate_mouse_sensitivity.Y));
      }
   }

	Node3D cameraMount;
   Camera3D camera;

   public override void _Process(double delta)
   {
      base._Process(delta);

		using (DebugDraw3D.NewScopedConfig().SetThickness(0.001f))
		{
			DebugDraw3D.DrawCameraFrustum(camera, Colors.Red);
		}
      using (DebugDraw3D.NewScopedConfig().SetThickness(0.01f))
      {

         var visuals = _.visuals.Get();
			//var toOrigin 
         DebugDraw3D.DrawArrow(visuals.GlobalPosition, visuals.GlobalPosition + visuals.GlobalForward(), Colors.Orange);
         DebugDraw3D.DrawArrow(visuals.GlobalPosition, Vector3.Zero, Colors.White,0.1f);
			//DebugDraw3D.DrawPosition(visuals.GlobalTransform, Colors.Yellow);
			DebugDraw3D.DrawGizmo(visuals.GlobalTransform, Colors.Green);


         //draw global coord system

			//z
         var start = this.GlobalPosition + Vector3.Back *2;
         var zEnd = start + (Vector3.Back * 2);
         var yEnd = start + Vector3.Up * 2;
			var xEnd = start + Vector3.Right * 2;
         DebugDraw3D.DrawArrow(start, zEnd, Colors.Aqua,0.1f);
         DebugDraw3D.DrawArrow(start, yEnd, Colors.Green, 0.1f);
         DebugDraw3D.DrawArrow(start, xEnd, Colors.Red, 0.1f);

			DebugDraw3D.DrawArrow(this.GlobalPosition, this.GlobalForward(), Colors.Pink);
      }
   }



   public float Speed = 3.0f;
	public const float JumpVelocity = 4.5f;
	public const float WalkingSpeed = 3.0f;
	public const float RunningSpeed = 5.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{

		if (Engine.IsEditorHint())
		{
			return;
		}
		bool isRunning = false;
		if (Input.IsActionPressed(Imb.P1Run))
		{
			isRunning = true;
			Speed = RunningSpeed;
		}
		else
		{
			Speed = WalkingSpeed;
		}

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector(Imb.P1StrafeLeft,Imb.P1StrafeRight,Imb.P1Forward,Imb.P1Backward);
		Vector3 direction = -(Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			//testService.Test();
			//_.visuals.mixamo_base.AnimationPlayer.Play()

			if (isRunning)
			{
            _.visuals.mixamo_base.AnimationPlayer.Play(AnimationNames.Running);
			}
			else
			{
            _.visuals.mixamo_base.AnimationPlayer.Play(AnimationNames.Walking);
         }

         var target = GlobalPosition + direction;

			DebugDraw3D.DrawSphere(target,0.5f, Colors.Purple);
         var visuals = _.visuals.Get();
         _.visuals.Get().LookAt(target);

			var current = visuals.GlobalPosition - visuals.Forward();
   
			//var visualForward = visuals.Forward();
			//var visualsGlobalForward = visuals.GlobalForward();
			//var current = Position;// - visuals.Forward();// Position;// visuals.Forward();
			//var next = StatelessLerp(current, target);
			////_.visuals.Get().LookAt(Position + direction);
			//next = current.MoveToward(target, 0.05f);
   //      visuals.LookAt(current+ visuals.GlobalTransform.Forward());
   //      //visuals.GlobalForward()
         
			

         velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
      {
         _.visuals.mixamo_base.AnimationPlayer.Play(AnimationNames.Idle);
velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		DebugDraw2D.SetText($"velocity:{velocity:F2}");
	}

   public static Vector3 StatelessLerp(Vector3 current, Vector3 target, float minimumRotationRadians=0.1f, float maximumRotationRadians=1f, float springynessPercent=0.5f)
   {
      // Calculate original magnitudes of the current and target vectors
      float currentMagnitude = current.Length();
      float targetMagnitude = target.Length();

      // Normalize input vectors to ensure proper angle calculations
      current = current.Normalized();
      target = target.Normalized();

      // Calculate the angle between the current and target vectors
      float angleBetween = current.AngleTo(target);

      // Calculate the rotation step, apply springiness percent and clamp it within the specified minimum and maximum rotation bounds
      float rotationStep = Mathf.Clamp(springynessPercent * angleBetween, minimumRotationRadians, maximumRotationRadians);

      // If rotation step is very small and magnitudes are close, return the current vector directly adjusted to the target magnitude
      if (rotationStep < 0.0001f && Mathf.IsEqualApprox(currentMagnitude, targetMagnitude))
      {
         return current * targetMagnitude;
      }

      // Compute rotation axis using the cross product of the current and target vectors
      Vector3 rotationAxis = current.Cross(target).Normalized();

      // Create a quaternion for the rotation
      Quaternion rotation = new Quaternion(rotationAxis, rotationStep);

      // Apply the rotation to the current vector using quaternion multiplication
      Vector3 rotatedVector = rotation * current;

      // Interpolate between the current magnitude and the target magnitude using the springinessPercent
      float interpolatedMagnitude = Mathf.Lerp(currentMagnitude, targetMagnitude, springynessPercent);

      // Apply the interpolated magnitude back to the rotated vector
      return rotatedVector * interpolatedMagnitude;
   }
}

public static class AnimationNames
{
   public static readonly StringName Walking = "walking";
   public static readonly StringName Idle = "idle";
   public static readonly StringName Running = "running";
   public static readonly StringName knock_down = "knock_down";
	public static readonly StringName get_up = "get_up";
	public static readonly StringName kick = "kick";

}

[InputMap]
public static partial class Imb { }

public static class GameSettings
{
	public static class Keys
   {
      public static readonly StringName player_rotate_mouse_sensitivity_x = "player/rotate_mouse_sensitivity_x";
      public static readonly StringName player_rotate_mouse_sensitivity_y = "player/rotate_mouse_sensitivity_y";
   }

   public static float player_rotate_mouse_sensitivity_x { get;  } = ProjectSettings.GetSetting(Keys.player_rotate_mouse_sensitivity_x).AsSingle();
   public static float player_rotate_mouse_sensitivity_y { get;  } = ProjectSettings.GetSetting(Keys.player_rotate_mouse_sensitivity_y).AsSingle();
	public static Vector2 player_rotate_mouse_sensitivity { get;  } = new(GameSettings.player_rotate_mouse_sensitivity_x, GameSettings.player_rotate_mouse_sensitivity_y);
}