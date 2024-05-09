using Godot;
using System;
using System.Runtime.CompilerServices;
using GodotEx.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NotNot;
using System.Diagnostics;
using test1.src.lib.DI;


namespace test1;

[SceneTree]
[Tool]
public partial class Main : DotNetScene
{
   public World World { get; set; }



   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {
  
      


      World = new();
      AddChild(World);


      //var player = Player.Instantiate();
      //AddChild(player);

      var cc3d = new CharacterController3d();
      cc3d.Position = new Vector3(-1, 1, -1);
      AddChild(cc3d);

   }

   public override void _Process(double delta)
   {
      base._Process(delta);

      using (DD3d.NewScopedConfig().SetThickness(0.05f))
      {
         //DD3d.DrawGrid(this.target.GlobalPosition, new(10,1,10), new(0,0,0), new(1,1));
         //DD3d.DrawGrid(this.target.GlobalPosition, target.Basis.X * 10, target.Basis.Z * 10, new(10, 10), Colors.Gray);
         DD3d.DrawArrow(Vector3.Zero, Vector3.Back * 10, Colors.Aqua, 0.1f);
         DD3d.DrawArrow(Vector3.Zero, Vector3.Up * 10, Colors.Green, 0.1f);
         DD3d.DrawArrow(Vector3.Zero, Vector3.Right*10, Colors.Red,0.1f);
         //DD3d.DrawArrow(target.GlobalPosition, target.GlobalPosition + target.Forward(), Colors.YellowGreen);
      }
   }
}