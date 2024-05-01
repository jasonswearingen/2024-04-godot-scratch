
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using NotNot;
using NotNot.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using NotNot.DI.Advanced;
using NotNot.SwaggerGen.Advanced;
using Scrutor;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZiggyCreatures.Caching.Fusion;

public static class zz_Extensions_HostApplicationBuilder
{

   private static ObjectTrackingCollection<object> _initializedServiceTracker = new();

   /// <summary>
   /// registeres services extending interfaces like ISingleton and also decorates services implementing IAutoInitialize with a call to .AutoInitialize()
   /// </summary>
   /// <param name="services"></param>
   /// <param name="ct"></param>
   /// <param name="scanAssemblies">assemblies you want to scan for automapper and scrutor types.  default is everything: AppDomain.CurrentDomain.GetAssemblies()</param>
   /// <param name="scanIgnore">assemblies to not scan for DI types.   by default this is 'Microsoft.*' because ASP NetCore IHostedService internal registrations conflict.</param>
   /// <returns></returns>
   public static async Task _NotNotEzSetup(this IHostApplicationBuilder builder, CancellationToken ct, IEnumerable<Assembly>? scanAssemblies = null, IEnumerable<string>? scanIgnore = null)
   {
      NotNot.Secrets.SecretsLoader.LoadSecrets(builder.Configuration);

      await _NotNotUtils_ConfigureLogging(builder, ct);
      await _NotNotUtils_ConfigureCache(builder, ct);

      scanAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();
      var targetAssemblies = new List<Assembly>(scanAssemblies);
      scanIgnore ??= new[] { "Microsoft.*" }; // by default microsoft so we don't step on it's internal DI registrations

      //ensure this assembly is included in targetAssemblies
      //this is so various DI services inside this assembly can be auto-registered
      var thisAssembly = Assembly.GetExecutingAssembly();
      if (!targetAssemblies.Contains(thisAssembly))
      {
         targetAssemblies.Add(thisAssembly);
      }
      //remove ignored assemblies. 
      Microsoft.Extensions.FileSystemGlobbing.Matcher matcher = new();
      matcher.AddIncludePatterns(scanIgnore);

      for (var i = targetAssemblies.Count - 1; i >= 0; i--)
      {
         var current = targetAssemblies[i];
         var name = current.FullName;
         var results = matcher.Match(name);
         if (results.HasMatches)
         {
            targetAssemblies.RemoveAt(i);
         }

      }


      //add automapper type mappings found in all assemblies
      builder.Services.AddAutoMapper(targetAssemblies);

      await _ScrutorRegisterServiceInterfaces(builder, ct, targetAssemblies);


      await _DecorateAutoInitializeServices(builder, ct);

      await _NotNotUtils_ConfigureSwaggerGen(builder, ct);




   }

   /// <summary>
   /// nice caching subsystem.  docs for fusionCache here: https://github.com/ZiggyCreatures/FusionCache/tree/main
   /// </summary>
   internal static async Task _NotNotUtils_ConfigureCache(this IHostApplicationBuilder builder, CancellationToken ct)
   {

      //builder.Services.AddMemoryCache();
      //builder.Services.AddDistributedMemoryCache();


      //verify that NotNot.Cache node exists in AppSettings.Json
      var cacheNode = builder.Configuration.GetSection("NotNot.Cache");
      if (!cacheNode.Exists())
      {
         __.GetLogger()._EzError("NotNot.Cache node not found in AppSettings.json.  FusionCache will use defaults");
      }


      //config cache
      builder.Services.AddFusionCache()
         //.WithOptions(opt => { 

         //})
         .WithDefaultEntryOptions(opt =>
         {
            opt.Duration = TimeSpan.FromSeconds(builder.Configuration.GetValue<double?>("NotNot.Cache.DurationDefaultSec") ?? 33);
            opt.FailSafeMaxDuration = TimeSpan.FromSeconds(builder.Configuration.GetValue<double?>("NotNot.Cache.DurationMaxFailSafeSec") ?? 77);
            opt.IsFailSafeEnabled = builder.Configuration.GetValue<bool?>("NotNot.Cache.IsFailSafeEnabled") ?? true;
            //opt.FactorySoftTimeout = TimeSpan.FromMilliseconds(100);
         })
         ;

   }

   internal static async Task _NotNotUtils_ConfigureLogging(this IHostApplicationBuilder builder, CancellationToken ct)
   {

      //config logging
      builder.Logging.ClearProviders();
      builder.Services.AddSerilog((hostingContext, loggerConfiguration) =>
         loggerConfiguration.ReadFrom.Configuration(builder.Configuration)
#if DEBUG
            .WriteTo.Sink(new NotNot.Logging.AssertOnMsgSink(builder.Configuration), LogEventLevel.Warning)
#endif
      );

   }

   /// <summary>
   /// does general swaggergen configs:  adding xml docs, adding [SwaggerIgnore] or [SwaggerExample] attributes
   /// </summary>
   /// <param name="builder"></param>
   /// <param name="ct"></param>
   /// <returns></returns>
   internal static async Task _NotNotUtils_ConfigureSwaggerGen(this IHostApplicationBuilder builder, CancellationToken ct)
   {
      builder.Services.ConfigureSwaggerGen((options) =>
      {

         //use globbing to load xml docs from all assemblies, to be used for swagger request/response examples docgen
         var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
         xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));


         //Add custom SwaggerGen filters so you can decorate properties with [SwaggerIgnore] or [SwaggerExample] attributes
         options.SchemaFilter<SwaggerSchemaFilter_ApplyAttributes>();
         options.OperationFilter<SwaggerOperationFilter_DiscoverUsedSchemas>();

         options.DocumentFilter<SwaggerDocumentFilter_RebuildSchema>();
      });
   }

   /// <summary>
   /// hooks up all services that implement ISingleton, ITransient, IScoped to be auto-registered
   /// </summary>
   /// <param name="services"></param>
   /// <param name="ct"></param>
   /// <returns></returns>
   [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
   internal static async Task _ScrutorRegisterServiceInterfaces(this IHostApplicationBuilder builder, CancellationToken ct, IEnumerable<Assembly> targetAssemblies)
   {


      //logger.LogTrace("utilizing Scrutor auto-registration of DI services....");

      //use Scrutor nuget library to auto-register DI Services
      builder.Services.Scan(scan =>
      {
         //register interfaces

         void RegisterHosted<TInterface>()
         {
            scan.FromAssemblies(targetAssemblies)
               .AddClasses(classes => classes.AssignableTo<TInterface>())
               .UsingRegistrationStrategy(RegistrationStrategy.Append)
               .AsSelfWithInterfaces()
               .WithSingletonLifetime();

         }
         void RegisterSingleton<TInterface>()
         {
            //as interface, append
            scan.FromAssemblies(targetAssemblies)
               .AddClasses((classes) =>
               {
                  var result = classes.AssignableTo<TInterface>();


               })
               //.AddClasses(classes => classes.Where(t => !t.IsGenericTypeDefinition &&
               //                                          t.GetInterfaces().Any(i => i.IsGenericType &&
               //                                                                     i.GetGenericTypeDefinition() == typeof(TInterface).GetGenericTypeDefinition())))
               //.AddClasses(classes => classes.Where(t => !t.IsGenericTypeDefinition &&
               //                                          t.BaseType != null && t.BaseType.GetInterfaces().Any(i => i.IsGenericType &&
               //                                                                                                    i.GetGenericTypeDefinition() == typeof(TInterface).GetGenericTypeDefinition())))
               //.AddClasses(classes => classes.Where(t => !t.IsGenericTypeDefinition &&
               //                                          t.BaseType != null && t.BaseType._IsAssignableTo<TInterface>()))
               //.AddClasses(classes => classes.Where(t => !t.IsGenericTypeDefinition &&
               //                                          t._IsAssignableTo<TInterface>()))

               .UsingRegistrationStrategy(RegistrationStrategy.Append)
               .AsSelfWithInterfaces()
               .WithSingletonLifetime();
         }

         void RegisterTransient<TInterface>()
         {
            //as interface, append
            scan.FromAssemblies(targetAssemblies)
               .AddClasses(classes => classes.AssignableTo<TInterface>())
               .UsingRegistrationStrategy(RegistrationStrategy.Append)
               .AsSelfWithInterfaces()
               .WithTransientLifetime();
         }

         void RegisterScoped<TInterface>()
         {
            //as interface, append
            scan.FromAssemblies(targetAssemblies)
               .AddClasses(classes => classes.AssignableTo<TInterface>())
               .UsingRegistrationStrategy(RegistrationStrategy.Append)
               .AsSelfWithInterfaces()
               .WithScopedLifetime()
               ;
         }

         RegisterHosted<IHostedService>();
         RegisterSingleton<ISingletonService>();
         RegisterTransient<ITransientService>();
         RegisterScoped<IScopedService>();
      });

   }


   //// Skip open generic types
   /// if (serviceType.Name.StartsWith("Cache"))
   /// {
   /// var xxx = 0;
   /// }
   //if (serviceType.IsGenericTypeDefinition)
   //{
   //   continue;
   //}

   public static class TypeDiscovery
   {
      public static List<Type> FindClosedGenericsOfOpenGeneric(Type openGenericType)
      {



         if (!openGenericType.IsGenericTypeDefinition)
         {
            throw new ArgumentException("The provided type must be an open generic type", nameof(openGenericType));
         }
         // Get all loaded assemblies
         Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

         var closedTypes = new List<Type>();

         foreach (var assembly in assemblies)
         {
            foreach (var type in assembly.GetTypes())
            {
               // Check if the type is a generic type and if it is constructed from the openGenericType
               if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType)
               {
                  closedTypes.Add(type);
               }
            }
         }

         return closedTypes;
      }
   }

   internal static async Task _DecorateAutoInitializeServices(this IHostApplicationBuilder builder, CancellationToken ct)
   {
      var services = builder.Services;

      foreach (var serviceDescriptor in services.ToList())
      {
         var serviceType = serviceDescriptor.ServiceType;


         if (typeof(IAutoInitialize).IsAssignableFrom(serviceType))
         {
            //ok
         }
         else if (serviceDescriptor.ImplementationType != null && typeof(IAutoInitialize).IsAssignableFrom(serviceDescriptor.ImplementationType))
         {
            //ok
         }
         else if (serviceDescriptor.ImplementationInstance != null && typeof(IAutoInitialize).IsAssignableFrom(serviceDescriptor.ImplementationInstance.GetType()))
         {
            //ok
         }
         else
         {
            //not assignable
            continue;
         }

         Decorate_AutoInit_ServiceRegistrationUpdater.DecorateService(services, serviceDescriptor);

      }
   }


   /// <summary>
   /// hooks up all services that implement IAutoInitialize to be decorated with a call to .AutoInitialize()
   /// </summary>
   internal static async Task _ScrutorHookAutoInitialize_old(this IHostApplicationBuilder builder, CancellationToken ct)
   {
      var hasImplementation = builder.Services.Where(sd =>
      {
         var serviceType = sd.ServiceType;

         if (typeof(IAutoInitialize).IsAssignableFrom(serviceType))
         {
            return true;
         }

         //can't check factory return type, so always have to decorate and try their returned result
         if (sd.ImplementationFactory != null)
         {
            return true;
         }

         //check if ImplementationInstance inherits from IInitializeableService
         if (sd.ImplementationInstance != null)
         {
            if (typeof(IAutoInitialize).IsAssignableFrom(sd.ImplementationInstance.GetType()))
            {
               return true;
            }

            return false;
         }

         //check if ImplementationType inherits from IInitializeableService
         if (sd.ImplementationType != null)
         {
            if (typeof(IAutoInitialize).IsAssignableFrom(sd.ImplementationType))
            {
               return true;
            }

            return false;
         }

         return false;
      });

      var groupedByServiceType = hasImplementation
         .GroupBy(serviceDescriptor => serviceDescriptor.ServiceType);

      List<Type> serviceTypes = groupedByServiceType.Select(grouping => grouping.Key).ToList();

      foreach (Type serviceType in serviceTypes)
      {
         try
         {
            var _myServiceType1 = serviceType;
            builder.Services.Decorate(serviceType, (innerService, serviceProvider) =>
            {
               var _myServiceType2 = serviceType;
               if (innerService is IAutoInitialize initService)
               {
                  //ensure that we only call .AutoInitialize() once per object, first time it's requested
                  if (_initializedServiceTracker.TryAdd(innerService))
                  {
                     initService.AutoInitialize(serviceProvider, ct)._SyncWait();
                  }
               }

               return innerService;
            });
         }
         catch (DecorationException ex)
         {
            __.GetLogger()._EzError("error calling .AutoInitialzie() on  decorated service. ", serviceType, ex);
         }
      }
   }

}

/// <summary>
/// mark a DI services and it will be auto-registered for use.  It will have lifetime Singleton, 
/// </summary>
public interface ISingletonService
{
}

/// <summary>
/// mark a DI services and it will be auto-registered for use.  It will have lifetime Transient, 
/// </summary>
public interface ITransientService
{
}

/// <summary>
/// mark a DI services and it will be auto-registered for use.  It will have lifetime Scoped, 
/// </summary>
public interface IScopedService
{
}

/// <summary>
/// use to signal that when your service is created, it should be initialized via a call to .AutoInitialize()
/// <para>note:  already created services returned by serviceDescriptor.ImplementationInstance will not have AutoInitialize() called.</para>
/// <para>also note:  "open generics" services  can not implement a DI decorator interface like IAutoInitialize. Open generic types are not supported for decoration because generic factories are not supported in C# DI. You must do a workaround (for example, init via the ctor instead of IAutoInitialize)</para>
/// </summary>
public interface IAutoInitialize
{
   ValueTask AutoInitialize(IServiceProvider services, CancellationToken ct);
}


/// <summary>
///  helper  DI service for letting LoLo automatically get a reference to services (__.Services)
/// </summary>
public class LoLoRunner(IServiceProvider _services) : IHostedLifecycleService, IAutoInitialize
{
   public async Task StartAsync(CancellationToken cancellationToken)
   {
      //  _logger._EzTrace("in");
      //  __.Services = _services;
   }

   public async Task StopAsync(CancellationToken cancellationToken)
   {
      //  _logger._EzTrace("in");
   }

   public async Task StartingAsync(CancellationToken cancellationToken)
   {
      //  _logger._EzTrace("in");
      // __.Services = _services;
   }

   public async Task StartedAsync(CancellationToken cancellationToken)
   {
      // _logger._EzTrace("in");
   }

   public async Task StoppingAsync(CancellationToken cancellationToken)
   {
   }

   public async Task StoppedAsync(CancellationToken cancellationToken)
   {
      //   _logger._EzTrace("in");
   }

   public async ValueTask AutoInitialize(IServiceProvider services, CancellationToken ct)
   {
      if (__.Services is null)
      {
         __.Services = _services;
      }
   }
}
