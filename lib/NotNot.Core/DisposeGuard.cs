namespace NotNot;

/// <summary>
///    helper to ensure object gets disposed properly.   can either be used as a base class, or as a member.
/// </summary>
public class DisposeGuard : IDisposable
{
   private bool _IsDisposed;

   public DisposeGuard()
   {
#if DEBUG

      var traceStr = Environment.StackTrace;
      //CtorStackTrace = EnhancedStackTrace.Current();
      CtorStackTrace = FixStackTraceForDisposeGuardLogging(traceStr);
#endif
   }

   public bool IsDisposed { get => _IsDisposed; init => _IsDisposed = value; }


   private List<string> CtorStackTrace { get; set; } //= "Callstack is only set in #DEBUG";

   public void Dispose()
   {
      if (IsDisposed)
      {
         __.GetLogger()._EzError(false, "why is dispose called twice?");
         return;
      }

      OnDispose(true);

#if DEBUG
      __.GetLogger()._EzError(IsDisposed,
         "Your override didn't call base.OnDispose() like you are supposed to", null, GetType().FullName);
#endif
      GC.SuppressFinalize(this);
   }

   /// <summary>
   ///    Override to implement the dispose pattern.  Be sure to call base.OnDispose() if you do.
   /// </summary>
   /// <param name="managedDisposing">
   ///    if true, called via normal .Dispose() workflow.  if false, called from destructor, so
   ///    only delete unmanaged resources (do not touch managed member references in this case)
   /// </param>
   protected virtual void OnDispose(bool managedDisposing)
   {
      _IsDisposed = true;
   }

   ~DisposeGuard()
   {
      if (!IsDisposed)
      {
         var msg = $"Did not call {GetType().Name}.Dispose() (or Dispose of it's parent) type properly.  Stack=\n\t\t";

         //Debug.WriteLine(msg);

         __.GetLogger()._EzError(false,
            msg + (CtorStackTrace is null ? "Callstack is only set in #DEBUG" : string.Join("\n\t\t", CtorStackTrace)));
         OnDispose(false);
      }
   }

   /// <summary>
   ///    custom fixup of the stack trace just for dispose-guard use
   /// </summary>
   /// <param name="trace"></param>
   /// <returns></returns>
   private static List<string> FixStackTraceForDisposeGuardLogging(string trace)
   {
      var hasLineInfo = trace.Contains(":line ");

      var lines = trace.Split(new[] { Environment.NewLine },
         StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
      //remove first entry as it's the disposeguard location
      //lines.RemoveAt(0);

      for (var i = lines.Count - 1; i >= 0; i--)
      {
         var line = lines[i];
         //line = line.TrimEnd();
         //lines[i] = line;

         var thisLineHasSourceInfo = false;

         //if source info, need to put in proper format for output window to detect.
         //see https://learn.microsoft.com/en-us/cpp/build/formatting-the-output-of-a-custom-build-step-or-build-event?view=msvc-170
         if (line.Contains(":line"))
         {
            thisLineHasSourceInfo = true;

            line = line.Replace(":line ", "(");
            line += ") : ";


            //continue;
         }

         //reverse so line details come in front
         var split = line.Split(" in ").Reverse().ToList();
         line = string.Join(" in ", split).TrimEnd();
         lines[i] = line;


         if (line.EndsWith("]") || thisLineHasSourceInfo)
         {
            continue;
         }


         lines.RemoveAt(i);
      }

      return lines;
   }
}

/// <summary>
///    helper to ensure object gets disposed properly.   can either be used as a base class, or as a member.
/// </summary>
public class AsyncDisposeGuard : IAsyncDisposable
{
   private bool _IsDisposed;

   public AsyncDisposeGuard()
   {
#if DEBUG

      var traceStr = Environment.StackTrace;
      //CtorStackTrace = EnhancedStackTrace.Current();
      CtorStackTrace = FixStackTraceForDisposeGuardLogging(traceStr);
#endif
   }

   protected bool IsDisposed { get => _IsDisposed; init => _IsDisposed = value; }


   private List<string> CtorStackTrace { get; set; } //= "Callstack is only set in #DEBUG";

   public async ValueTask DisposeAsync()
   {
      if (IsDisposed)
      {
         __.GetLogger()._EzError(false, "why is dispose called twice?");
         return;
      }

      await OnDispose(true);

#if DEBUG
      __.GetLogger()._EzError(IsDisposed,
         "Your override didn't call base.OnDispose() like you are supposed to", null, GetType().FullName);
#endif
      GC.SuppressFinalize(this);
   }


   /// <summary>
   ///    Override to implement the dispose pattern.  Be sure to call base.OnDispose() if you do.
   /// </summary>
   /// <param name="managedDisposing">
   ///    if true, called via normal .Dispose() workflow.  if false, called from destructor, so
   ///    only delete unmanaged resources (do not touch managed member references in this case)
   /// </param>
   protected virtual ValueTask OnDispose(bool managedDisposing)
   {
      _IsDisposed = true;
      return ValueTask.CompletedTask;
   }

   ~AsyncDisposeGuard()
   {
      if (!IsDisposed)
      {
         var msg = $"Did not call {GetType().Name}.Dispose() (or Dispose of it's parent) properly.  Stack=\n\t\t";

         //Debug.WriteLine(msg);

         __.GetLogger()._EzError(false,
            msg + (CtorStackTrace is null ? "Callstack is only set in #DEBUG" : string.Join("\n\t\t", CtorStackTrace)));

         OnDispose(false)._SyncWait();
      }
   }

   /// <summary>
   ///    custom fixup of the stack trace just for dispose-guard use
   /// </summary>
   /// <param name="trace"></param>
   /// <returns></returns>
   private static List<string> FixStackTraceForDisposeGuardLogging(string trace)
   {
      var hasLineInfo = trace.Contains(":line ");

      var lines = trace.Split(new[] { Environment.NewLine },
         StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
      //remove first entry as it's the disposeguard location
      //lines.RemoveAt(0);

      for (var i = lines.Count - 1; i >= 0; i--)
      {
         var line = lines[i];
         //line = line.TrimEnd();
         //lines[i] = line;

         var thisLineHasSourceInfo = false;

         //if source info, need to put in proper format for output window to detect.
         //see https://learn.microsoft.com/en-us/cpp/build/formatting-the-output-of-a-custom-build-step-or-build-event?view=msvc-170
         if (line.Contains(":line"))
         {
            thisLineHasSourceInfo = true;

            line = line.Replace(":line ", "(");
            line += ") : ";


            //continue;
         }

         //reverse so line details come in front
         var split = line.Split(" in ").Reverse().ToList();
         line = string.Join(" in ", split).TrimEnd();
         lines[i] = line;


         if (line.EndsWith("]") || thisLineHasSourceInfo)
         {
            continue;
         }


         lines.RemoveAt(i);
      }

      return lines;
   }
}