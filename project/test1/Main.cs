using Godot;
using System;
using System.Runtime.CompilerServices;

namespace test1;

[Tool]
public partial class Main : Node3D
{
   public World World { get; set; }
   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {
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



