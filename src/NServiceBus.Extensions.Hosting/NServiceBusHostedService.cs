namespace NServiceBus.Extensions.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    class NServiceBusHostedService(EndpointStarter endpointStarter) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken = default)
            => endpoint = await endpointStarter.GetOrStart(cancellationToken).ConfigureAwait(false);

        public Task StopAsync(CancellationToken cancellationToken = default) =>
            endpoint?.Stop(cancellationToken) ?? Task.CompletedTask;

        IEndpointInstance endpoint;
    }
}