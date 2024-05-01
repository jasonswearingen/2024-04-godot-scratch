using System.Runtime.CompilerServices;

namespace NotNot;

/// <summary>
///    high performance reflection helpers
/// </summary>
public class ReflectHelper
{
   public static ReflectHelper Instance = new();

   /// <summary>
   ///    return details about the callsite of the caller
   ///    this is generated at build time, so no performance impact.
   /// </summary>
   public (string MemberName, string FilePath, int LineNumber) GetCallerInfo([CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
   {
      return (memberName, sourceFilePath, sourceLineNumber);
   }
}