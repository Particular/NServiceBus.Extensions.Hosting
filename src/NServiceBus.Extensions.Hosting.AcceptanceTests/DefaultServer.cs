namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using Autofac.Extensions.DependencyInjection;
    using Extensions.Hosting;
    using Lamar;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
//                endpointConfiguration.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                endpointConfiguration.UseServiceProviderFactory<ServiceRegistry>(new LamarServiceProviderFactory());

                configurationBuilderCustomization(endpointConfiguration);
            });
        }
    }
}