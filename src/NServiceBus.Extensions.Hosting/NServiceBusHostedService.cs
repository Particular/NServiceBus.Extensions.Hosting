namespace NServiceBus.Extensions.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    class NServiceBusHostedService(EndpointStarter endpointStarter) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken = default)
            => endpoint = await endpointStarter.GetOrStart(cancellationToken).ConfigureAwait(false);

#pragma warning disable CS0618 // Type or member is obsolete
        public Task StopAsync(CancellationToken cancellationToken = default) =>
            endpoint?.Stop(cancellationToken) ?? Task.CompletedTask;

        IEndpointInstance endpoint;
#pragma warning restore CS0618 // Type or member is obsolete
    }
}