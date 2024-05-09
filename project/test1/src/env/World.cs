using Godot;
using System;
using NotNot;
using test1.src.lib;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using test1.src.lib.DI;


namespace test1;

[Tool]
public partial class World : Node3D, IEzNode
{
   //[EzAddChild]
   public Map Map; //=new Map();// { get; private set; }

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

   GodotNotifications _lastNotification;

   public override void _Notification(int what)
   {
      base._Notification(what);

      var currentWhat = (GodotNotifications)what;

      if (currentWhat != _lastNotification)
      {
         switch (currentWhat)
         {
            case GodotNotifications.Node_WMMouseEnter:
            case GodotNotifications.Node_WMMouseExit:
            case GodotNotifications.Node_ApplicationFocusIn:
            case GodotNotifications.Node_ApplicationFocusOut:
            case GodotNotifications.Node_WMWindowFocusIn:
            case GodotNotifications.Node_WMWindowFocusOut:
               return;
         }

         _lastNotification = currentWhat;
         this._PrintTrace($"._Notification({currentWhat}:{what}) @ {DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss.fff")}");
      }
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

      var floor = new Floor();
      AddChild(floor);

      //var floor = new CsgBox3D();
      //floor.Size = new(1000, 2, 1000);
      //floor.Position = new(0, -1, 0);
      //floor._EzSetAlbedoTexture("res://assets/textures/grids/Dark/texture_05.png");
      //floor.UseCollision = true;
      //AddChild(floor);

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
      {
         //ramp
         var ramp = new CsgBox3D();
         ramp.Size = new(2, 0.5f, 4);
         ramp.Position = new(5, 0.25f, 0);
         ramp.RotateX(Mathf.DegToRad(30));
         ramp._EzSetAlbedoTexture("res://assets/textures/grids/Light/texture_09.png");
         ramp.UseCollision = true;
         AddChild(ramp);
      }
      {
         //stair
         var location = new Vector3(-2, 0, 0);
         var steps = 20;
         var stepHeight = 0.2f;
         var stepWidth = 2f;
         var stepDepth = 0.5f;


         for (int i = 0; i < steps; i++)
         {
            var stair = new CsgBox3D();
            stair.Size = new(stepWidth, stepHeight, stepDepth);
            stair.Position = new(location.X, location.Y + (i * stepHeight), location.Z + (i * stepDepth));
            //stair.RotateX(Mathf.DegToRad(30));
            stair._EzSetAlbedoTexture("res://assets/textures/grids/Red/texture_09.png");
            stair.UseCollision = true;
            AddChild(stair);
         }



      }
   }
}

public partial class Floor : CsgBox3D, IEzNode
{
   [EzInject] public TestService testService;

   [EzInject] private ILogger<Floor> _logger;

   public override void _Ready()
   {
      base._Ready();
      Size = new(1000, 2, 1000);
      Position = new(0, -1, 0);
      this._EzSetAlbedoTexture("res://assets/textures/grids/Dark/texture_05.png");
      this.UseCollision = true;

      //_GD.Log("Floor._Ready()", Colors.Red);
      try
      {
         //testService = DI.globalHost.serviceProvider.GetService<TestService>();
         if (_logger is not null)
         {
            _logger._EzDebug("Hello, WORLD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
         }
         else
         {
            _GD.Log("_logger is null!", Colors.Red);
         }

         if (testService is not null)
         {
            testService.Test();
         }
         else
         {
            _GD.Log("testService is null!", Colors.Red);
         }
      }
      catch (Exception ex)
      {
         GD.PrintErr($"Main._Ready() Error!: \n{ex} \n {ex.ToStringDemystified()} \n {ex._ToUserFriendlyString()}");
      }
   }
}