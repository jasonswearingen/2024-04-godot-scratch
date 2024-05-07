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
internal class DiCreate
{
   private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


   private readonly Dictionary<Type, MemberInfo[]> _members = new();







}

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
   public static void DiscoverAndInject(Node target)
   {
      if (Engine.IsEditorHint() is false && target is not IEzNode)
      {
         return;
      }
      var type = target.GetType();

      var members = _GetMatchingMembers(type, TYPE,BINDING_FLAGS);

      var attachCount = 0;
      foreach (var childMemberInfo in members)
      {

         //ensure child not already set
         var childNode = childMemberInfo.GetValue(target);
         if (childNode is not null)
         {
            throw new InvalidOperationException("child already set");
         }


         var injectedChildType = childMemberInfo.GetMemberType();



         if (injectedChildType._IsAssignableTo<Node>())
         {
            //this is a godot node, so find it in the tree

            //find child in scene tree
            var child = target.GetNodeOrNull(injectedChildType.Name);
            //target.GetTree().EditedSceneRoot._FindChild()
         }



         



         if ( childNode is not Node cn)
         {
            throw new InvalidCastException("child not Node");
         }
         var parent = cn.GetParent();
         if(parent is null)
         {
            target.AddChild(cn);
            attachCount++;
         }
         else if(parent.GetInstanceId() != target.GetInstanceId())
         {
            throw new InvalidOperationException(" child already has parent");
         }
         //if (parent is not null)
         //{
         //   throw new InvalidOperationException("child already has parent");
         //}
      }

      if (attachCount>0 && target is not IEzNode)
      {
         throw new MissingMemberException("You did not mark the node inheriting 'IEzNode' so it won't be attached at runtime");
      }
      
   }


   public static void DiscoverAndDetatch(Node target)
   {
      if (Engine.IsEditorHint() is false && target is not IEzNode)
      {
         return;
      }
      var type = target.GetType();

      var members = _GetMatchingMembers(type, TYPE, BINDING_FLAGS);

      var detachCount = 0;
      foreach (var member in members)
      {
         var memberType = member.GetMemberType();
         

         var childNode = member.GetValue(target);
         if (childNode is null)
         {
            //throw new InvalidCastException("child null");
            continue;
         }
         if (childNode is not Node cn)
         {
            throw new InvalidCastException("child not Node");
         }
         if (cn.GetParent() is null)
         {

            throw new InvalidOperationException("child already has no parent");
         }
         target.RemoveChild(cn);

         member.SetValue(target, null);

         detachCount++;
      }

      if (detachCount > 0 && target is not IEzNode)
      {
         throw new MissingMemberException("You did not mark the node inheriting 'IEzNode' so it won't be attached at runtime");
      }

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

