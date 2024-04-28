using Godot;
using System;
using System.Runtime.CompilerServices;
using GodotEx.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace test1;

[Tool]
public partial class Main : Node3D
{
   public World World { get; set; }
   public ApplicationHost AutoloadHost { get; set; }

   

   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {
     
      //make an autoload DI Host
      //AutoloadHost = new ApplicationHost() { };      
      ////GetTree().Root.
      //GetNode("/root").CallDeferred("add_child", AutoloadHost);
         //AddChild(AutoloadHost);


      World = new();
      AddChild(World);
      //if (Engine.IsEditorHint())
      //{
      //   GD.Print("Executing in the editor");
      //}
      //else
      //{
      //   GD.Print("Executing in the game");
      //}
      //GD.Print("Executing !!  Main");

      var player = new Player();
      AddChild(player);




   }

   // Called every frame. 'delta' is the elapsed time since the previous frame.
   public override void _Process(double delta)
   {
      //  GD.Print("Executing !!  Main process");
   }
}

public partial class ApplicationHost : Host
{
   // This line is required due to a Godot bug that doesn't run _EnterTree() in an external library
   public override void _EnterTree()
   {
      var x = 1;
      x++;
      GD.Print("ApplicationHost._EnterTree()");
      base._EnterTree();
   }

   protected override void ConfigureServices(IServiceCollection services)
   {
      base.ConfigureServices(services);
      // Add your background services here
   }

}


public static class zz_Extensions_CsgBox3D
{
   public static void _EzSetAlbedoTexture(this CsgBox3D csg, string albedoTexture)
   {
      var material = new StandardMaterial3D
      {
         AlbedoTexture = ResourceLoader.Load<Texture2D>(albedoTexture),
         Uv1Triplanar = true,
      };
      csg.Material = material;
   }
}



