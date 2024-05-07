using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotNot;

namespace test1.src.lib;


public static class DI
{
   [ModuleInitializer]
   public static void Initialize()
   {
      // This code runs when the assembly containing this class is loaded      
      //_GD.Print("ModuleInitializer.Initialize(): Assembly loaded..", Colors.DarkOrange);
      _GD.Log("[[[ === DI_ModuleInitializer COLD RELOAD OF MANAGED PROJECT === ]]]", Colors.DarkOrange);

      //_GD.Log($"Singletons: {Engine.GetSingletonList().Join()}");


      var mainLoop = Engine.GetMainLoop();
      if (mainLoop is SceneTree sceneTree)
      {
         _GD.Log($"scenetree: , {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}");
         sceneTree.NodeAdded += SceneTree_NodeAdded;
         sceneTree.NodeRemoved += SceneTree_NodeRemoved;
         sceneTree.NodeConfigurationWarningChanged += SceneTree_NodeConfigurationWarningChanged;
         
      }

      globalHost = new();
      globalHost.Initialize(CancellationToken.None)._SyncWait();

      
   }

   private static void SceneTree_NodeConfigurationWarningChanged(Node node)
   {
      var sceneTree = _Engine.SceneTree;
      //_GD.Log($"SceneTree_NodeConfigurationWarningChanged, {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}, node={node.Name}:{node.GetInstanceId()} x {node.GetHashCode()}");
   }

   private static void SceneTree_NodeRemoved(Node node)
   {
      var sceneTree = Engine.GetMainLoop() as SceneTree;
      //_GD.Log($"SceneTree_NodeRemoved, {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}, node={node.Name}:{node.GetInstanceId()} x {node.GetHashCode()}");
   }

   private static void SceneTree_NodeAdded(Node node)
   {

      var sceneTree = Engine.GetMainLoop() as SceneTree;



      //_GD.Log($"SceneTree_NodeAdded, {sceneTree.GetInstanceId()} x {sceneTree.GetHashCode()}, node={node.Name}:{node.GetInstanceId()} x {node.GetHashCode()}");
   }

   public static DiHostBase globalHost;
}



public partial class DiHost : Node
{
   public override void _Ready()
   {
      base._Ready();

     // AddChild(new EditorHotReloadHelper());

      
   }

}



public class DiHostBase : IDisposable
{
   //public static GlobalDiHost Instance { get; protected set; }

   //public GlobalDiHost()
   //{
   //   if (Instance is not null)
   //   {
   //      throw new Exception("GlobalDiHost.Instance already exists.  this is supposed to be a singleton autoload configured in your Godot Project Settings");
   //   }
   //   Instance = this;
   //   Initialize();

   //}


   public bool IsInitialized { get; private set; } = false;

   protected IHost DiHost { get; set; }
   public IServiceProvider serviceProvider { get; protected set; }

   public virtual async Task Initialize(CancellationToken ct)
   {
      if (IsInitialized)
      {
         throw new Exception("GlobalDiHost already initialized.");
      }
      IsInitialized = true;

      //hostBuilder workflow
      var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
      builder.Configuration.AddJsonFile("appsettings.json", optional: false);

      //configure app specific services firstly
      ConfigureServices(builder.Services);

      await builder._NotNotEzSetup(ct);


      //configure NotNot default services
      DiHost = builder.Build();
      serviceProvider = DiHost.Services;

      
   }



   public void Dispose()
   {
      if (serviceProvider is not null)
      {

         if (serviceProvider is IDisposable disposable)
         {
            disposable.Dispose();
         }

         ////detach and dispose all configured services
         //var services = serviceProvider.GetServices<IDisposable>();
         //foreach (var service in services)
         //{
         //   if (service is Node node)
         //   {
         //      //node lifecycle is managed by the scene tree they are attached to
         //      continue;
         //   }
         //   try
         //   {
         //      service.Dispose();
         //   }
         //   catch (Exception ex)
         //   {
         //      GD.PrintErr($"Error disposing service: {ex.Message}");
         //   }
         //}
      }

      if (this.DiHost is not null)
      {
         DiHost.Dispose();
      }
      serviceProvider = null;
      DiHost = null;
   }

   protected virtual void ConfigureServices(IServiceCollection services)
   {
      _GD.Print("HOST CONFIGURING SERVICES");

      services.AddSingleton(this);
   }

}
