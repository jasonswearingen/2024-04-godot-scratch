//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.CodeAnalysis;
//using System.Collections.Immutable;
//using System.Linq;
//using System.Text;


//[Generator(LanguageNames.CSharp)]
//public class InputMapGenerator : IIncrementalGenerator
//{
//   public void Initialize(IncrementalGeneratorInitializationContext context)
//   {
//      var additionalTexts = context.AdditionalTextsProvider.Where(static (file) =>
//          file.Path.EndsWith("project.godot")
//      );

//      var additionalTextsContent = additionalTexts.Select(static (additionalText, cancellationToken) =>
//          additionalText.GetText(cancellationToken)!.ToString()
//      );

//      var inputActions = additionalTextsContent.SelectMany(static (additionalTextContent, _) =>
//          additionalTextContent.Split('\n')
//                               .Where(static (line) => !string.IsNullOrWhiteSpace(line))
//                               .SkipWhile(static (line) => !line.StartsWith("[input]")).Skip(1)
//                               .TakeWhile(static (line) => !line.StartsWith("["))
//                               .Where(static (line) => line.IndexOf('=') > 0)
//                               .Select(static (line) => line.Split('=')[0])
//      );

//      var collected = inputActions.Collect();

//      context.RegisterSourceOutput(collected, CreateSource);
//   }

//   private static void CreateSource(SourceProductionContext context, ImmutableArray<string> inputActions)
//   {
//      var source = new StringBuilder();
//      _ = source.AppendLine("/*\n   This file was generated.\n   Do not edit.\n*/")
//                .AppendLine()
//                .AppendLine("using Godot;")
//                .AppendLine()
//                .AppendLine("namespace YOUR_NAMESPACE_HERE;")
//                .AppendLine()
//                .AppendLine("public static class InputMap")
//                .AppendLine("{");

//      foreach (var inputAction in inputActions)
//      {
//         var name = InputActiontoConstant(inputAction);
//         var value = $"\"{inputAction}\"";
//         _ = source.AppendLine($"    public static readonly StringName {name} = {value};");
//      }

//      _ = source.AppendLine("}");

//      context.AddSource("InputMap.g.cs", source.ToString());
//   }

//   private static string InputActiontoConstant(string inputAction)
//   {
//      // Specific naming conventions can be implemented here.
//      // Ours is: PascalCase, with specific edge cases (like ui should be translated as UI).
//      return string.Join("", inputAction.Replace('.', '_').Split('_').Where(word => word.Length > 0).Select(
//          static (word) => word.ToLowerInvariant() switch
//          {
//             "ui" => "UI",
//             _ => $"{word.Substring(0, 1).ToUpperInvariant()}{word.Substring(1)}",
//          }
//      ));
//   }
//}