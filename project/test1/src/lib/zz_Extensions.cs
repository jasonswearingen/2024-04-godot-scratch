using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace test1.src.lib;
public static class zz_Extensions_Node
{
   public static void _Print(this Node node, string message, Color? color = null)
   {
      if(color is null)
      {
         color = Colors.Gray;
      }
      GD.PrintRich($"[color=#{color.Value.ToRgba32():X8}]{message}[/color]");
   }

   public static void _PrintTrace(this Node node, string message)
   {
      _PrintNodeHelper(_GetNodeIdDetails(node), message, Colors.DarkSlateGray);
   }
   public static void _PrintWarn(this Node node, string message)
   {
      _PrintNodeHelper(_GetNodeIdDetails(node), message, Colors.Yellow);
   }

   public static void _PrintInfo(this Node node, string message)
   {
      _PrintNodeHelper(_GetNodeIdDetails(node), message, Colors.Aquamarine);
   }

   private static string _GetNodeIdDetails(this Node node)
   {
      var name = node.Name;
      if(name != node.GetType().Name)
      {
         name = $"{node.GetType().Name}({name})";
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


