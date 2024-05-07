using Godot;
using test1.src.lib;
using test1.src.lib.DI;

namespace test1;

[Tool]
public partial class DotNetScene : Node
{
   private static bool isReloadRequested = false;
   public DotNetScene()
   {
      this._PrintInfo(".ctor");
      _ClrReloadHelper();
   }

   /// <summary>
   /// helper function that detects when the CLR is cold-reloaded, such as when running in the editor and a rebuild occurs
   /// <para>when that happens, this function will reload the level and ensure the global DI Host is available.</para>
   /// </summary>
   private void _ClrReloadHelper()
   {
      var node = this;
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

public partial class DiHostNode : Node
{
   ////public required Node Owner { get; init; }
   //public DiHostNode(Node owner)
   //{
   //  // Owner = owner;

   //   this.o
   //}


   //public override void _EnterTree()
   //{
   //   base._EnterTree();

   //   diWrapper = new DiWrapper();
   //   diWrapper.Initialize(CancellationToken.None)._SyncWait();

   //}
   //protected override void Dispose(bool disposing)
   //{
   //   if(disposing)
   //   {
   //      diWrapper.Dispose();
   //      diWrapper = null;
   //   }

   //   base.Dispose(disposing);
   //}

   public void TryPopulate(Node node)
   {
      if(Owner.IsAncestorOf(node) is false)
      {
         return;
      }

      EzInjectAttribute.DiscoverAndInject(node, DiStatic.globalHost.serviceProvider);

   }


}