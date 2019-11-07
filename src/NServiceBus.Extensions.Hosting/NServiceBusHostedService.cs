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

        public Task<IEndpointInstance> Endpoint => endpointTcs.Task;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var endpoint = await startableEndpoint.Start(new ServiceProviderAdapter(serviceProvider))
                    .ConfigureAwait(false);
                endpointTcs.SetResult(endpoint);
            }
            catch (Exception e)
            {
                endpointTcs.SetException(e);
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var endpoint = await endpointTcs.Task.ConfigureAwait(false);
            await endpoint.Stop().ConfigureAwait(false);
        }

        public void UseServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        readonly TaskCompletionSource<IEndpointInstance> endpointTcs = new TaskCompletionSource<IEndpointInstance>();
        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        IServiceProvider serviceProvider;
    }
}