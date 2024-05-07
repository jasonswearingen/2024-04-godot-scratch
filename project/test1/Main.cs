using Godot;
using System;
using System.Runtime.CompilerServices;
using GodotEx.Hosting;
using Microsoft.Extensions.DependencyInjection;
using test1.src.lib;
using NotNot;

namespace test1;


public static class zz_Extensions_Node
{
   private static bool isReloadRequested = false;
   /// <summary>
   /// <para>should ONLY be called from the constructor of csharp [Tool] nodes.</para>
   /// Ensures proper csharp initialization occurs
   /// </summary>
   public static void _ToolCtorHotReloadInit(this Node node)
   {
      node._PrintInfo(".ctor");
      if (isReloadRequested is false && Engine.IsEditorHint() && node.IsInsideTree())
      {
         node._PrintWarn("EditorHotReloadHelper()  RELOAD REQUESTED!");
         isReloadRequested = true;

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
         //DI.globalHost.Dispose();
         var immediateReload = false;
         _Engine.ReloadScene(immediateReload);

         //however this "AutoloadHost" is loaded prior to the scene,
         //so it will not be fixed when the scene reloads.
         //it is still broken, as the entire managed assembly has been cold-reloaded.
         //so before our "CallDeferred" scene reload occurs (above), we need to manually reinitialize this AutoloadHost
         //so that it's DI capabilities are available prior to the scene reload.

         //i'm not sure why this can't be inside the "CallDeferred", but it doesn't work there.
         //I think it's because the init has to happen in the same stack call as the .ctor() call.

      }
   }
}


[Tool]
public partial class Main : Node3D
{
   public World World { get; set; }

   [Inject]
   public TestService testService;

   public object InitTest = null;
   public Main()
   {
      //GD.Print($"Main({this.GetInstanceId()}/{this.GetHashCode()}).ctor()  AutoLoad={AutoloadHost.Instance is not null}");
      this._PrintInfo($".ctor() main, IsInsideTree={IsInsideTree()},  AutoLoad={AutoloadHost.Instance is not null} TestService={testService is not null} InitTest={InitTest is not null}");

      this._ToolCtorHotReloadInit();
       
   }

   protected override void Dispose(bool disposing)
   {
      
      //GD.PrintRich($"[color=yellow]Main({this.GetInstanceId()}/{this.GetHashCode()}).Dispose({disposing})  AutoLoad={AutoloadHost.Instance is not null}");
      //GD.PrintRich("[color=yellow]")
      this._PrintInfo($".Dispose({disposing})  AutoLoad={AutoloadHost.Instance is not null} TestService={testService is not null} InitTest={InitTest is not null}");
      DI.globalHost.Dispose();
      base.Dispose(disposing);
      
   }

   public override void _EnterTree()
   {

      InitTest = new();
      this._PrintInfo($"._EnterTree() AutoLoad={AutoloadHost.Instance is not null} TestService={testService is not null} InitTest={InitTest is not null}");
      if (Engine.IsEditorHint())
      {
         Callable.From(() =>
         {
            GD.Print($"Main({this.GetInstanceId()}) => Callable.From  AutoLoad={AutoloadHost.Instance is not null}");
            //AutoloadHost.Instance.TryInit();
         });
         //
      }
      var loop = Engine.GetMainLoop();



      base._EnterTree();
   }

   //private int _lastNotificationWhat = -1;
   //public override void _Notification(int what)
   //{
   //   if (what != _lastNotificationWhat)
   //   {
   //      switch ((GodotNotifications)what)
   //      {
   //         case GodotNotifications.Node_WMMouseEnter:
   //         case GodotNotifications.Node_WMMouseExit:
   //         case GodotNotifications.Node_ApplicationFocusIn:
   //         case GodotNotifications.Node_ApplicationFocusOut:
   //         case GodotNotifications.Node_WMWindowFocusIn:
   //         case GodotNotifications.Node_WMWindowFocusOut:
   //            return;
   //      }
   //      _lastNotificationWhat = what;
   //      this._PrintTrace($" ._Notification({(test1.src.lib.GodotNotifications)what}:{what}) @ {DateTime.Now.ToString("O")} AutoLoad={AutoloadHost.Instance is not null}");
   //      //GD.Print($"Main({this.GetInstanceId()})._Notification({(test1.src.lib.GodotNotifications)what}:{what}) @ {DateTime.Now.ToString("O")} AutoLoad={AutoloadHost.Instance is not null}");
   //   }

   //   base._Notification(what);
   //}


   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {

      this._PrintInfo($"._Ready() AutoLoad={AutoloadHost.Instance is not null} TestService={testService is not null} InitTest={InitTest is not null}");
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
      GD.Print($"11Main({this.GetInstanceId()})._Ready() AutoLoad={AutoloadHost.Instance is not null}");
      ///GD.Print($"Engine Singletons = {Engine.GetSingletonList().Join()}");
      var mainLoop = Engine.GetMainLoop();
      //mainLoop.





      var player = new Player();
      AddChild(player);
      try
      {
         if (testService is not null)
         {
            testService.Test();
         }
         else
         {
            _GD.Log("testService is null", Colors.Red);
         }
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



