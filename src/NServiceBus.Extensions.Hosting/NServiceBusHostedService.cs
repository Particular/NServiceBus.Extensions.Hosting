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
            // closure allocation is ok here
            using (cancellationToken.Register(() => { endpointTcs.TrySetCanceled(); }, useSynchronizationContext: false))
            {
                try
                {
                    var endpoint = await startableEndpoint.Start(new ServiceProviderAdapter(serviceProvider))
                        .ConfigureAwait(false);
                    endpointTcs.TrySetResult(endpoint);
                }
                catch (Exception e)
                {
                    endpointTcs.TrySetException(e);
                    throw;
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // closure allocation is ok here
            using (cancellationToken.Register(() => { endpointTcs.TrySetCanceled(); }, useSynchronizationContext: false))
            {
                var endpoint = await endpointTcs.Task.ConfigureAwait(false);
                
                var stopCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                using (cancellationToken.Register(() => { stopCompletionSource.TrySetCanceled(); }, useSynchronizationContext: false))
                {
                    var resultTask = await Task.WhenAny(endpoint.Stop(), stopCompletionSource.Task).ConfigureAwait(false);
                    await resultTask.ConfigureAwait(false); // will either immediately complete or throw
                }
            }
        }

        public void UseServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        readonly TaskCompletionSource<IEndpointInstance> endpointTcs = new TaskCompletionSource<IEndpointInstance>(TaskCreationOptions.RunContinuationsAsynchronously);
        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        IServiceProvider serviceProvider;
    }
}