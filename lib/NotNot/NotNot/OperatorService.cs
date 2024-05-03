using System.Runtime.CompilerServices;
using Spectre.Console;

namespace NotNot;

/// <summary>
/// ez console based I/O for "operator" use.
/// </summary>
public class OperatorService : ISingletonService
{
   public bool Confirm(Color color, string message, bool defaultValue = false)
   {
      return Confirm(color._MarkupString(message), defaultValue);
   }

   public bool Confirm(string message, bool defaultValue = false)
   {
      var result = AnsiConsole.Confirm(message, defaultValue);
      return result;
   }

   public void WriteLine(Color color, string? message = null, object? arg0 = null, object? arg1 = null, object? arg2 = null,
      [CallerArgumentExpression("arg0")] string argName0 = "null",
      [CallerArgumentExpression("arg1")] string argName1 = "null",
      [CallerArgumentExpression("arg2")] string argName2 = "null")
   {
      //WriteLine( color._MarkupString(message), arg0, arg1, arg2, argName0, argName1, argName2);
      var coloredMessage = color._MarkupString(message);
      if (coloredMessage is not null)
      {
         var finalMessage = coloredMessage._FormatAppendArgs(arg0, arg1, arg2, argName0, argName1, argName2);
         AnsiConsole.MarkupLine(finalMessage);
      }
      else
      {
         AnsiConsole.WriteLine();
      }
   }
   public Color DefaultColor { get; set; } = Color.Orchid;
   public void WriteLine(string? message = null, object? arg0 = null, object? arg1 = null, object? arg2 = null,
      [CallerArgumentExpression("arg0")] string argName0 = "null",
      [CallerArgumentExpression("arg1")] string argName1 = "null",
      [CallerArgumentExpression("arg2")] string argName2 = "null")
   {
      WriteLine(DefaultColor, message, arg0, arg1, arg2, argName0, argName1, argName2);

      //if (message is not null)
      //{
      //   var finalMessage = message._FormatAppendArgs(arg0, arg1, arg2, argName0, argName1, argName2);
      //   AnsiConsole.MarkupLine(finalMessage);
      //}
      //else
      //{
      //   AnsiConsole.WriteLine();
      //}
   }
}