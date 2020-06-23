namespace NServiceBus.Extensions.Hosting
{
    using System.Threading.Tasks;
    using ObjectBuilder;

    class EndpointInstanceStarter : IEndpointInstanceStarter, IStoppableEndpoint
    {
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

        Task IStoppableEndpoint.Stop()
        {
            return endpoint.Stop();
        }

        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        readonly IBuilder builder;

        IEndpointInstance endpoint;
    }
}