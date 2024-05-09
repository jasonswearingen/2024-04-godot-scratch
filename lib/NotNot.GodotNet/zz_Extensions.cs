using System.Runtime.CompilerServices;
using Godot;

public static class zz_Extensions_Node
{
   private static bool isReloadRequested = false;

   /// <summary>
   /// CALL FROM YOUR .NET SCENE ROOT NODE .CTOR()
   /// <para>helper function that detects when the CLR is cold-reloaded, such as when running in the editor and a rebuild occurs</para>
   /// <para>when that happens, this function will reload the level and ensure the global DI Host is available.</para>
   /// </summary>
   public static void _TryHotReload(this Node node)
   {

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

   // Extension method to find a child node of a specific type
   public static T _FindChild<T>(this Node node, bool recursive = false) where T : Node
   {

      // Check each child of the current node
      foreach (Node child in node.GetChildren())
      {
         if (child is T tChild)
         {
            return tChild;
         }

         // If recursive is true, look further down the tree
         if (recursive)
         {
            T recursiveChild = child._FindChild<T>(true);
            if (recursiveChild != null)
            {
               return recursiveChild;
            }
         }
      }

      return null; // Return null if no matching child is found
   }
   //public static string _GetFolderPath(this Node node, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName="")
   //{
      
   //}
}
public static class zz_Extensions_GodotObject
{
   public static void _Log<T>(this T node, string message, Color? color = null)
   {
      if (color is null)
      {
         color = Colors.Gray;
      }
      GD.PrintRich($"[color=#{color.Value.ToRgba32():X8}]{message}[/color]");
   }

   public static void _PrintTrace<T>(this T node, string message)
      where T:GodotObject
   {      
      _PrintNodeHelper(_GetNodeIdDetails(node), message, Colors.DarkSlateGray);
   }
   public static void _PrintWarn<T>(this T node, string message)
      where T : GodotObject
   {
      _PrintNodeHelper(_GetNodeIdDetails(node), message, Colors.Yellow);
   }

   public static void _PrintInfo<T>(this T node, string message)
      where T : GodotObject
   {
      _PrintNodeHelper(_GetNodeIdDetails(node), message, Colors.Aquamarine);
   }

   private static string _GetNodeIdDetails<T>(this T node)
      where T : GodotObject
   {
      string name;
      if (node is Node _node)
      {
         name = _node.Name;
         if (name != _node.GetType().Name)
         {
            name = $"{_node.GetType().Name}({name})";
         }
      }
      else
      {
         name = node.GetType().Name;
      }
      return $"{name}:...{node.GetInstanceId() % 10000}:...{node.GetHashCode() % 10000}";
   }

   private static void _PrintNodeHelper(string neutralPrefix, string coloredMessage, Color color)
   {
      GD.PrintRich($"[{neutralPrefix}] [color=#{color.ToRgba32():X8}]{coloredMessage}[/color]");
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


