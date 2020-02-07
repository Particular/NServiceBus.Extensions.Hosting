namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Extensions.DependencyInjection;
    using ObjectBuilder;
    using ObjectBuilder.Common;

    class ContainerAdapter<TContainerBuilder> : IContainer
    {
        public ContainerAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory, ContainerSettings<TContainerBuilder> containerSettings)
        {
            this.containerSettings = containerSettings;
            serviceCollectionAdapter = new ServiceCollectionAdapter(containerSettings.ServiceCollection);
            serviceProviderAdapter = new Lazy<ServiceProviderAdapter>(() =>
            {
                locked = true;
                var containerBuilder = serviceProviderFactory.CreateBuilder(containerSettings.ServiceCollection);
                foreach (var customConfiguration in containerSettings.ContainerConfigurations)
                {
                    customConfiguration(containerBuilder);
                }
                var serviceProvider = serviceProviderFactory.CreateServiceProvider(containerBuilder);
                return new ServiceProviderAdapter(serviceProvider);
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public void Dispose()
        {
            serviceProviderAdapter.Value.Dispose();
        }

        public object Build(Type typeToBuild)
        {
            return serviceProviderAdapter.Value.Build(typeToBuild);
        }

        public IContainer BuildChildContainer()
        {
            return new ChildContainerAdapter(serviceProviderAdapter.Value.CreateChildBuilder());
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return serviceProviderAdapter.Value.BuildAll(typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            ThrowOnLockedContainer();

            serviceCollectionAdapter.ConfigureComponent(component, dependencyLifecycle);
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            ThrowOnLockedContainer();

            serviceCollectionAdapter.ConfigureComponent(component, dependencyLifecycle);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            ThrowOnLockedContainer();

            serviceCollectionAdapter.RegisterSingleton(lookupType, instance);
        }

        public bool HasComponent(Type componentType)
        {
            return serviceCollectionAdapter.HasComponent(componentType);
        }

        public void Release(object instance)
        {
            serviceProviderAdapter.Value.Release(instance);
        }

        void ThrowOnLockedContainer()
        {
            if (locked)
            {
                throw new InvalidOperationException("This operation is not valid anymore because the container has been locked.");
            }
        }

        readonly ContainerSettings<TContainerBuilder> containerSettings;
        readonly ServiceCollectionAdapter serviceCollectionAdapter;
        readonly Lazy<ServiceProviderAdapter> serviceProviderAdapter;

        bool locked;

        class ChildContainerAdapter : IContainer
        {
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
                throw new InvalidOperationException("Cannot build further child containers on a child container.");
            }

            public IEnumerable<object> BuildAll(Type typeToBuild)
            {
                return builder.BuildAll(typeToBuild);
            }

            public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
            {
                throw new InvalidOperationException("Cannot configure services on a child container.");
            }

            public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
            {
                throw new InvalidOperationException("Cannot configure services on a child container.");
            }

            public void RegisterSingleton(Type lookupType, object instance)
            {
                throw new InvalidOperationException("Cannot configure services on a child container.");
            }

            public bool HasComponent(Type componentType)
            {
                throw new InvalidOperationException();
            }

            public void Release(object instance)
            {
                builder.Release(instance);
            }

            readonly IBuilder builder;
        }
    }
}