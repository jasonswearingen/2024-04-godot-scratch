using Godot;
using test1.src.lib;

namespace test1.lib;

/// <summary>
/// example of a custom main loop (add via Godot Editor Project Settings =>Advanced =>Run.  
/// does not run in editor, regardless of if [Tool] attribute is specified.
/// </summary>
[GlobalClass]
public partial class CustomMainLoop : SceneTree
{


   public override bool _Process(double delta)
   {
      this._PrintWarn("CustomMainLoop._Process() " + DateTime.UtcNow.ToLocalTime().ToString());
      return base._Process(delta);
   }
   public override void _Notification(int what)
   {
      base._Notification(what);

      if (what != lastWhat)
      {
         lastWhat = what;
         this._PrintWarn($"CustomMainLoop._Notification({(GodotNotifications)what}:{what}) @ {DateTime.UtcNow.ToLocalTime().ToString("O")}");
      }
   }

   private int lastWhat = -1;

   public override void _Initialize()
   {



      this._PrintWarn("CustomMainLoop._Initialize()");

      

      base._Initialize();
   }

   public override void _Finalize()
   {
      this._PrintWarn("CustomMainLoop._Finalize()");
      
      base._Finalize();
   }

   public override bool _PhysicsProcess(double delta)
   {
      this._PrintWarn("CustomMainLoop._PhysicsProcess() " + DateTime.UtcNow.ToLocalTime().ToString());
      return base._PhysicsProcess(delta);
   }

   protected override void Dispose(bool disposing)
   {
      this._PrintWarn($"CustomMainLoop.Dispose({disposing})");
      base.Dispose(disposing);
   }
}
