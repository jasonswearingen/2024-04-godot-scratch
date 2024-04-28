using Godot;
using System;
using System.Runtime.CompilerServices;
using GodotEx.Hosting;
using Microsoft.Extensions.DependencyInjection;
using test1.src.lib;

namespace test1;

[Tool]
public partial class Main : Node3D
{
   public World World { get; set; }

   [Inject]
   public TestService testService;

   public object InitTest = null;
   public Main()
   {
      //GD.Print($"Main({this.GetInstanceId()}/{this.GetHashCode()}).ctor()  AutoLoad={Host.Autoload?.IsHostInitialized}");
      this._PrintInfo($".ctor(), IsInsideTree={IsInsideTree()},  AutoLoad={Host.Autoload?.IsHostInitialized} TestService={testService is not null} InitTest={InitTest is not null}");

      if (Engine.IsEditorHint() && IsInsideTree())
      {
         //The Godot Editor has Cold-Reloaded the CSharp project.  force the editor to reload the entire scene
         //this will allow csharp tools to properly initialize

         this._PrintWarn("RELOADING SCENE TRIGGER!");
         //need to use a callback because we are currently in a critical path,
         //and attempting to reload the editor will crash if we do it now.
         Callable.From(() => {
            this._PrintWarn("RELOADING SCENE START");
            
            EditorInterface.Singleton.ReloadSceneFromPath(this.SceneFilePath);
            //this._PrintWarn("RELOADING SCENE DONE");
         }).CallDeferred();
         
      }

   }

   protected override void Dispose(bool disposing)
   {
      
      //GD.PrintRich($"[color=yellow]Main({this.GetInstanceId()}/{this.GetHashCode()}).Dispose({disposing})  AutoLoad={Host.Autoload?.IsHostInitialized}");
      //GD.PrintRich("[color=yellow]")
      this._PrintInfo($".Dispose({disposing})  AutoLoad={Host.Autoload?.IsHostInitialized} TestService={testService is not null} InitTest={InitTest is not null}");
      base.Dispose(disposing);
   }

   public override void _EnterTree()
   {
      InitTest = new();
      this._PrintInfo($"._EnterTree() AutoLoad={Host.Autoload?.IsHostInitialized} TestService={testService is not null} InitTest={InitTest is not null}");
      if (Engine.IsEditorHint())
      {
         Callable.From(() =>
         {
            GD.Print($"Main({this.GetInstanceId()}) => Callable.From  AutoLoad={Host.Autoload?.IsHostInitialized}");
            //Host.Autoload.TryInit();
         });
         //
      }
      var loop = Engine.GetMainLoop();



      base._EnterTree();
   }

   private int _lastNotificationWhat = -1;
   public override void _Notification(int what)
   {
      if (what != _lastNotificationWhat)
      {
         switch ((GodotNotifications)what)
         {
            case GodotNotifications.Node_WMMouseEnter:
            case GodotNotifications.Node_WMMouseExit:
            case GodotNotifications.Node_ApplicationFocusIn:
            case GodotNotifications.Node_ApplicationFocusOut:
            case GodotNotifications.Node_WMWindowFocusIn:
            case GodotNotifications.Node_WMWindowFocusOut:
               return;
         }
         _lastNotificationWhat = what;
         this._PrintTrace($" ._Notification({(test1.src.lib.GodotNotifications)what}:{what}) @ {DateTime.Now.ToString("O")} AutoLoad={Host.Autoload?.IsHostInitialized}");
         //GD.Print($"Main({this.GetInstanceId()})._Notification({(test1.src.lib.GodotNotifications)what}:{what}) @ {DateTime.Now.ToString("O")} AutoLoad={Host.Autoload?.IsHostInitialized}");
      }

      base._Notification(what);
   }


   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {

      this._PrintInfo($"._Ready() AutoLoad={Host.Autoload?.IsHostInitialized} TestService={testService is not null} InitTest={InitTest is not null}");
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
      GD.Print($"Main({this.GetInstanceId()})._Ready() AutoLoad={Host.Autoload?.IsHostInitialized}");
      ///GD.Print($"Engine Singletons = {Engine.GetSingletonList().Join()}");
      var mainLoop = Engine.GetMainLoop();
      //mainLoop.





      var player = new Player();
      AddChild(player);
      try
      {
         testService.Test();
      }
      catch (Exception ex)
      {
         GD.PrintErr($"Main._Ready() Error: {ex.Message}");
      }


   }

   // Called every frame. 'delta' is the elapsed time since the previous frame.
   public override void _Process(double delta)
   {
      //  GD.Print("Executing !!  Main process");

      //testService.Test();
   }
}



