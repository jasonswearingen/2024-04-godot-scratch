using System.Runtime.CompilerServices;
using Godot;

namespace test1.lib.Internal;

public class ModuleInitializer
{
   [ModuleInitializer]
   public static void Initialize()
   {
      // This code runs when the assembly containing this class is loaded      
      //_GD.Print("ModuleInitializer.Initialize(): Assembly loaded..", Colors.DarkOrange);
      _GD.Log("[[[ === COLD RELOAD OF MANAGED PROJECT === ]]]", Colors.DarkOrange);

      if (Engine.IsEditorHint())
      {
         
      }
   }
}
