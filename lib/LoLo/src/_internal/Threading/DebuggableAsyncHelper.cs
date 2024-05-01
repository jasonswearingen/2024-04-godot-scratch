﻿using LoLo._internal.Threading;
using LoLo.src._internal.Threading;

namespace LoLo;

public class DebuggableAsyncHelper
{
	private DebuggableTaskFactory _DebuggableTaskFactory;

	public DebuggableAsyncHelper(bool isSingleThreadedFactory)
	{
		_DebuggableTaskFactory = new DebuggableTaskFactory(isSingleThreadedFactory);
	}

	/// <summary>
	///    run tasks on this, such as Factory.Run()
	/// </summary>
	public TaskFactory Factory => _DebuggableTaskFactory.Factory;

   public CancellationToken CancelAfter(CancellationToken linked, TimeSpan delay)
   {
		return DebuggableTimeoutCancelTokenHelper.Timeout(linked, delay);
	}

   public CancellationToken CancelAfter(TimeSpan delay)
   {
		return DebuggableTimeoutCancelTokenHelper.Timeout(delay);
	}

	public void CancelAfter(CancellationTokenSource cts, TimeSpan delay)
	{
		DebuggableTimeoutCancelTokenHelper.CancelAfter(cts, delay);
	}


   public Task Delay(TimeSpan duration)
	{
      var ct = CancelAfter(duration);
      var tcs = new TaskCompletionSource();
      var ctr = tcs._SetFromCancellationToken(ct);
      _ = tcs.Task.ContinueWith(async task => { ctr.Dispose(); });
      return tcs.Task;
	}

	public Task Delay(int msDuration, CancellationToken cancellationToken)
	{
		return Delay(TimeSpan.FromMilliseconds(msDuration), cancellationToken);
	}

	public Task Delay(TimeSpan duration, CancellationToken cancellationToken)
	{
      var ct = CancelAfter(cancellationToken, duration);
      var tcs = new TaskCompletionSource();
      var ctr = tcs._SetFromCancellationToken(ct);
      _ = tcs.Task.ContinueWith(async task => { ctr.Dispose(); });
      return tcs.Task;
   }

   /// <summary>
   ///    run a long running task  (Thread) here to avoid clogging the task pool.
   ///    run via this helper so that if you choose to run the application single-threaded, this will also.
   /// </summary>
   public Task LongRun(bool synchronizeWithCurrent, Func<Task> action, CancellationToken ct = default)
   {
      if (synchronizeWithCurrent is true || _DebuggableTaskFactory.SingleThreaded)
      {
         //implmentation from: https://stackoverflow.com/a/16916466/1115220
         //also see https://learn.microsoft.com/en-us/archive/msdn-magazine/2011/february/msdn-magazine-parallel-computing-it-s-all-about-the-synchronizationcontext

         //return Task.Factory.StartNew(action, 
         //   ct, 
         //   TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();

         return Run(action, ct,
            TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());
      }

      //if (_DebuggableTaskFactory.SingleThreaded)
      //{
      //   //long running ususally runs on it's own thread.   
      //   //if we want to force a single thread for our app, we need to undo this.
      //   return Factory.Run(debugWrapper, ct);
      //}

      return Run(action, ct, TaskCreationOptions.LongRunning);
   }

   /// <summary>
   ///    run a long running task  (Thread) here to avoid clogging the task pool.
   ///    run via this helper so that if you choose to run the application single-threaded, this will also.
   /// </summary>
   public Task LongRun(Func<Task> action, CancellationToken ct = default)
   {
      return LongRun(false, action, ct);
   }

   /// <summary>
   ///    run a task here, to aid in debugging. (if you set __.Config.IsSingleThreaded, this will run on a single thread)
   /// </summary>
   /// <param name="action"></param>
   /// <param name="ct"></param>
   /// <param name="options"></param>
   /// <param name="scheduler"></param>
   /// <returns></returns>
   public Task Run(Func<Task> action, CancellationToken ct = default, TaskCreationOptions? options = default, TaskScheduler? scheduler = default)
   {
      scheduler ??= Factory.Scheduler;
      var debugWrapper = (Func<Task>)(async () =>
      {
         try
         {
            await action();
         }
         catch (Exception ex)
         {
            switch (ex)
            {
               case TaskCanceledException:
               case OperationCanceledException:
                  //ignore
                  throw;
               case AggregateException ae:
                  if (ae.InnerException is TaskCanceledException)
                  {
                     //ignore
                     throw;
                  }

                  if (ae.InnerException is OperationCanceledException)
                  {
                     //ignore
                     throw;
                  }

                  break;
            }

            __.GetLogger()._EzError("Unhandled exception in LongRun.", ex);
            throw;
         }
      });

      return Factory.Run(debugWrapper, ct, options, scheduler);
   }
}