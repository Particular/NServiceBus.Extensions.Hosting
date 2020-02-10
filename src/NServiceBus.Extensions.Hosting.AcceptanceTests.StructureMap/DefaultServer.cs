namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using MessageInterfaces.MessageMapper.Reflection;
    using StructureMap;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
                endpointConfiguration.UseContainer(new StructureMapServiceProviderFactory(new StructureMapPropertyInjectionRegistry()));

                configurationBuilderCustomization(endpointConfiguration);
            });
        }
    }

    class StructureMapPropertyInjectionRegistry : Registry
    {
        public StructureMapPropertyInjectionRegistry()
        {
            Policies.SetAllProperties(setterConvention => setterConvention.WithAnyTypeFromNamespace("NServiceBus.AcceptanceTests"));
            Policies.FillAllPropertiesOfType<IMessageCreator>().Use<MessageMapper>();
        }
    }
}