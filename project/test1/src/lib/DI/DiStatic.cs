using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;
using GodotEx.Hosting;
using NotNot;

namespace test1.src.lib.DI;

public static class DiStatic
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // This code runs when the assembly containing this class is loaded      
        //_GD.Print("ModuleInitializer.Initialize(): Assembly loaded..", Colors.DarkOrange);
        _GD.Log("[[[ === DI_ModuleInitializer COLD RELOAD OF MANAGED PROJECT === ]]]", Colors.DarkOrange);

        //_GD.Log($"Singletons: {Engine.GetSingletonList().Join()}");


        _DoInit();

        // register cleanup code to prevent unloading issues
        System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(System.Reflection.Assembly.GetExecutingAssembly()).Unloading += alc =>
        {
            _DoFree();
        };

    }

    private static void _DoFree()
    {

        _GD.Print("DI._DoFree()", Colors.Aqua);

        globalHost.Dispose();
        globalHost = null;

        //var mainLoop = Engine.GetMainLoop();
        //if (mainLoop is SceneTree sceneTree)
        //{
        //   //remove first, because if this is running in the editor (tool mode) the editor reloads the CLR
        //   //which would cause these events to be double-subscribed.


        //}

    }
    private static void _DoInit()
    {
        _GD.Print("DI._DoInit()", Colors.Aqua);
        var mainLoop = Engine.GetMainLoop();
        if (mainLoop is SceneTree sceneTree)
        {
            _GD.Log($"scenetree: , {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}");


            //sceneTree.NodeAdded += SceneTree_NodeAdded;
            //sceneTree.NodeRemoved += SceneTree_NodeRemoved;
            //sceneTree.NodeConfigurationWarningChanged += SceneTree_NodeConfigurationWarningChanged;         
        }

        globalHost = new();
        globalHost.Initialize(CancellationToken.None)._SyncWait();
    }


    public static DiWrapper globalHost;



}

//public partial class EzInjectStore : Node
//{
//   public Dictionary<string, object> diStore;

//   public override void _EnterTree()
//   {
//      base._EnterTree();
//      diStore = new();
//   }
//   public override void _ExitTree()
//   {
//      base._ExitTree();
//      diStore.Clear();
//   }

//   public static TNode FindTargetFromStore<TNode>(Node currentNode) where TNode : Node
//   {
//      var startingNode = currentNode;

//      while (currentNode is not null)
//      {
//        var ezInjectStore =  currentNode._FindChild<EzInjectStore>();
//         if (ezInjectStore is not null)
//         {
//            ezInjectStore.diStore
//         }

//         if (currentNode is TNode target)
//         {
//            return target;
//         }
//         currentNode = currentNode.GetParent();
//      }


//   }


//}