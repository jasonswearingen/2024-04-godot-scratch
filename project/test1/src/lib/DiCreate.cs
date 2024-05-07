using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotEx.Reflections;
using Godot;
using GodotEx.Hosting;

namespace test1.src.lib;


public interface IEzNode { }

/// <summary>
/// mark a node's instance property with this attribute to have it automatically be referenced from the scene tree during _Ready().
/// If the property type is not a Node, it will be attached from the GlobalDIHost instead
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]

public class EzInjectAttribute : Attribute
{
   internal static readonly Type TYPE = typeof(EzInjectAttribute);

   private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
   private static Dictionary<Type, MemberInfo[]> _members = new();

   /// <summary>
   /// discover and attach all nodes with the EzInjectAttribute
   /// </summary>
   /// <param name="target"></param>
   /// <returns>count of remaining members that have not been filled (no injection found)</returns>
   public static int DiscoverAndInject(Node target, IServiceProvider serviceProvider)
   {
      if (Engine.IsEditorHint() is false && target is not IEzNode)
      {
         //early exit if not in editor mode and not marked as IEzNode (for perf)
         return 0;
      }
      var targetType = target.GetType();
      var childMembersToInspect = _GetMatchingMembers(targetType, TYPE,BINDING_FLAGS);

      var attachCount = 0;
      foreach (var childMemberInfo in childMembersToInspect)
      {

         //ensure child not already set
         var childNode = childMemberInfo.GetValue(target);
         if (childNode is not null)
         {
            throw new InvalidOperationException("child already set");
         }

         var injectedChildType = childMemberInfo.GetMemberType();


         //find the type in our serviceProvider
         var service = serviceProvider.GetService(injectedChildType);
         if (service is not null)
         {
            //we found a service, so set it
            childMemberInfo.SetValue(target, service);
            attachCount++;
            continue;
         }


      }

      if (attachCount>0 && target is not IEzNode)
      {
         throw new MissingMemberException("You did not mark the node inheriting 'IEzNode' so it won't be attached at runtime");
      }

      return childMembersToInspect.Length - attachCount;
      
   }





   private static MemberInfo[] _GetMatchingMembers(Type typeToInspect, Type typeToFind, BindingFlags typeToFindBindingFlags)
   {
      if (!_members.TryGetValue(typeToInspect, out var members))
      {
         var properties = typeToInspect.GetPropertiesWithAttribute(typeToFind, typeToFindBindingFlags);
         var fields = typeToInspect.GetFieldsWithAttribute(typeToFind, typeToFindBindingFlags);
         members = properties.Cast<MemberInfo>().Concat(fields).ToArray();
         _members[typeToInspect] = members;
      }

      return members;
   }
}

