using Godot;
using GodotEx.Hosting;
using System;
using NotNot.GodotNet;

[Tool]
public partial class Player : CharacterBody3D
{
   //public Player()
   //{
   //  }

   [Inject]
   public TestService testService;
   public override void _Ready()
   {
      base._Ready();


		var collisionShape = new CollisionShape3D();
		var cylinder = new CylinderShape3D();
		cylinder.Radius = 0.3f;		
		collisionShape.Shape = cylinder;
      collisionShape.Position = new Vector3(0, 1, 0);		
      AddChild(collisionShape);

		var visuals = new Node3D();
		var charModelScene = ResourceLoader.Load<PackedScene>("res://assets/models/mixamo_base.glb");
		var modelInstance = charModelScene.Instantiate<Node3D>();
		//godot "forward" is +Z, so rotate the model to face -Z
		modelInstance.RotateY(Mathf.DegToRad(180));
		AddChild(modelInstance);

		var cameraMount = new Node3D();
		cameraMount.Name = "CameraMount";
		cameraMount.Position = new Vector3(0, 1.4f, 0);
		AddChild(cameraMount);

		
      camera = new Camera3D();
      camera.Position = new Vector3(0.75f, 0.25f, 1.75f);
		//camera.RotateY(Mathf.DegToRad(180));
		cameraMount.AddChild(camera);
		
   }
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
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			testService.Test();
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
