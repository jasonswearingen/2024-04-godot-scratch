using Godot;
using GodotEx.Hosting;
using System;
using NotNot;
using test1;

//[SceneTree]
[Tool]
public partial class Player : CharacterBody3D
{
   //public Player()
   //{
   //  }
   
	Node3D modelInstance;

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
		//modelInstance = charModelScene.Instantiate<Node3D>();
		modelInstance = this.FindChild("mixamo_base") as Node3D;

      //godot "forward" is +Z, so rotate the model to face -Z
	//	modelInstance.RotateY(Mathf.DegToRad(180));
		//AddChild(modelInstance);

      cameraMount = new Node3D();
		cameraMount.Name = "CameraMount";
		cameraMount.Position = new Vector3(0, 1.4f, 0);
		AddChild(cameraMount);

		//var animationPlayer = modelInstance._FindChild<AnimationPlayer>();

		//"@CharacterBody3D@10/mixamo_base/AnimationPlayer";



      camera = new Camera3D();
      camera.Position = new Vector3(0.75f, 0.25f, 1.75f);
		//camera.RotateY(Mathf.DegToRad(180));
		cameraMount.AddChild(camera);


		//handle mouse
		if (Engine.IsEditorHint() is false)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
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
         cameraMount.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * GameSettings.player_rotate_mouse_sensitivity.Y));
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
   }



   public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint())
		{
			return;
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
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			//testService.Test();
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
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