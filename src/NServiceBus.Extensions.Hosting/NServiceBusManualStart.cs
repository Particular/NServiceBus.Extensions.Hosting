namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading.Tasks;

    class NServiceBusManualStartAndStop : IStartAndStopThisEndpoint
    {
        public NServiceBusManualStartAndStop(IStartableEndpointWithExternallyManagedContainer startableEndpoint, IServiceProvider serviceProvider)
        {
            this.startableEndpoint = startableEndpoint;
            this.serviceProvider = serviceProvider;
        }
        
        public async Task Start()
        {
            endpoint = await startableEndpoint.Start(new ServiceProviderAdapter(serviceProvider))
                .ConfigureAwait(false);
        }

        public Task Stop()
        {
            return endpoint.Stop();
        }
        
        IEndpointInstance endpoint;
        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        readonly IServiceProvider serviceProvider;
    }

    /// <summary>
    /// Allows manually starting and stopping the endpoint.
    /// </summary>
    public interface IStartAndStopThisEndpoint
    {
        /// <summary>
        /// Starts the endpoint
        /// </summary>
        /// <returns></returns>
        Task Start();
        
        /// <summary>
        /// Stops the endpoint
        /// </summary>
        /// <returns></returns>
        Task Stop();
    }
}