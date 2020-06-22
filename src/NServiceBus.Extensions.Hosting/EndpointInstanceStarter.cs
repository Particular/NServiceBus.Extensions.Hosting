namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading.Tasks;

    class EndpointInstanceStarter : IEndpointInstanceStarter, IStoppableEndpoint
    {
        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        readonly IServiceProvider serviceProvider;

        public EndpointInstanceStarter(
            IStartableEndpointWithExternallyManagedContainer startableEndpoint,
            IServiceProvider serviceProvider)
        {
            this.startableEndpoint = startableEndpoint;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task<IStoppableEndpoint> Start()
        {
            endpoint = await startableEndpoint.Start(new ServiceProviderAdapter(serviceProvider))
                .ConfigureAwait(false);
            return this;
        }

        Task IStoppableEndpoint.Stop() => endpoint.Stop();

        IEndpointInstance endpoint;
    }
}
