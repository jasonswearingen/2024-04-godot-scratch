using Godot;
using test1.src.lib;
using test1.src.lib.DI;

namespace test1;

[Tool]
public partial class DotNetScene : Node
{
   public DotNetScene()
   {
      this._PrintInfo(".ctor");
      this._TryHotReload();
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


   /// <summary>
   /// true if this node is properly "roundtrip" initialized in the Godot scene tree.
   /// this will be false in a CLR cold-reload scenario, where this dotnet object exists in the tree but it's CLR properties were not properly initialized.
   /// <para>generally this can be ignored as this "DotNetScene" tracks cold-reloads automatically.  but it can be helpful for deciding what to dispose of.</para>
   /// </summary>
   protected bool isClrInitialized = false;

   public override void _EnterTree()
   {
      this._PrintInfo($"._EnterTree()  isClrInitialized={isClrInitialized}");
      base._EnterTree();
      isClrInitialized = true;

      this.GetTree().NodeAdded += DotNetScene_NodeAdded;
      this.GetTree().NodeRemoved += DotNetScene_NodeRemoved;

      
   }

   private void DotNetScene_NodeRemoved(Node node)
   {
      if (isClrInitialized is false)
      {
         return;
      }
      
      if (node is IEzNode && this.IsAncestorOf(node))
      {
         this._PrintInfo($".DotNetScene_NodeRemoved({node.Name}:{node.GetInstanceId()}:{node.GetHashCode()}:{node.GetType().Name})");
      }
   }

   private void DotNetScene_NodeAdded(Node node)
   {
      if(isClrInitialized is false)
      {
         return;
      }
      
      if ((node is IEzNode || Engine.IsEditorHint() ) && this.IsAncestorOf(node)) //if running in editor, we will try to inject, just to throw error if not IEzNode
      {
         this._PrintInfo($".DotNetScene_NodeAdded({node.Name}:{node.GetInstanceId()}:{node.GetHashCode()}:{node.GetType().Name})");
         EzInjectAttribute.DiscoverAndInject(node, DiStatic.globalHost.serviceProvider);
      }
   }


   public override void _ExitTree()
   {
      this._PrintInfo($"._ExitTree()  isClrInitialized={isClrInitialized}");
      base._ExitTree();

      _ClrDispose();
   }

   private void _ClrDispose()
   {
      if (isClrInitialized)
      {
         //this object is properly initialized, and can be disposed of.
         //this is likely due to a normal scene tree exit.
         //this object will not be re-initialized.
         isClrInitialized = false;

         this.GetTree().NodeAdded -= DotNetScene_NodeAdded;
         this.GetTree().NodeRemoved -= DotNetScene_NodeRemoved;
      }
   }


   /// <summary>
   /// when handling CLR Cold-Reload disposings, make sure that .IsInitialized is true.
   /// </summary>
   /// <param name="disposing"></param>
   protected override void Dispose(bool disposing)
   {
      //_GD.Print($"!.Dispose({disposing})  isClrInitialized={isClrInitialized} {this.GetInstanceId()}:{this.GetHashCode()}", Colors.OrangeRed);
      this._PrintInfo($".Dispose({disposing})  isClrInitialized={isClrInitialized}");

      
      //
      

      if (disposing)
      {


         _ClrDispose();


      }

      base.Dispose(disposing);

   }

}
