using System.Runtime.CompilerServices;
using Godot;
using GodotEx;
using NotNot;

namespace test1.lib.Internal;

//public class ModuleInitializer
//{
//   [ModuleInitializer]
//   public static void Initialize()
//   {
//      // This code runs when the assembly containing this class is loaded      
//      //_GD.Print("ModuleInitializer.Initialize(): Assembly loaded..", Colors.DarkOrange);
//      _GD.Log("[[[ === COLD RELOAD OF MANAGED PROJECT === ]]]", Colors.DarkOrange);

//      if (Engine.IsEditorHint())
//      {
//         var mainLoop = Engine.GetMainLoop();
//         if(mainLoop is SceneTree sceneTree)
//         {
//            _GD.Log($"scene tree, {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}");


//            sceneTree.NodeAdded += SceneTree_NodeAdded;
//            sceneTree.NodeRemoved += SceneTree_NodeRemoved;
//         }
//      }
//   }

//   private static void SceneTree_NodeRemoved(Node node)
//   {
//      var sceneTree = Engine.GetMainLoop() as SceneTree;
//      //_GD.Log($"SceneTree_NodeRemoved, {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}, node={node.Name}:{node.GetInstanceId()} x {node.GetHashCode()}");
//   }

//   private static void SceneTree_NodeAdded(Node node)
//   {
//      var sceneTree = Engine.GetMainLoop() as SceneTree;
//      //_GD.Log($"SceneTree_NodeAdded, {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}, node={node.Name}:{node.GetInstanceId()} x {node.GetHashCode()}");
//   }
//}
