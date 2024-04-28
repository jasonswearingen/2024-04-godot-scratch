using Godot;
using Microsoft.Extensions.DependencyInjection;
using System;
using GodotEx.Hosting;
using test1.src.lib;

/// <summary>
/// The Default DI Host for the project.
/// Automatically loaded upon application launch via Godot's Autoload feature (see Project Settings).
/// for docs on DI Host, see: https://altamkp.github.io/GodotEx/docs/GodotEx.Hosting/Hosting.html#setting-up-an-autoload-host
/// </summary>
[Tool]
public partial class AutoloadHost : Host
{

   public AutoloadHost
   ()
   {
      _GD.Print("AutoloadHost.ctor()", Colors.Orange);
      if (IsInsideTree())
      {
         this.TryInit();
         Autoload = this;
      }
   }// Called when the node enters the scene tree for the first time.
   public override void _Ready()
	{
      GD.Print("AUTOLOAD._Read()11");
      base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

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

      services.AddSingleton<TestService>();
   }

   protected override void Dispose(bool disposing)
   {
      GD.Print("ApplicationHost.Dispose(): " + disposing);
      base.Dispose(disposing);
   }
}
