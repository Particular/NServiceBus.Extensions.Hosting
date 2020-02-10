namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using Castle.Windsor.MsDependencyInjection;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
                // property injection working OOTB
                endpointConfiguration.UseContainer(new WindsorServiceProviderFactory());

                configurationBuilderCustomization(endpointConfiguration);
            });
        }
    }
}