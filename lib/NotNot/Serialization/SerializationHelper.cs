using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotNot.Serialization;

//public static class zz_Extensions_Object
//{
//	public record struct ToPoCoOptions(int MaxDepth = 10, bool IncludeFields = false,
//		bool IncludeRecursive = false,
//		bool IncludeNonPublic = false
//	);

//	public static object ToPoCo(this object obj, ToPoCoOptions options = default)
//	{
//		HashSet<object> visited = new();
//		return ToPoCo_Worker(obj, options, visited);
//	}
//	public static object ToPoCo_Worker(object obj, ToPoCoOptions options , HashSet<object> visited)
//	{
//		var type = obj.GetType();

//	}

//}

public class SerializationHelper
{

   /// <summary>
   /// configure sane defaults for http json options (de)serializing post body
   /// </summary>
   /// <param name="options"></param>
   public static void ConfigureJsonOptions(JsonSerializerOptions options)
   {
      //be forgiving in parsing user json
      options.ReadCommentHandling = JsonCommentHandling.Skip;
      options.AllowTrailingCommas = true;
      options.PropertyNameCaseInsensitive = true;
      options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.MaxDepth = 10;
      options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
      options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      options.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;
      options.UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip;
      options.WriteIndented = true;
      var newConverters = new List<JsonConverter>
      {
         new ObjConverter<MethodBase>(value => value.Name),
         new ObjConverter<Type>(value => value.FullName),
         new ObjConverter<StackTrace>(value => value.GetFrames()),
         new ObjConverter<StackFrame>(value =>
            $"at {value.GetMethod().Name} in {value.GetFileName()}:{value.GetFileLineNumber()}"),
         //new ObjConverter<StackFrame>((value) => $"{value.ToString()}\n"),
      };
      foreach (var converter in newConverters)
      {
         options.Converters.Add(converter);
      }

   }

   public static void ConfigureJsonOptions(JsonLoadSettings options)
   {
      options.DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Replace;
      options.CommentHandling = CommentHandling.Ignore;
      options.LineInfoHandling = LineInfoHandling.Ignore;
   }


   private SerializationHelper() { }

   private static ILogger _logger = __.GetLogger<SerializationHelper>();

   private static JsonSerializerOptions _jsonOptions = new()
   {
      MaxDepth = 10,
      IncludeFields = true,
      ReferenceHandler = ReferenceHandler.IgnoreCycles,
      Converters =
      {
         new ObjConverter<MethodBase>(value => value.Name),
         new ObjConverter<Type>(value => value.FullName),
         new ObjConverter<StackTrace>(value => value.GetFrames()),
         new ObjConverter<StackFrame>(value =>
            $"at {value.GetMethod().Name} in {value.GetFileName()}:{value.GetFileLineNumber()}"),
			//new ObjConverter<StackFrame>((value) => $"{value.ToString()}\n"),
		},
      AllowTrailingCommas = true,
      WriteIndented = true,
   };

   /// <summary>
   ///    converts input object into a "plain old collection object", a nested Dictionary/List structure.
   /// </summary>
   /// <param name="obj"></param>
   /// <returns></returns>
   public static object ToPoCo(object obj)
   {
      try
      {
         //serialize/deserialize to unknown collection
         //this process is used because System.Text.Json can better serialize various types that would cause Newtonsoft to throw PlatformNotSupported exceptions
         //however deserialize to unknown collection is not supported in System.Text.Json so we have to use Newtonsoft to deserialize to Dictionary/Array, using our custom JsonHelper.DeserializeUnknownType() function
         {
            var serialized = JsonSerializer.Serialize(obj, _jsonOptions);
            var deserialized = JsonToPoCo(serialized);
            return deserialized;
         }
      }
      catch (Exception ex)
      {
         __.GetLogger()._EzError(ex, "could not convert to PoCo due to json roundtrip error");
         //throw new ApplicationException("could not convert to PoCo due to json roundtrip error", ex);
         throw;
      }
   }
   /// <summary>
   /// converts input object into a "plain old collection object", then to JSON string
   /// </summary>
   /// <param name="obj"></param>
   /// <returns></returns>
   public static string ToJson(object obj)
   {
      var roundTrip = ToPoCo(obj);
      return JsonSerializer.Serialize(roundTrip, _jsonOptions);
   }

   /// <summary>
   ///    deserialize json to a Dictionary/List hiearchy.  This is useful for logging json of "unknown" or hard-to-deserialize
   ///    types.
   ///    best used with objects serialized via System.Text.Json, using the ReferenceHandler = ReferenceHandler.IgnoreCycles
   ///    option.
   ///    adapted from this answer https://stackoverflow.com/a/19140420/1115220
   ///    via
   ///    https://stackoverflow.com/questions/5546142/how-do-i-use-json-net-to-deserialize-into-nested-recursive-dictionary-and-list
   /// </summary>
   public static object JsonToPoCo(string json)
   {
      //Console.WriteLine($"DeserializeUnknownType {json}");
      return ToObject(JToken.Parse(json));
   }

   /// <summary>
   /// </summary>
   /// <param name="token"></param>
   /// <param name="discardMetaNodes">
   ///    TRUE useful to remove metadata nodes (starting with $) if ReferenceHandler.Preserve
   ///    option is used. not useful otherwise.
   /// </param>
   /// <returns></returns>
   private static object ToObject(JToken token, bool discardMetaNodes = false)
   {
      switch (token.Type)
      {
         // key/value node
         case JTokenType.Object:
            {
               if (discardMetaNodes == false)
               {
                  return token.Children<JProperty>()
                     .ToDictionary(prop => prop.Name,
                        prop => ToObject(prop.Value, discardMetaNodes));
               }

               var dict = new Dictionary<string, object>();


               foreach (var prop in token.Children<JProperty>())
               {
                  if (prop.Name == "$values")
                  {
                     //if (dict.Count > 0)
                     //{
                     //	throw new ApplicationException("assume no other value nodes if $values is present");
                     //}

                     //just return the value metdata node
                     return ToObject(prop.Value, discardMetaNodes);
                  }

                  if (prop.Name.StartsWith("$"))
                  {
                     continue;
                  }

                  dict.Add(prop.Name, ToObject(prop.Value, discardMetaNodes));
               }

               return dict;
            }
         // array node
         case JTokenType.Array:
            {
               return token.Select(tok => ToObject(tok, discardMetaNodes)).ToList();
            }
         // simple node
         default:
            return ((JValue)token).Value;
      }
   }
}