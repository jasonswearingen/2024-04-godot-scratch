﻿using LoLo.Serialization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;


// ReSharper disable once CheckNamespace
namespace LoLo;

/// <summary>
///    wrapper over Microsoft.Extensions.Logging.ILogger to include callsite and reduce boilerplate.
/// </summary>
[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]

[DebuggerNonUserCode]
public static class zz_Extensions_ILogger
{
	public static int MaxFileInfoFolderDepth = 3;

   /// <summary>
   /// internal thread safety
   /// </summary>
	private static object _lock = new();
   /// <summary>
   /// internal cache to reduce allocations
   /// </summary>
	private static List<(string name, object? value)> _argPairsCache = new();

   [DebuggerNonUserCode]
   private static void _Ez(this ILogger logger, LogLevel level, string? message = "", object? arg0 = null,
		object? arg1 = null, object? arg2 = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0,
		[CallerArgumentExpression("arg0")] string argName0 = "null",
		[CallerArgumentExpression("arg1")] string argName1 = "null",
		[CallerArgumentExpression("arg2")] string argName2 = "null",
		Exception? ex = null,
		[CallerArgumentExpression("ex")] string? exName = null
	)
	{
      //store all (objToLog,objToLogName) pairs in a list, discarding any pairs with an objToLogName of "null"
      //create a finalLogMessage combining the message with the names from each pair, showing the values from each pair      
      //pass the finalLogMessage and all the values to the Microsoft.Extensions.Logging.ILogger.Log method
      //that ILogger.Log has the following signature: public static void Log(this ILogger logger, LogLevel logLevel, Exception? exception, string? message, params object?[] args)

      var originalMessage = message;
		lock (_lock)
		{
			try
			{
				var argPairs = _argPairsCache;
#if DEBUG
				if (argPairs.Count > 0)
				{
					throw new Exception("argPairs.Count > 0");
				}
#endif
				if (arg0 is not null || argName0 is not "null")
				{
					argPairs.Add((argName0, arg0));
				}

				if (arg1 is not null || argName1 is not "null")
				{
					argPairs.Add((argName1, arg1));
				}

				if (arg2 is not null || argName2 is not "null")
				{
					argPairs.Add((argName2, arg2));
				}

				//roundtrip argValues to json to avoid logger (serilog) max depth errors
				{
					for (var i = 0; i < argPairs.Count; i++)
					{
						try
						{
							var obj = argPairs[i].value;

							if (obj is null)
							{
								continue;
							}

							argPairs[i] = (argPairs[i].name, SerializationHelper.ToPoCo(obj));
						}
						catch (Exception err)
						{
							logger.LogError($"could not roundtrip {argPairs[i].name} due to error {argPairs[i].value}.", err);
							throw;
						}
					}
				}

				//add callsite to output message/args
				{
					var method = $"{memberName}";
					argPairs.Add(("method", method));

					var callsite = $"{sourceFilePath}({sourceLineNumber}) : ";
					argPairs.Add(("callsite", callsite));
				}

				//adjust our message to include all arg Name+Values
				for (var i = 0; i < argPairs.Count; i++)
				{
					var (argName, argValue) = argPairs[i];

					argName = argName.Trim('"');

					//serilog can't log braces so replace them with aproximates 
					var sanitizedArgName =
						argName._ConvertToAlphanumeric(); //?._Replace(" {}[]", '_'); //?.Replace('{', '[').Replace('}', ']');

					if (
						argName.Equals(argValue) //string variable as argument
						|| argName.Equals(argValue.ToString()) //primitive variable as argument
						|| argName.Contains(',')) //tuple as argument
					{
						//argName is the same as the value that will be logged, so just set it to "_arg{i}" to avoid redundancy
						sanitizedArgName = $"_arg{i}";
					}

					sanitizedArgName ??= $"_UNKNOWN{i}";

					message += $"\n\t{sanitizedArgName} : {{@{sanitizedArgName}}}";
				}


				//copy argValues for passing to base logger (object[] params)
				var argValues = __.pool.GetArray<object>(argPairs.Count);
				for (var i = 0; i < argPairs.Count; i++)
				{
					argValues[i] = argPairs[i].value;
				}

				//invoke base ILogger functionality with our ez message/objsToLogg/Callsite            
				if (ex is null)
				{
					logger.Log(level, message, argValues);
				}
				else
				{
					message = $"[Exception:{exName}] " + message;
					logger.Log(level, ex, message, argValues);
				}
            //if running XUnit, also write to test output (console)
            if (__.Test.IsTestingActive)
            {
               __.Test.Write($"[{level.ToString().ToUpperInvariant()}] {originalMessage}", arg0,arg1,arg2,memberName,sourceFilePath,sourceLineNumber,argName0,argName1,argName2);
            }

				__.pool.ReturnArray(argValues);
			}
			finally
			{
				_argPairsCache.Clear();
			}
		}
	}

   public static void _EzTrace(this ILogger logger, string? message, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      logger._Ez(LogLevel.Trace, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }

   public static void _EzDebug(this ILogger logger, string? message, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      logger._Ez(LogLevel.Debug, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }

   public static void _EzWarn(this ILogger logger, string? message, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null")
   {
      logger._Ez(LogLevel.Warning, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }

   public static void _EzInfo(this ILogger logger, string? message, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      logger._Ez(LogLevel.Information, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }

   [DebuggerNonUserCode]
   public static void _EzError(this ILogger logger, string? message, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      logger._Ez(LogLevel.Error, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }

   public static void _EzTrace(this ILogger logger,bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
		object? objToLog2 = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0,
		[CallerArgumentExpression("objToLog0")]
		string? objToLog0Name = "null",
		[CallerArgumentExpression("objToLog1")]
		string? objToLog1Name = "null",
		[CallerArgumentExpression("objToLog2")]
		string? objToLog2Name = "null"
	)
	{
      if(condition)
      {
         return;
      }
		logger._Ez(LogLevel.Trace, message, objToLog0, objToLog1, objToLog2
			, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
			objToLog1Name, objToLog2Name);
	}

	public static void _EzDebug(this ILogger logger, bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
		object? objToLog2 = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0,
		[CallerArgumentExpression("objToLog0")]
		string? objToLog0Name = "null",
		[CallerArgumentExpression("objToLog1")]
		string? objToLog1Name = "null",
		[CallerArgumentExpression("objToLog2")]
		string? objToLog2Name = "null"
	)
   {
      if (condition)
      {
         return;
      }
      logger._Ez(LogLevel.Debug, message, objToLog0, objToLog1, objToLog2
			, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
			objToLog1Name, objToLog2Name);
	}

	public static void _EzWarn(this ILogger logger, bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
		object? objToLog2 = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0,
		[CallerArgumentExpression("objToLog0")]
		string? objToLog0Name = "null",
		[CallerArgumentExpression("objToLog1")]
		string? objToLog1Name = "null",
		[CallerArgumentExpression("objToLog2")]
		string? objToLog2Name = "null")
	{
		logger._Ez(LogLevel.Warning, message, objToLog0, objToLog1, objToLog2
			, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
			objToLog1Name, objToLog2Name);
	}

	public static void _EzInfo(this ILogger logger, bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
		object? objToLog2 = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0,
		[CallerArgumentExpression("objToLog0")]
		string? objToLog0Name = "null",
		[CallerArgumentExpression("objToLog1")]
		string? objToLog1Name = "null",
		[CallerArgumentExpression("objToLog2")]
		string? objToLog2Name = "null"
	)
   {
      if (condition)
      {
         return;
      }
      logger._Ez(LogLevel.Information, message, objToLog0, objToLog1, objToLog2
			, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
			objToLog1Name, objToLog2Name);
	}

   public static void _EzError(this ILogger logger, bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      if (condition)
      {
         return;
      }
      logger._Ez(LogLevel.Error, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }

   [DoesNotReturn]
   public static void _EzErrorThrow(this ILogger logger, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      logger._EzErrorThrow(false, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
   }
   public static void _EzErrorThrow(this ILogger logger, [DoesNotReturnIf(false)] bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   )
   {
      if (condition)
      {
         return;
      }
      logger._Ez(LogLevel.Error, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);
      throw new LoLoDiagnosticsException(message._FormatAppendArgs(objToLog0, objToLog1, objToLog2,objToLog0Name,objToLog1Name, objToLog2Name), memberName, sourceFilePath, sourceLineNumber);
   }
   
   public static void _EzErrorThrow<TException>(this ILogger logger, [DoesNotReturnIf(false)] bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null",
      [CallerArgumentExpression("condition")]
      string? conditionName = "null"
   ) where TException: Exception, new()
   {
      if (condition)
      {
         return;
      }
      logger._Ez(LogLevel.Error, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name);

      TException ex = null;
      try
      {
         ex = typeof(TException)._CreateInstance<TException>($"ERROR_THROW({conditionName}) {message}");
         ex.Source = $"{memberName}:{sourceFilePath}:{sourceLineNumber}";
         throw ex;
      }
      catch (Exception e)
      {
         throw new LoLoDiagnosticsException(message._FormatAppendArgs(objToLog0, objToLog1, objToLog2, objToLog0Name, objToLog1Name, objToLog2Name)
            + $"(Could not create {typeof(TException).Name}, creating LoLoDiagnosticsException instead)"
            , memberName, sourceFilePath, sourceLineNumber);      
      }
   }
   [Conditional("CHECKED")]
   public static void _EzCheckedThrow<TException>(this ILogger logger, [DoesNotReturnIf(false)] bool condition, string? message = null, object? objToLog0 = null, object? objToLog1 = null,
      object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null"
   ) where TException : Exception, new()
   {
      _EzErrorThrow<TException>(logger, condition, message, objToLog0, objToLog1, objToLog2, memberName, sourceFilePath, sourceLineNumber, objToLog0Name, objToLog1Name, objToLog2Name);

   }

      public static TException _EzTrace<TException>(this ILogger logger, TException ex, string? message = null, object? objToLog0 = null,
      object? objToLog1 = null, object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null",
      [CallerArgumentExpression("ex")] string? exName = null
   ) where TException : Exception
   {
		logger._Ez(LogLevel.Trace, message, objToLog0, objToLog1, objToLog2
			, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
			objToLog1Name, objToLog2Name, ex, exName);
      return ex;
   }

   public static TException _EzInfo<TException>(this ILogger logger, TException ex, string? message = null, object? objToLog0 = null,
      object? objToLog1 = null, object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null",
      [CallerArgumentExpression("ex")] string? exName = null
   ) where TException : Exception
   {
      logger._Ez(LogLevel.Information, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name, ex, exName);
      return ex;
   }


   public static TException _EzDebug<TException>(this ILogger logger, TException ex, string? message = null, object? objToLog0 = null,
      object? objToLog1 = null, object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null",
      [CallerArgumentExpression("ex")] string? exName = null
   ) where TException: Exception
   {
      logger._Ez(LogLevel.Debug, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name, ex, exName);
      return ex;
   }


   public static TException _EzWarn<TException>(this ILogger logger, TException ex, string? message = null, object? objToLog0 = null,
      object? objToLog1 = null, object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null",
      [CallerArgumentExpression("ex")] string? exName = null
   ) where TException : Exception
   {
      logger._Ez(LogLevel.Warning, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name, ex, exName);
      return ex;
   }
   public static TException _EzError<TException>(this ILogger logger, TException ex, string? message = null, object? objToLog0 = null,
      object? objToLog1 = null, object? objToLog2 = null,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerArgumentExpression("objToLog0")]
      string? objToLog0Name = "null",
      [CallerArgumentExpression("objToLog1")]
      string? objToLog1Name = "null",
      [CallerArgumentExpression("objToLog2")]
      string? objToLog2Name = "null",
      [CallerArgumentExpression("ex")] string? exName = null
   ) where TException : Exception
   {
      logger._Ez(LogLevel.Error, message, objToLog0, objToLog1, objToLog2
         , memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber, objToLog0Name,
         objToLog1Name, objToLog2Name, ex, exName);
      return ex;
   }



   public static void _Kill(this ILogger logger, bool condition, string message = null, Exception? innerException = null,
           [CallerArgumentExpression("condition")]
                string? conditionName = null,
                [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "",
                     [CallerLineNumber] int sourceLineNumber = 0)
   {
      logger._KillHelper(LogLevel.Error, condition, message, innerException, conditionName, memberName, sourceFilePath,
                 sourceLineNumber);
   }
   public static void _Kill(this ILogger logger, string message, Exception? innerException = null,
                [CallerArgumentExpression("condition")]
                               string? conditionName = null,
                               [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "",
                                                   [CallerLineNumber] int sourceLineNumber = 0)
   {
      logger._KillHelper(LogLevel.Error, true, message, innerException, conditionName, memberName, sourceFilePath,
                         sourceLineNumber);
   }
   public static void _Kill(this ILogger logger, Exception innerException,
                     [CallerArgumentExpression("condition")]
                                                   string? conditionName = null,
                                                   [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "",
                                                                                                     [CallerLineNumber] int sourceLineNumber = 0)
   {
      logger._KillHelper(LogLevel.Error, true, null, innerException, conditionName, memberName, sourceFilePath,
                                 sourceLineNumber);
   }

   /// <summary>
   ///    something super bad happened. log it and kill the process
   /// </summary>
   [DebuggerNonUserCode]
   [DebuggerHidden]
   [DoesNotReturn]
   private static void _KillHelper(this ILogger logger, LogLevel level, bool condition, string message = null, Exception? innerException = null,
      [CallerArgumentExpression("condition")]
      string? conditionName = null,
      [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (condition)
      {
         return;
      }

      message ??= "KILL condition failed";

      var ex = new LoLoDiagnosticsException($"{level}_KILL({conditionName}) {message}", innerException);
      ex.Source = $"{memberName}:{sourceFilePath}:{sourceLineNumber}";

      _Ez(logger: logger,level:level,ex:ex, message: message, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber);

      _BreakIntoDebugger();

      Environment.FailFast(ex.Message, ex);
   }

   /// <summary>
   ///    helper to break into the debugger
   /// </summary>
   [DebuggerNonUserCode]
   [DebuggerHidden]
   private static void _BreakIntoDebugger()
   {
      if (Debugger.IsAttached == false)
      {
         Debugger.Launch();
      }

      if (Debugger.IsAttached)
      {
         Debugger.Break();
      }
   }


   public static bool _If(this ILogger logger, LogLevel level, Action? action = null)
	{
		if (logger.IsEnabled(level))
		{
			if (action is not null)
			{
				action.Invoke();
			}

			return true;
		}

		return false;
	}


	public static bool _IfTrace(this ILogger logger, Action? action = null)
	{
		return logger._If(LogLevel.Trace, action);
	}

	public static bool _IfDebug(this ILogger logger, Action? action = null)
	{
		return logger._If(LogLevel.Debug, action);
	}

	public static bool _IfWarn(this ILogger logger, Action? action = null)
	{
		return logger._If(LogLevel.Warning, action);
	}

	public static bool _IfError(this ILogger logger, Action? action = null)
	{
		return logger._If(LogLevel.Error, action);
	}

	public static bool _IfInfo(this ILogger logger, Action? action = null)
	{
		return logger._If(LogLevel.Information, action);
	}

}