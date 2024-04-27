using Godot;
using System;

namespace test1;

[Tool]
public partial class World : Node3D
{

   public Map Map { get; private set; }

   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {
      Map = new Map();
      AddChild(Map);

      var sun = new DirectionalLight3D();
      sun.SkyMode = DirectionalLight3D.SkyModeEnum.LightAndSky;
      sun.DirectionalShadowMode = DirectionalLight3D.ShadowMode.Parallel4Splits;
      sun.LightEnergy = 1.0f;
      sun.LightColor = new(1f, 0.9f, 0.8f);
      sun.RotationDegrees = new(-60, 150, 0);
      sun.ShadowEnabled = true;
      AddChild(sun);

      //6:42
      var worldEnv = new WorldEnvironment();
      //enable SDFGI
      worldEnv.Environment = new();
      worldEnv.Environment.SdfgiEnabled = true;
      worldEnv.Environment.FogEnabled = true;
      worldEnv.Environment.FogDensity = 0.001f;
      worldEnv.Environment.FogLightColor = Colors.White;



      AddChild(worldEnv);


   }

   // Called every frame. 'delta' is the elapsed time since the previous frame.
   public override void _Process(double delta)
   {
      //   GD.Print("Executing !!  World process");
   }
}

public partial class Map : Node3D
{


   //public Map()
   //{
   //   //var floor = new CsgBox3D();
   //   //floor.Size = new(1000, 2, 1000);
   //   //floor.Position = new(0, -1, 0);
   //   //AddChild(floor);
   //}

   public override void _Ready()
   {
      base._Ready();
      GD.Print("Executing !!  Map2");

      var floor = new CsgBox3D();
      floor.Size = new(1000, 2, 1000);
      floor.Position = new(0, -1, 0);
      floor._EzSetAlbedoTexture("res://assets/textures/grids/Dark/texture_05.png");
      floor.UseCollision = true;
      AddChild(floor);

      {
         var box = new CsgBox3D();
         box.Size = new(1, 1, 1);
         box.Position = new(4.5f, 0.5f, 5);
         box.RotateY(Mathf.DegToRad(45));
         box._EzSetAlbedoTexture("res://assets/textures/grids/Orange/texture_09.png");
         box.UseCollision = true;
         AddChild(box);
      }
      {
         //4:40
         var box = new CsgBox3D();
         box.Size = new(2, 2, 2);
         box.Position = new(-5, 1, 5);
         box.RotateY(Mathf.DegToRad(45));
         box._EzSetAlbedoTexture("res://assets/textures/grids/Purple/texture_09.png");
         box.UseCollision = true;
         AddChild(box);
      }
      {
         //5:09
         var box = new CsgBox3D();
         box.Size = new(3, 3, 3);
         box.Position = new(1, 1.5f, -5);
         box.RotateY(Mathf.DegToRad(30));
         box._EzSetAlbedoTexture("res://assets/textures/grids/Green/texture_09.png");
         box.UseCollision = true;
         AddChild(box);
      }
   }
}

public partial class Floor : CsgBox3D
{
   public override void _Ready()
   {
      base._Ready();
      Size = new(1000, 2, 1000);
      Position = new(0, -1, 0);

   }
}