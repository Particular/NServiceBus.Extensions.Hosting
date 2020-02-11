namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using Lamar;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
                var containerSettings = endpointConfiguration.UseContainer<ServiceRegistry>(new LamarServiceProviderFactory());
                containerSettings.ConfigureContainer(registry =>
                {
                    registry.Policies.SetAllProperties(setterConvention => setterConvention.WithAnyTypeFromNamespace("NServiceBus.AcceptanceTests"));
                    registry.Policies.FillAllPropertiesOfType<IMessageCreator>();
                });

                configurationBuilderCustomization(endpointConfiguration);
            });
        }
    }
}