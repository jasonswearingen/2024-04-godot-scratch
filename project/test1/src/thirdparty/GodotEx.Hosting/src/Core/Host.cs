using Godot;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GodotEx.Hosting;


/// <summary>
/// A node that provides hosting service. Override <see cref="ConfigureServices(IServiceCollection)"/>
/// to configure services added to the internal <see cref="ServiceProvider"/>. 
/// <br/><br/>
/// Includes default services, namely:
/// <br/>Current <see cref="Host"/>
/// <br/><see cref="SceneTree"/>
/// <br/><see cref="DependencyInjector"/>
/// <br/><see cref="NodeResolver"/>
/// <br/><see cref="SingleNodeManager"/>
/// </summary>
/// <remarks>
/// <b>Note</b>: host nodes can exist any where within the current scene, 
/// but there may only be at most one autoload host.
/// </remarks>
public abstract partial class Host : Node
{
   /// <summary>
   /// Autoload host instance configured in Project Settings.
   /// </summary>
   public static Host Autoload { get; protected set; }

   public ServiceProvider serviceProvider;


   public Host()
   {
      GD.Print($"Host({GetInstanceId()}/{this.GetHashCode()}).ctor()  AutoLoad={Host.Autoload?.IsHostInitialized}");
      GD.Print($"IsInsideTree:{IsInsideTree()}");
   }
   /// <summary>
   /// Called when the node enters the Godot.SceneTree (e.g. upon instancing, scene
   /// changing, or after calling <see cref="Node.AddChild(Node, bool, InternalMode)"/>
   /// in a script). If the node has children, its Godot.Node._EnterTree callback will
   /// be called first, and then that of the children.
   /// </summary>
   public override void _EnterTree()
   {
      GD.Print("HOST ENTER TREE.");
      if (GetParent() == GetTree().Root && this != GetTree().CurrentScene)
      {
         if (Autoload != null)
         {
            throw new InvalidOperationException("Autoload host exists already.");
         }
         Autoload = this;
      }
      


      DoInit();
   }

   private void DoInit()
   {
      GD.Print("HOST.DoInit()");
      IsHostInitialized = true;
      var services = new ServiceCollection();
      ConfigureServices(services);
      serviceProvider = services.BuildServiceProvider();

      var eagerTypes = Assembly.GetExecutingAssembly().GetTypes()
          .Concat(Assembly.GetCallingAssembly().GetTypes())
          .Where(t => t.IsDefined(typeof(EagerAttribute)));

      foreach (var eagerType in eagerTypes)
      {
         var eagerService = serviceProvider.GetService(eagerType);
         if (eagerService is null)
         {
            throw new InvalidOperationException($"No service of type {eagerType.Name} found.");
         }
         else
         {
            //no-op, we just want to instantiate the eager service "early"
         }
      }
   }

   public bool IsHostInitialized;

   public bool TryInit()
   {
      GD.Print($"Host.TryInit()  IsHostInitialized={IsHostInitialized}");
      if (!IsHostInitialized)
      {
         DoInit();
         return true;
      }
      return false;
   }


   /// <summary>
   /// Configures service collection.
   /// </summary>
   /// <param name="services">Service collection to configure.</param>
   protected virtual void ConfigureServices(IServiceCollection services)
   {
      GD.Print("HOST CONFIGURING SERVICES");
      services.AddSingleton(GetType(), this);
      services.AddSingleton(GetTree());
      services.AddSingleton<DependencyInjector>();
      services.AddSingleton<NodeResolver>();
      services.AddSingleton<SingleNodeManager>();
   }

   protected override void Dispose(bool disposing)
   {
      GD.Print("HOST DISPOSING: " + disposing);

      if (disposing)
      {
         if (serviceProvider is not null)
         {
            //detach and dispose all configured services
            var services = serviceProvider.GetServices<IDisposable>();
            foreach (var service in services)
            {
               if(service is Node node)
               {                  
                  //node lifecycle is managed by the scene tree they are attached to
                  continue;
               }
               try
               {
                  service.Dispose();
               } catch (Exception ex) {
                  GD.PrintErr($"Error disposing service: {ex.Message}");
               }
            }
            serviceProvider.Dispose();
         }
         serviceProvider = null;
         IsHostInitialized = false;
      }


      base.Dispose(disposing);

   }
}
