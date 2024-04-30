using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace test1.src.lib;
public static class zz_Extensions_Node
{

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


