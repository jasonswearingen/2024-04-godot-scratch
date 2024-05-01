// [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] 
// [!!] Copyright ©️ NotNot Project and Contributors. 
// [!!] This file is licensed to you under the MPL-2.0.
// [!!] See the LICENSE.md file in the project root for more info. 
// [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!] [!!]  [!!] [!!] [!!] [!!]


using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable PH_S034

namespace LoLo.Diagnostics.Advanced;

/// <summary>
///    extra info about the debugger
/// </summary>
[SuppressMessage("ReSharper", "AsyncVoidLambda")]
public static class DebuggerInfo
{
   private static bool _isPaused;

   ///// <summary>
   ///// event that fires when the debugger is paused or unpaused
   ///// arg will be true for paused, false for unpaused
   ///// </summary>
   //public static ActionEvent<bool> OnPause = new();

   static DebuggerInfo()
   {
      //TODO: disable DebuggerInfo.IsPaused for release builds
     // __.placeholder.Later("disable DebuggerInfo.IsPaused for release builds");

      //new Task<int>(() => { }).

      //Task.Run()
      //new Task(() => { }).st
      //start a long-lived task that just loops, looking for when the application is paused (being stepped-into in the debugger)

#pragma warning disable EPC17 // Avoid async-void delegates
      new Task(async () =>
      {
         var heartbeatSw = Stopwatch.StartNew();
         while (true)
         {
            heartbeatSw.Restart();
            await Task.Delay(TimeSpan.FromSeconds(0.25));
            var elapsed = heartbeatSw.Elapsed;
            if (IsPaused is true)
            {
               //already flagged as debugging, so need to run "fast" to unflag
               if (elapsed < TimeSpan.FromSeconds(0.3))
               {
                  IsPaused = false;
               }
            }
            else
            {
               //we are not flagged as active debugging, so only if the pause exceeds 1 second we say debugging.
               if (elapsed > TimeSpan.FromSeconds(1))
               {
                  IsPaused = true;
                  WasPaused = true;
               }
            }
         }
      }, TaskCreationOptions.LongRunning).Start();
#pragma warning restore EPC17 // Avoid async-void delegates
   }

   /// <summary>
   ///    helper to detect when a debugger is attached and actively stepping through code.
   ///    Useful for preventing subsystem timeouts due to a paused debugger.
   /// </summary>
   /// <remarks>
   ///    I hacked together a "solution" that fires up a long-lived task that resets a stopwatch every 250ms.   so if the time
   ///    between resets exceeds 1 second I consider the app "paused".   Not very elegant so I'd love to hear suggestions.
   ///    I think I need this Debugger.IsPaused thingy so I can Assert if a task may be deadlocked(runs too long )...
   ///    basically some debugging helper for a multithreading game engine I'm writing
   ///    So if I screw up and deadlock somewhere I don't have to stare at a blank screen wondering what I did wrong.
   ///    Not having a Debugger.IsPaused solution means that whenever I step through in a debugger, my "deadlock assert" code
   ///    would trigger because everything runs too long
   /// </remarks>
   public static bool IsPaused
   {
      get => _isPaused;
      private set
      {
         if (_isPaused != value)
         {
            _isPaused = value;
            //OnPause.Invoke(value);
         }
      }
   }

   /// <summary>
   ///    if a debugger ever stepped through, this will be true
   /// </summary>
   public static bool WasPaused { get; private set; }
}