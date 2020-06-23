namespace NServiceBus.Extensions.Hosting
{
    using ObjectBuilder;
    using System.Threading.Tasks;

    class EndpointInstanceStarter : IEndpointInstanceStarter, IStoppableEndpoint
    {
        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        readonly IBuilder builder;

        public EndpointInstanceStarter(
            IStartableEndpointWithExternallyManagedContainer startableEndpoint,
            IBuilder builder)
        {
            this.startableEndpoint = startableEndpoint;
            this.builder = builder;
        }

        /// <inheritdoc />
        public async Task<IStoppableEndpoint> Start()
        {
            endpoint = await startableEndpoint.Start(builder).ConfigureAwait(false);
            return this;
        }

        Task IStoppableEndpoint.Stop() => endpoint.Stop();

        IEndpointInstance endpoint;
    }
}
