namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Extensions.DependencyInjection;
    using ObjectBuilder;
    using ObjectBuilder.Common;

    /// <summary>
    /// 
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        //TODO: Action<TContainerBuilder> param
        public static void UseServiceProviderFactory<TContainerBuilder>(this EndpointConfiguration configuration, 
            IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            IContainer containerAdapter = new ContainerAdapter<TContainerBuilder>(serviceProviderFactory);
            configuration.UseContainer(containerAdapter);
        }
    }

    class ContainerAdapter<TContainerBuilder> : IContainer
    {
        ServiceCollectionAdapter scAdapter;
        ServiceCollection collection = new ServiceCollection();

        Lazy<ServiceProviderAdapter> spAdapter;


        public ContainerAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            scAdapter = new ServiceCollectionAdapter(collection);
            spAdapter = new Lazy<ServiceProviderAdapter>(
                () =>
                {
                    var containerBuilder = serviceProviderFactory.CreateBuilder(collection);
                    var serviceProvider = serviceProviderFactory.CreateServiceProvider(containerBuilder);
                    return new ServiceProviderAdapter(serviceProvider);
                }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public void Dispose()
        {
            spAdapter.Value.Dispose();
        }

        public object Build(Type typeToBuild)
        {
            return spAdapter.Value.Build(typeToBuild);
        }

        public IContainer BuildChildContainer()
        {
            return new ChildContainerAdapter(spAdapter.Value.CreateChildBuilder());
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return spAdapter.Value.BuildAll(typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            scAdapter.ConfigureComponent(component, dependencyLifecycle);
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            scAdapter.ConfigureComponent(component, dependencyLifecycle);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            scAdapter.RegisterSingleton(lookupType, instance);
        }

        public bool HasComponent(Type componentType)
        {
            return scAdapter.HasComponent(componentType);
        }

        public void Release(object instance)
        {
            spAdapter.Value.Release(instance);
        }

        class ChildContainerAdapter : IContainer
        {
            readonly IBuilder builder;

            public ChildContainerAdapter(IBuilder childBuilder)
            {
                builder = childBuilder;
            }

            public void Dispose()
            {
                builder.Dispose();
            }

            public object Build(Type typeToBuild)
            {
                return builder.Build(typeToBuild);
            }

            public IContainer BuildChildContainer()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<object> BuildAll(Type typeToBuild)
            {
                return builder.BuildAll(typeToBuild);
            }

            public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
            {
                throw new NotImplementedException();
            }

            public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
            {
                throw new NotImplementedException();
            }

            public void RegisterSingleton(Type lookupType, object instance)
            {
                throw new NotImplementedException();
            }

            public bool HasComponent(Type componentType)
            {
                throw new NotImplementedException();
            }

            public void Release(object instance)
            {
                builder.Release(instance);
            }
        }
    }
}