﻿using Godot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotNot.Internal;

namespace test1.src.lib.DI;

/// <summary>
/// a wrapper over DI for godot use
/// </summary>
public class DiWrapper : IDisposable
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
    public bool IsDisposed { get; private set; } = false;

    protected IHost DiHost;
    public IServiceProvider serviceProvider
    {
        get
        {
            if (!IsInitialized)
            {
                throw new Exception("GlobalDiHost NOT initialized.");
            }

            if (IsDisposed)
            {
                throw new Exception("GlobalDiHost already disposed.");
            }
            if (DiHost is null || DiHost.Services is null)
            {
                throw new Exception("GlobalDiHost services is null.  Why?!?!");
            }
            return DiHost.Services;
        }
    }


    public virtual async Task Initialize(CancellationToken ct)
    {
        _GD.Print("DiHostBase.Initialize()", Colors.Magenta);
        if (IsInitialized)
        {
            throw new Exception("GlobalDiHost already initialized.");
        }

        if (IsDisposed)
        {
            throw new Exception("GlobalDiHost already disposed.");
        }

        IsInitialized = true;

        //hostBuilder workflow
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddJsonFile("appsettings.json", optional: false);

        //configure app specific services firstly
        ConfigureServices(builder.Services);

        await builder._NotNotEzSetup(ct,
           extraLoggerConfig: (lc) => { lc.WriteTo.Godot(); });



        //configure NotNot default services
        DiHost = builder.Build();


        var __ = NotNotLoLo.__;
        __.Services = serviceProvider;

    }


    public void Dispose()
    {
        IsDisposed = true;

        //if (serviceProvider is not null)
        //{
        //   if (serviceProvider is IDisposable disposable)
        //   {
        //      disposable.Dispose();
        //   }

        //   //detach and dispose all configured services
        //   var services = serviceProvider.GetServices<IDisposable>();
        //   foreach (var service in services)
        //   {
        //      if (service is Node node)
        //      {
        //         //node lifecycle is managed by the scene tree they are attached to
        //         continue;
        //      }
        //      try
        //      {
        //         service.Dispose();
        //      }
        //      catch (Exception ex)
        //      {
        //         GD.PrintErr($"Error disposing service: {ex.Message}");
        //      }
        //   }
        //}

        if (DiHost is not null)
        {
            DiHost.Dispose();
            DiHost = null;
        }

    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        _GD.Print("HOST CONFIGURING SERVICES");

        services.AddSingleton(this);
    }
}

//public partial class EzInjectStore : Node
//{
//   public Dictionary<string, object> diStore;

//   public override void _EnterTree()
//   {
//      base._EnterTree();
//      diStore = new();
//   }
//   public override void _ExitTree()
//   {
//      base._ExitTree();
//      diStore.Clear();
//   }

//   public static TNode FindTargetFromStore<TNode>(Node currentNode) where TNode : Node
//   {
//      var startingNode = currentNode;

//      while (currentNode is not null)
//      {
//        var ezInjectStore =  currentNode._FindChild<EzInjectStore>();
//         if (ezInjectStore is not null)
//         {
//            ezInjectStore.diStore
//         }

//         if (currentNode is TNode target)
//         {
//            return target;
//         }
//         currentNode = currentNode.GetParent();
//      }


//   }


//}