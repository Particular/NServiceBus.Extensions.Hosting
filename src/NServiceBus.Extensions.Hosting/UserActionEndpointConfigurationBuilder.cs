namespace NServiceBus
{
    using System;
    using System.Threading.Tasks;
    using Extensions.Hosting;
    using Microsoft.Extensions.Hosting;

    sealed class UserActionEndpointConfigurationBuilder : IEndpointConfigurationBuilder
    {
        public UserActionEndpointConfigurationBuilder(Func<HostBuilderContext, EndpointConfiguration> endpointConfigurationBuilder, HostBuilderContext hostBuilderContext)
        {
            this.hostBuilderContext = hostBuilderContext;
            this.endpointConfigurationBuilder = endpointConfigurationBuilder;
        }

        public Task<EndpointConfiguration> Build()
        {
            var endpointConfiguration = endpointConfigurationBuilder(hostBuilderContext);
            return Task.FromResult(endpointConfiguration);
        }


        readonly HostBuilderContext hostBuilderContext;
        readonly Func<HostBuilderContext, EndpointConfiguration> endpointConfigurationBuilder;
    }
}