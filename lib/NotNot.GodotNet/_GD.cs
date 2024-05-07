using System.Reflection;
using Godot;

public static class _GD
{
   public static void Log(string message, Color? color = null)
   {
      if (color is null)
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