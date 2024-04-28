using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace test1.src.lib.Internal;

public class ModuleInitializer
{
   [ModuleInitializer]
   public static void Initialize()
   {
      // This code runs when the assembly containing this class is loaded      
      _GD.Print("ModuleInitializer.Initialize(): Assembly loaded..", Colors.Aqua);
   }
}
