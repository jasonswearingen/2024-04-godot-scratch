using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

public static class AssemblyReflectionHelper
{

   /// <summary>
   /// 
   /// </summary>
   /// <param name="scanAssemblies">assemblies you want to scan for automapper and scrutor types.  default is everything: AppDomain.CurrentDomain.GetAssemblies()</param>
   /// <param name="scanIgnore">assemblies to not scan for DI types.   by default this is 'Microsoft.*' because ASP NetCore IHostedService internal registrations conflict.</param>
   /// <param name="matcher"></param>
   /// <returns></returns>
   public static List<Assembly> _FilterAssemblies(IEnumerable<Assembly>? scanAssemblies, IEnumerable<string>? scanIgnore)
   {
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
      var matcher = new Matcher();
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

      return targetAssemblies;
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
