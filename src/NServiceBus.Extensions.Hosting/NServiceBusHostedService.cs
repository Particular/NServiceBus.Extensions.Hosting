namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    class NServiceBusHostedService : IHostedService
    {
        public NServiceBusHostedService(IStartableEndpointWithExternallyManagedContainer startableEndpoint)
        {
            this.startableEndpoint = startableEndpoint;
        }

        public IEndpointInstance Endpoint { get; private set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Endpoint = await startableEndpoint.Start(new ServiceProviderAdapter(serviceProvider))
                .ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Endpoint.Stop();
        }

        public void Configure(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        IServiceProvider serviceProvider;
    }
}