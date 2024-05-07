﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotNot.Internal;
using Godot;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System;
using System.Globalization;
using System.IO;
using System.Linq;


/// <summary>
/// godot sink for serilog.  complements of @paulloz: https://gist.github.com/paulloz/a0a01539ed96298682005ce61ba33a90
/// </summary>
public class GodotSink : ILogEventSink
{
   private readonly ITextFormatter formatter;

   public GodotSink(string outputTemplate, IFormatProvider? formatProvider)
   {
      formatter = new TemplateRenderer(outputTemplate, formatProvider);
   }

   public void Emit(LogEvent logEvent)
   {
      using TextWriter writer = new StringWriter();
      formatter.Format(logEvent, writer);
      writer.Flush();

      string color = logEvent.Level switch
      {
         LogEventLevel.Debug => Colors.SpringGreen.ToHtml(),
         LogEventLevel.Information => Colors.Cyan.ToHtml(),
         LogEventLevel.Warning => Colors.Yellow.ToHtml(),
         LogEventLevel.Error => Colors.Red.ToHtml(),
         LogEventLevel.Fatal => Colors.Purple.ToHtml(),
         _ => Colors.LightGray.ToHtml(),
      };

      foreach (string line in writer.ToString()?.Split('\n') ?? Array.Empty<string>())
         GD.PrintRich($"[color=#{color}]{line}[/color]");

      if (logEvent.Exception is null) return;

      if (logEvent.Level >= LogEventLevel.Error)
         GD.PushError(logEvent.Exception);
      else
         GD.PushWarning(logEvent.Exception);
   }

   private class TemplateRenderer : ITextFormatter
   {
      private delegate void Renderer(LogEvent logEvent, TextWriter output);

      private readonly Renderer[] renderers;
      private readonly IFormatProvider? formatProvider;

      public TemplateRenderer(string outputTemplate, IFormatProvider? formatProvider)
      {
         this.formatProvider = formatProvider;

         MessageTemplate template = new MessageTemplateParser().Parse(outputTemplate);
         renderers = template.Tokens.Select(
         token => token switch
         {
            TextToken textToken => (_, output) => output.Write(textToken.Text),
            PropertyToken propertyToken => propertyToken.PropertyName switch
            {
               OutputProperties.LevelPropertyName
                      => (logEvent, output) => output.Write(logEvent.Level),
               OutputProperties.MessagePropertyName
                      => (logEvent, output) => logEvent.RenderMessage(output, formatProvider),
               OutputProperties.NewLinePropertyName
                      => (_, output) => output.Write('\n'),
               OutputProperties.TimestampPropertyName
                      => RenderTimestamp(propertyToken.Format),
               _
                      => RenderProperty(propertyToken.PropertyName, propertyToken.Format),
            },
            _ => null,
         }
         ).OfType<Renderer>().ToArray();
      }

      public void Format(LogEvent logEvent, TextWriter output)
      {
         foreach (var renderer in renderers)
            renderer.Invoke(logEvent, output);
      }

      private Renderer RenderTimestamp(string? format)
      {
         Func<LogEvent, string> f = formatProvider?.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter formatter
             ? (logEvent) => formatter.Format(format, logEvent.Timestamp, formatProvider)
             : (logEvent) => logEvent.Timestamp.ToString(format, formatProvider ?? CultureInfo.InvariantCulture);

         return (logEvent, output) => output.Write(f(logEvent));
      }

      private Renderer RenderProperty(string propertyName, string? format)
      {
         return delegate (LogEvent logEvent, TextWriter output)
         {
            if (logEvent.Properties.TryGetValue(propertyName, out var propertyValue))
               propertyValue.Render(output, format, formatProvider);
         };
      }
   }
}

public static class GodotSinkExtensions
{
   private const string DefaultGodotSinkOutputTemplate = "[{Timestamp:HH:mm:ss}] {Message:lj}";

   public static LoggerConfiguration Godot(this LoggerSinkConfiguration configuration,
                                           string outputTemplate = DefaultGodotSinkOutputTemplate,
                                           IFormatProvider? formatProvider = null)
   {
      return configuration.Sink(new GodotSink(outputTemplate, formatProvider));
   }
}