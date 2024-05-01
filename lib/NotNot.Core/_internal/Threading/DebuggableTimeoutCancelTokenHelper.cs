using NotNot.Diagnostics.Advanced;
using System.Diagnostics;

namespace NotNot._internal.Threading;

/// <summary>
///    debugger helper logic for timeouts via a CancellationToken, but will pause the timeout from counting-down while the
///    application is paused in a debugger.
/// </summary>
public static class DebuggableTimeoutCancelTokenHelper
{
   public static void CancelAfter(CancellationTokenSource cts, TimeSpan timeout)
   {
      if (__.Config.IsCtsDebuggableCancelTimeoutEnabled is not true)
      {
         cts.CancelAfter(timeout);

         return;
      }

      //if we are debugging, we don't want to cancel the cts after a timeout, because we may be stepping through code.
      if (DebuggerInfo.IsPaused || timeout > _cancelThreshhold)
      {
         lock (_storage)
         {
            _storage.Add(new _CancelPair { delayRemaining = timeout, cts = cts });
         }
      }
      else
      {
         cts.CancelAfter(timeout);
      }
   }

   public static CancellationToken Timeout(TimeSpan delay)
   {
      var cts = new CancellationTokenSource();
      CancelAfter(cts, delay);
      return cts.Token;
   }

   public static CancellationToken Timeout(CancellationToken linked, TimeSpan delay)
   {
      var cts = CancellationTokenSource.CreateLinkedTokenSource(linked);
      CancelAfter(cts, delay);
      return cts.Token;
   }


   #region private helpers

   private class _CancelPair
   {
      public CancellationTokenSource cts;
      public TimeSpan delayRemaining;
   }

   private static List<_CancelPair> _storage = new();
   private static TimeSpan _pollRate = TimeSpan.FromMilliseconds(100);
   private static TimeSpan _cancelThreshhold = TimeSpan.FromMilliseconds(150);

   /// <summary>
   ///    a background thread that will cancel any CTS that has timed out.
   /// </summary>
   /// <returns></returns>
   private static async Task _workerThread()
   {
      var sw = Stopwatch.StartNew();
      while (true)
      {
         await Task.Delay(_pollRate);
         if (DebuggerInfo.IsPaused)
         {
            sw.Restart();
            continue;
         }

         lock (_storage)
         {
            var elapsed = sw.Elapsed;
            //if we are paused, we don't want to subtract a large amount of time from the delayRemaining, because we may be stepping through code.
            elapsed = elapsed > _pollRate * 2 ? _pollRate : elapsed;
            sw.Restart();
            for (var i = _storage.Count - 1; i >= 0; i--)
            {
               var pair = _storage[i];
               //subtract elapsed from each pair
               pair.delayRemaining -= elapsed;
               if (pair.delayRemaining <= _cancelThreshhold)
               {
                  if (pair.delayRemaining <= TimeSpan.Zero)
                  {
                     //cancel the cts

#pragma warning disable PH_S019 // Blocking Method in Async Method
                     pair.cts.Cancel();
#pragma warning restore PH_S019 // Blocking Method in Async Method
                     pair.cts.Dispose();
                  }
                  else
                  {
                     pair.cts.CancelAfter(pair.delayRemaining);
                  }

                  //remove the pair
                  _storage.RemoveAt(i);
               }
            }
         }
      }
   }

   private static Task? _workerThreadTask;

   static DebuggableTimeoutCancelTokenHelper()
   {
      //library helper so we don't want to be in single-threaded mode due to LoLo, so we create our longrunning manually.
      _workerThreadTask = Task.Factory.Run(_workerThread, default,
         TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
   }

   #endregion private helpers
}