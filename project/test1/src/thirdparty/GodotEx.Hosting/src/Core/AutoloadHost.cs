using Godot;
using Microsoft.Extensions.DependencyInjection;
using System;
using GodotEx.Hosting;
using NotNot.GodotNet;
using test1.lib;
using test1.src.lib;

namespace GodotEx.Hosting;

/// <summary>
/// The Default DI Host for the project.
/// you should configure this to Automatically load upon application launch via Godot's Autoload feature (see Project Settings).
/// for docs on DI Host, see: https://altamkp.github.io/GodotEx/docs/GodotEx.Hosting/Hosting.html#setting-up-an-autoload-host
/// </summary>
[Tool]
public partial class AutoloadHost : Host
{
   /// <summary>
   /// Autoload host instance configured in Project Settings.
   /// </summary>
   public static AutoloadHost Instance { get; protected set; }
    
   
   public AutoloadHost()
   {
      if(Instance is not null)
      {
         throw new Exception("AutoloadHost.Instance already exists.  this is supposed to be a singleton autoload configured in your Godot Project Settings");
      }
      Instance = this;

      this._Log("AutoloadHost.ctor()", Colors.DarkOrange);

      if (Engine.IsEditorHint() && IsInsideTree())
      {
         //We are running in the editor, and even though this object was just .CTOR'd,
         //we are already inside the SceneTree.
         //CAUSE: The Godot Editor has Cold-Reloaded the CSharp project,
         //which is an effect of building the .csproj externally (By Visual Studio)

         //this has caused all managed (Csharp) objects to be recreated,
         //but godot has NOT reinitialized them, and indeed the native godot objects are still initialized properly.
         //this leaves managed objects broken, and they need to be reinitialized.
         //the only way to do this is to force the Godot Editor to reload the entire scene
         //this will allow csharp tools to properly initialize,
         //as the native godot objects reinitialize (which drives the managed objects)

         //need to use a callback because we are currently in a critical path,
         //and attempting to reload the editor will crash if we do it now.
         var immediateReload = false;
         _Engine.ReloadScene(immediateReload);

         //however this "AutoloadHost" is loaded prior to the scene,
         //so it will not be fixed when the scene reloads.
         //it is still broken, as the entire managed assembly has been cold-reloaded.
         //so before our "CallDeferred" scene reload occurs (above), we need to manually reinitialize this AutoloadHost
         //so that it's DI capabilities are available prior to the scene reload.

         //i'm not sure why this can't be inside the "CallDeferred", but it doesn't work there.
         //I think it's because the init has to happen in the same stack call as the .ctor() call.

         this._DoInit();
      }


      //if (IsInsideTree() && Engine.IsEditorHint())
      //{
      //   this.TryInit();
      //   Autoload = this;
      //}
   }

   protected override void _DoInit()
   {
      this._PrintWarn("AutoloadHost._DoInit()");
      base._DoInit();

      Engine.RegisterSingleton(this.Name, this);
   }

   /// <summary>
   /// Called when the node enters the scene tree for the first time.
   /// however in cold-load, this is not called.
   /// </summary>
   public override void _Ready()
   {
      GD.Print("AUTOLOAD._Read()11");
      base._Ready();
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
      this._Log($"AutoloadHost.Dispose({disposing}) ",Colors.DarkOrange);
      Engine.UnregisterSingleton(this.Name);
      base.Dispose(disposing);
      Instance = null!;
   }
}
