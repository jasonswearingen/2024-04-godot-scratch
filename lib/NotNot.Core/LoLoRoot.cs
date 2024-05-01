﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using NotNot._internal;
using NotNot._internal.Diagnostics;
using NotNot._internal.Threading;
using NotNot.Validation;
using Xunit;

namespace NotNot;

[DebuggerNonUserCode]
public partial class LoLoRoot
{
#pragma warning disable CS8618
   protected static LoLoRoot? _instance;
#pragma warning restore CS8618
   private ThreadLocal<Random> _random = new(() => new Random());

   /// <summary>
   ///    Call the .Run() command to schedule tasks to execute on a <see cref="DebuggableTaskFactory" />
   ///    This TaskFactory aids in debugging by running all scheduled tasks sequentially,  when running in #CHECKED or if
   ///    <see cref="DebuggableTaskFactory" />.singleThreaded is set to True
   /// </summary>
   public DebuggableAsyncHelper Async;
   //	[ThreadStatic]
   //	private static Random? _rand;
   //	//private static ThreadLocal<Random> _rand2 = new(() => new());

   //	/// <summary>
   //	/// get a thread-local Random
   //	/// </summary>
   //	public static Random Rand
   //	{
   //		get
   //		{
   //			if (_rand == null)
   //			{
   //				_rand = new Random();
   //			}
   //			return _rand;
   //		}
   //	}

   /// <summary>
   /// convenience method to emit Trace logs.  This is useful for development/test.
   /// </summary>
   [Conditional("DEBUG"), Conditional("TRACE") ]
   public void DevTrace(string? message = "", object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null, [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0, [CallerArgumentExpression("condition")] string conditionName = "",
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null")
   {

      var _devTraceLogger = __.GetLogger(sourceFilePath);

      _devTraceLogger._EzTrace(message, objToLog0, objToLog1, objToLog2, memberName, sourceFilePath, sourceLineNumber, objToLog0Name, objToLog1Name, objToLog2Name);      
      
   }
   public void Todo(string message, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = ""
      )
   {
      Assert(false, "Todo:" + message, memberName, sourceFilePath, sourceLineNumber);
   }

   public void Todo([CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "")
   {
      Assert(false, "Todo", memberName, sourceFilePath, sourceLineNumber);
   }

   public void Assert(bool condition, string? message = "", object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null, [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0, [CallerArgumentExpression("condition")] string conditionName = "",
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null")
   {
      if (condition is false)
      {
         var finalMessage = message._FormatAppendArgs(conditionName, argName0: "condition")._FormatAppendArgs(objToLog0,objToLog1,objToLog2,objToLog0Name,objToLog1Name,objToLog2Name)._FormatAppendArgs(memberName, sourceFilePath, sourceLineNumber);
         Debug.Assert(false, finalMessage);
         if (__.Test.IsTestingActive)
         {
            Xunit.Assert.Fail(finalMessage);
         }
      }
   }

   public void Assert(string? message = null, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null")
   {
      Assert(false, message,objToLog0,objToLog1,objToLog2, memberName, sourceFilePath, sourceLineNumber,objToLog0Name,objToLog1Name,objToLog2Name);
   }


   public void Assert(Exception ex, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      var finalMessage = ex.Message._FormatAppendArgs(memberName, sourceFilePath, sourceLineNumber);
      Debug.Assert(false,finalMessage , ex._ToUserFriendlyString());
      if (__.Test.IsTestingActive)
      {
         Xunit.Assert.Fail(finalMessage);
      }
   }

   private ConcurrentDictionary<string, int> _assertOnceKeys = new();

   private bool _tryAssertOnce(string? message, string sourceFilePath, int sourceLineNumber)
   {
      var key = $"{sourceFilePath}:{sourceLineNumber}:{message}";

      if (_assertOnceKeys.TryAdd(key, 1))
      {
         return true;
      }
      return false;

   }

   public void AssertOnce(bool condition, string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (condition is true)
      {
         return;
      }
      if (!_tryAssertOnce(message, sourceFilePath, sourceLineNumber))
      {
         return;
      }
      Assert(false, message, memberName, sourceFilePath, sourceLineNumber);

   }

   public void AssertOnce(string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (!_tryAssertOnce(message, sourceFilePath, sourceLineNumber))
      {
         return;
      }
      Assert(false, message, memberName, sourceFilePath, sourceLineNumber);
   }


   public void AssertOnce(Exception ex, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (!_tryAssertOnce(ex.GetType().Name, sourceFilePath, sourceLineNumber))
      {
         return;
      }
      Assert(ex, memberName, sourceFilePath, sourceLineNumber);
   }

   [Conditional("DEBUG")]
   public void DebugExec(Action action)
   {
      action();
   }
   [Conditional("DEBUG")]
   public async void DebugExec(Func<Task> func)
   {
      await func();
   }

   [Conditional("CHECKED")]
   public void CheckedExec(Action action)
   {
      action();
   }
   [Conditional("CHECKED")]
   public async void CheckedExec(Func<Task> func)
   {
      await func();
   }

   [Conditional("CHECKED")]
   public void CheckedAssert(bool condition, string? message = null, [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (condition is false)
      {
         Debug.Assert(false, message._FormatAppendArgs(memberName, sourceFilePath, sourceLineNumber));
      }
   }

   [Conditional("CHECKED")]
   public void CheckedAssert(string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      Assert(false, message, memberName, sourceFilePath, sourceLineNumber);
   }


   public void CheckedAssert(Exception ex, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      Debug.Assert(false, ex.Message._FormatAppendArgs(memberName, sourceFilePath, sourceLineNumber), ex._ToUserFriendlyString());
   }



   [Conditional("CHECKED")]
   public void CheckedAssertOnce(bool condition, string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (condition is true)
      {
         return;
      }
      if (!_tryAssertOnce(message, sourceFilePath, sourceLineNumber))
      {
         return;
      }
      Assert(false, message, memberName, sourceFilePath, sourceLineNumber);

   }

   [Conditional("CHECKED")]
   public void CheckedAssertOnce(string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (!_tryAssertOnce(message, sourceFilePath, sourceLineNumber))
      {
         return;
      }
      Assert(false, message, memberName, sourceFilePath, sourceLineNumber);
   }


   [Conditional("CHECKED")]
   public void CheckedAssertOnce(Exception ex, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (!_tryAssertOnce(ex.GetType().Name, sourceFilePath, sourceLineNumber))
      {
         return;
      }
      Assert(ex, memberName, sourceFilePath, sourceLineNumber);
   }

   /// <summary>
   /// throw an Exception if condition is false
   /// </summary>
   /// <param name="condition"></param>
   /// <exception cref="NotImplementedException"></exception>
   [DoesNotReturn]
   public Exception Throw(string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      Throw(false, message, memberName, sourceFilePath, sourceLineNumber);
      //never gets here because of throw
      return null;
   }
   /// <summary>
   /// throw an Exception if condition is false
   /// </summary>
   /// <param name="condition"></param>
   /// <exception cref="NotImplementedException"></exception>
   public void Throw([DoesNotReturnIf(false)] bool condition, string? message = null, [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0, [CallerArgumentExpression("condition")] string conditionName = "")
   {
      if (condition)
      {
         return;
      }
      Assert(false, message, memberName, sourceFilePath, sourceLineNumber, conditionName);
      var ex = new LoLoDiagnosticsException(message._FormatAppendArgs(conditionName, argName0: "condition"), memberName, sourceFilePath, sourceLineNumber);
      throw ex;
   }

   ///// <summary>
   /////    Logging/Assert functionality ONLY enabled in CHECKED builds
   ///// </summary>
   //[Obsolete("use __.GetLogger() instead", true)]
   //public CheckedDiag CHECKED;

   ///// <summary>
   /////    Logging/Assert functionality ONLY enabled in CHECKED or DEBUG builds
   ///// </summary>
   //[Obsolete("use __.GetLogger() instead", true)]
   //public DebugDiag DEBUG;

   ///// <summary>
   /////    used by GetLogger() to truncate sourceFilePath verbosity
   ///// </summary>
   //[Obsolete("use __.GetLogger() instead", true)]
   //internal string entrypointFilePath = null!;

   ///// <summary>
   /////    Logging/Assert functionality enabled in ALL builds
   ///// </summary>
   //[Obsolete("use __.GetLogger() instead", true)]
   //public ErrorDiag ERROR;





   public Placeholder placeholder = new();

   public StaticPool pool = new();


   private bool? _isProduction;
   /// <summary>
   /// returns true if we are running in production environment.  false otherwise
   /// </summary>
   public bool IsProduction
   {
      get
      {
         if (_isProduction is null)
         {
            _isProduction = (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")._AproxEqual("production"))
               || (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")._AproxEqual("production"));
         }
         return _isProduction.Value;
      }
   }


   private TestHelper _test = new();

   public TestHelper Test
   {
      get
      {
         if (__.IsProduction)
         {
            throw new LoLoDiagnosticsException("TestHelper should only be accessed when not in production.  why is this not true?");
         }
         return _test;
      }
   }

   //public Validate Validate = new();

   public Normalize Normalize = new();

   public LoLoRoot(LoLoConfig? config = null, IServiceProvider services = null)
   {

      Config = config ?? new LoLoConfig();
      _services = services;

      Async = new DebuggableAsyncHelper(Config.IsDebuggableTaskFactorySingleThreaded);
   }

#pragma warning disable IDE1006
   public static LoLoRoot __
#pragma warning restore IDE1006
   {
      get
      {
         if (_instance is null)
         {
            //throw new NullReferenceException("lolo.__ is not set, call lolo.__ = new lolo() first, in your program.cs");
            _instance = new LoLoRoot();
         }

         return _instance;
      }
      //set
      //{
      //	if (_instance != null)
      //	{
      //		//throw new Exception("lolo.__ is already set");
      //		//return;
      //	}

      //	_instance = value;
      //}
   }


   public Random Random => _random.Value!;

   public LoLoConfig Config { get; private set; }


   private IServiceProvider _services;
   public IServiceProvider? Services
   {
      get
      {
         if (_services is null)
         {
            throw new NullReferenceException("lolo.Services is not set.  You must call '__.Services=app.Services;' immediately after calling 'app = builder.Build();'");
         }
         return _services;
      }
      set
      {
         if (_services is not null)
         {
            throw new LoLoException("lolo.Services is already set");
         }

         _services = value;
      }
   }


  
   /// <summary>
   /// helper to dispose static variables, used by test runners.  probably not needed otherwise.
   /// </summary>
   public void Dispose()
   {
      _services = null;
   }

}

/// <summary>
///    simple console format used before serilog is loaded (or if no DI exists)
/// </summary>
public class FallbackConsoleFormatter : ConsoleFormatter
{
   public FallbackConsoleFormatter() : base("fallback")
   {
   }

   public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
   {
      lock (this)
      {
         var originalColor = Console.ForegroundColor;
         try
         {
            Console.ForegroundColor = GetColor(logEntry.LogLevel);

            var timestamp = DateTimeOffset.Now.ToString("HH:mm:ss.fff");
            var logLevel = logEntry.LogLevel.ToString().ToUpperInvariant();
            var message = logEntry.Formatter(logEntry.State, logEntry.Exception);

            textWriter.Write($"[{timestamp} {logLevel}] {message}  <NoDIServices_Fallback>");

            if (logEntry.Exception != null)
            {
               textWriter.Write($" {Environment.NewLine}{logEntry.Exception}");
            }

            textWriter.WriteLine();


            //textWriter.Flush();
         }
         finally
         {
            Console.ForegroundColor = originalColor;
         }
      }
   }

   private ConsoleColor GetColor(LogLevel logLevel)
   {
      return logLevel switch
      {
         LogLevel.Trace => ConsoleColor.Gray,
         LogLevel.Debug => ConsoleColor.Blue,
         LogLevel.Information => ConsoleColor.Green,
         LogLevel.Warning => ConsoleColor.Yellow,
         LogLevel.Error => ConsoleColor.Red,
         LogLevel.Critical => ConsoleColor.Magenta,
         _ => ConsoleColor.White,
      };
   }
}

public partial class LoLoRoot
{
   /// <summary>
   ///    logger used when DI Services are not set.  (see .GetLogger() logic below)
   /// </summary>
   internal ILogger? _DiMissingConsoleLoggerFallback;

   private bool _hasWarnedServicesMissing;

   private ConcurrentDictionary<string, object> _knownLoggerTypes = new();

   /// <summary>
   /// Obtain a logger for the current file.
   /// <para>This overload will cache the first call per-file and reuse the logger on all other calls from the same file.</para>
   /// </summary>
   /// <param name="callerFilePath_for_reflection_optimization_do_not_use"></param>
   /// <returns></returns>
   /// <exception cref="Exception"></exception>
   public ILogger GetLogger([CallerFilePath] string callerFilePath_for_reflection_optimization_do_not_use = "")
   {
      if (_services is null)
      {
         return _GetFallbackLogger();
      }

      var typeOrName = _knownLoggerTypes.GetOrAdd(callerFilePath_for_reflection_optimization_do_not_use, (key) =>
      {

         var trace = new StackTrace(3, false);
         var categoryType = trace.GetFrame(0)?.GetMethod()?.DeclaringType?.DeclaringType;
         if (categoryType is null)
         {
            var targetAssembly = Assembly.GetCallingAssembly();
            var loggerName = $"{targetAssembly.GetName().Name ?? "UNNAMED_ASSEMBLY"}_Logger";

            return loggerName;
            //var toReturn = Services!.GetRequiredService<ILoggerFactory>().CreateLogger(loggerName);           
            //var loggerType = typeof(ILogger<>).MakeGenericType(targetAssembly);
         }
         return categoryType;
      });
      switch (typeOrName)
      {
         case Type categoryType:
            return Services!.GetRequiredService<ILoggerFactory>().CreateLogger(categoryType);
         //return GetLogger(type);
         case string categoryName:
            return Services!.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);
         default:
            throw new Exception("unexpected");
      }
   }

   public ILogger GetLogger(object categoryFromType)
   {

      if (categoryFromType is Type categoryType)
      {//in case wrong overload called
         return GetLogger(categoryType);
      }

      var categoryTyepe = categoryFromType.GetType();
      return GetLogger(categoryTyepe);
   }

   public ILogger GetLogger(Type categoryType)
   {
      if (Services is null)
      {
         return _GetFallbackLogger();
      }

      //return (ILogger)Services!.GetRequiredService(loggerType);
      return Services!.GetRequiredService<ILoggerFactory>().CreateLogger(categoryType);


   }


   private ILogger _GetFallbackLogger()
   {
      if (_DiMissingConsoleLoggerFallback is null)
      {
         _DiMissingConsoleLoggerFallback = LoggerFactory.Create(logging =>
         {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);

            logging.AddConsole(options => { options.FormatterName = "fallback"; }).AddConsoleFormatter<FallbackConsoleFormatter, ConsoleFormatterOptions>();
         }).CreateLogger("NoDIServices_Fallback");
      }

      if (_hasWarnedServicesMissing is false)
      {
         _hasWarnedServicesMissing = true;
         _DiMissingConsoleLoggerFallback._EzWarn(
            "NoDIServices_Fallback IS BEING USED.  __.Services is null. It must be set to log properly (do so as soon as DI Services are available).  a simple console-only logger will be used until DI Services are available");
         //Serilog.Log.Logger.Warning("__.Services is null. It must be set to log properly (do so as soon as DI Services are available).  a simple console-only logger will be used until DI Services are available");
      }

      return _DiMissingConsoleLoggerFallback;
   }


   ///// <summary>
   /////    shortcut to create a new logger
   ///// </summary>
   //[Obsolete("use GetLogger() with either object or type instead", true)]
   //public ILogger GetLogger(string? name = null, [CallerFilePath] string sourceFilePath = "")
   //{
   //   throw new NotImplementedException("obsolete");
   //   //if (name._IsNullOrWhiteSpace())
   //   //{
   //   //	//set name to a friendly truncation of sourceFilePath
   //   //	name = sourceFilePath._GetUniqueSuffix(entrypointFilePath);
   //   //}

   //   //if (Services is null)
   //   //{
   //   //	if (_DiMissingConsoleLoggerFallback is null)
   //   //	{
   //   //		_DiMissingConsoleLoggerFallback = LoggerFactory.Create(logging =>
   //   //		{
   //   //			logging.ClearProviders();
   //   //			logging.SetMinimumLevel(LogLevel.Trace);

   //   //             logging.AddConsole(options => { options.FormatterName = "fallback"; }).AddConsoleFormatter<FallbackConsoleFormatter, ConsoleFormatterOptions>();
   //   //          }).CreateLogger("NoDIServices_Fallback");
   //   //	}

   //   //	if (_hasWarnedServicesMissing is false)
   //   //	{
   //   //		_hasWarnedServicesMissing = true;
   //   //		_DiMissingConsoleLoggerFallback._EzWarn(
   //   //			"NoDIServices_Fallback IS BEING USED.  __.Services is null. It must be set to log properly (do so as soon as DI Services are available).  a simple console-only logger will be used until DI Services are available");
   //   //		//Serilog.Log.Logger.Warning("__.Services is null. It must be set to log properly (do so as soon as DI Services are available).  a simple console-only logger will be used until DI Services are available");
   //   //	}

   //   //	return _DiMissingConsoleLoggerFallback;
   //   //}


   //   //return Services._GetService<ILoggerFactory>()!.CreateLogger(name);
   //}

   /// <summary>
   ///    shortcut to create a new logger
   /// </summary>
   public ILogger GetLogger<TCategory>()
   {
      if (Services is null)
      {
         return _GetFallbackLogger();
      }
      //return GetLogger(typeof(TCategory));
      return Services._GetService<ILoggerFactory>().CreateLogger<TCategory>();
   }


   //public AsyncLazy<ILogger> GetLoggerAsync(string? name = null, [CallerFilePath] string sourceFilePath = "")
   //{
   //	var toReturn = new AsyncLazy<ILogger>(async () =>
   //	{
   //		return GetLogger(name, sourceFilePath);
   //	});

   //	return toReturn;

   //	//if (this.IsInitialized)
   //	//{
   //	//	var logger = GetLogger(name);
   //	//	var toReturn = new AsyncLazy<ILogger>(logger);
   //	//	return toReturn;
   //	//}
   //	//else
   //	//{
   //	//	var toReturn = new AsyncLazy<ILogger>(async () =>
   //	//	{
   //	//		await _isInitializedTcs.Task;
   //	//		return GetLogger(name);
   //	//	});
   //	//	return toReturn;
   //	//}
   //}

   public AsyncLazy<ILogger> GetLoggerLazy<TCategory>()
   {
      return new AsyncLazy<ILogger>(async () =>
      {
         return GetLogger<TCategory>();
      });

      //if (this.IsInitialized)
      //{
      //	return new AsyncLazy<ILogger>(GetLogger<ILoggerFactory>());
      //}
      //else
      //{
      //	return new AsyncLazy<ILogger>(async () =>
      //	{
      //		await _isInitializedTcs.Task;
      //		return GetLogger<ILoggerFactory>();
      //	});
      //}
   }
}

public class _Tests
{
   private string AZ = "VZ";

   public _Tests()
   {
      //if (__ == null)
      //{
      //	__ = new LoLoRoot();
      //}
   }

   [Fact]
   public void Logger_Basic()
   {
      var AX = "VX";
      var AY = "VY";



      __.GetLogger(this)._EzInfo("warmup");
      __.GetLogger<_Tests>()._EzInfo("T", (AX: "VX", AY: "VY"));
      __.GetLogger()._EzInfo("test", AX, "AA", 123);
      __.GetLogger(this)._EzInfo("T", new { AX, AY });
      __.GetLogger(this)._EzInfo("T", (AX: "VX", AY: "VY"));
      __.GetLogger(this)._EzInfo("T", (AX, AY));
   }


   [Fact]
   public void Logger_RefEquals()
   {
      var logger = __.GetLogger();

      Assert.True(ReferenceEquals(logger, logger));
      Assert.True(ReferenceEquals(logger, __.GetLogger()));
      Assert.True(ReferenceEquals(logger, __.GetLogger(this)));
      Assert.True(ReferenceEquals(logger, __.GetLogger<_Tests>()));
   }
}