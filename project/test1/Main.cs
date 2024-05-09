using Godot;
using System;
using System.Runtime.CompilerServices;
using GodotEx.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NotNot;
using System.Diagnostics;
using test1.src.lib.DI;


namespace test1;

[Tool]
public partial class Main : DotNetScene
{
   public World World { get; set; }

   

   public object InitTest = null;

   public Main()
   {
      //GD.Print($"Main({this.GetInstanceId()}/{this.GetHashCode()}).ctor()  AutoLoad={DI.globalHost is not null}");
      this._PrintInfo($".ctor() main, IsInsideTree={IsInsideTree()},  AutoLoad={DiStatic.globalHost is not null}  InitTest={InitTest is not null}");
      
   }

   protected override void Dispose(bool disposing)
   {
      //GD.PrintRich($"[color=yellow]Main({this.GetInstanceId()}/{this.GetHashCode()}).Dispose({disposing})  AutoLoad={DI.globalHost is not null}");
      //GD.PrintRich("[color=yellow]")
      this._PrintInfo($".Dispose({disposing})  AutoLoad={DiStatic.globalHost is not null}  InitTest={InitTest is not null}");
      //DI.globalHost.Dispose();
      base.Dispose(disposing);
   }

   public override void _EnterTree()
   {
      InitTest = new();
      this._PrintInfo($"._EnterTree() AutoLoad={DiStatic.globalHost is not null}  InitTest={InitTest is not null}");
      if (Engine.IsEditorHint())
      {
         Callable.From(() =>
         {
            GD.Print($"! Main({this.GetInstanceId()}) => Callable.From  AutoLoad={DiStatic.globalHost is not null}");
            //DI.globalHost.TryInit();
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
   //      this._PrintTrace($" ._Notification({(test1.src.lib.GodotNotifications)what}:{what}) @ {DateTime.Now.ToString("O")} AutoLoad={DI.globalHost is not null}");
   //      //GD.Print($"Main({this.GetInstanceId()})._Notification({(test1.src.lib.GodotNotifications)what}:{what}) @ {DateTime.Now.ToString("O")} AutoLoad={DI.globalHost is not null}");
   //   }

   //   base._Notification(what);
   //}


   // Called when the node enters the scene tree for the first time.
   public override void _Ready()
   {
      this._PrintInfo($"._Ready() AutoLoad={DiStatic.globalHost is not null}  InitTest={InitTest is not null}");
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
      GD.Print($"Main({this.GetInstanceId()})._Ready() AutoLoad={DiStatic.globalHost is not null}");
      ///GD.Print($"Engine Singletons = {Engine.GetSingletonList().Join()}");

      
      
      var player = Player.Instantiate();
      AddChild(player);
    
   }

   // Called every frame. 'delta' is the elapsed time since the previous frame.
   public override void _Process(double delta)
   {
      //  GD.Print("Executing !!  Main process");

      //testService.Test();
   }
}