namespace NServiceBus.Extensions.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    class NServiceBusHostedService : IHostedService
    {
        public NServiceBusHostedService(IEndpointInstanceStarter endpointInstanceStarter)
        {
            this.endpointInstanceStarter = endpointInstanceStarter;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            endpoint = await endpointInstanceStarter.Start().ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return endpoint.Stop();
        }

        IStoppableEndpoint endpoint;
        readonly IEndpointInstanceStarter endpointInstanceStarter;
    }
}