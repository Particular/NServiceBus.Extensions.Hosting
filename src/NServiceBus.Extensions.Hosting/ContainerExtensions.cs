namespace NServiceBus.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using ObjectBuilder.Common;

    /// <summary>
    /// Extension methods to integrate containers support the Microsoft.Extensions.DependencyInjection model.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Use a custom dependency injection container implementing the Microsoft.Extensions.DependencyInjection model.
        /// The container lifetime will be managed by NServiceBus.
        /// Use <see cref="EndpointWithExternallyManagedContainer"/> to manage container lifecycle yourself.
        /// </summary>
        /// <param name="configuration">The endpoint configuration.</param>
        /// <param name="serviceProviderFactory">The <see cref="IServiceProviderFactory{TContainerBuilder}"/> of the container to be used.</param>
        public static void UseContainer<TContainerBuilder>(this EndpointConfiguration configuration, 
            IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            IContainer containerAdapter = new ContainerAdapter<TContainerBuilder>(serviceProviderFactory);
            configuration.UseContainer(containerAdapter);
        }
    }
}