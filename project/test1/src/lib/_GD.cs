using System.Reflection;
using Godot;

namespace test1.lib;

public static class _GD {

   public static void Log(string message, Color? color=null)
   {
      if(color is null)
      {
         color = Colors.Gray;
      }
      GD.PrintRich($"[color=#{color.Value.ToRgba32():x8}]{message}[/color]");
   }
   public static void Print(string message, Color? color = null)
   {
      if (color is null)
      {
         color = Colors.Gray;
      }
      GD.PrintRich($"[color=#{color.Value.ToRgba32():x8}]{message}[/color]");
   }

   public static Dictionary<string, Color> GetNamedColors()
   {
      // Accessing the internal static member 'namedColors' from 'Godot.Colors' class
      Type colorsType = typeof(Colors);
      FieldInfo fieldInfo = colorsType.GetField("namedColors", BindingFlags.NonPublic | BindingFlags.Static);

      if (fieldInfo != null)
      {
         return fieldInfo.GetValue(null) as Dictionary<string, Color>;
      }
      return null;
   }

}

public static class _Engine
{
   public static void ReloadScene(bool isExecutedImmediately = false)
   {
      void _DoSceneReload()
      {
         _GD.Log("RELOADING SCENE START",Colors.Yellow);
         var editor = EditorInterface.Singleton;
         editor.ReloadSceneFromPath(EditorInterface.Singleton.GetEditedSceneRoot().SceneFilePath);
         _GD.Log("RELOADING SCENE DONE", Colors.Yellow);
      }

      if (Engine.IsEditorHint())
      {
         _GD.Log("RELOADING SCENE TRIGGER!", Colors.Yellow);
         //need to use a callback because we are currently in a critical path,
         //and attempting to reload the editor will crash if we do it now.
         if (isExecutedImmediately)
         {
            _DoSceneReload();
         }
         else
         {
            Callable.From(_DoSceneReload).CallDeferred();
         }
      }
   }
}

