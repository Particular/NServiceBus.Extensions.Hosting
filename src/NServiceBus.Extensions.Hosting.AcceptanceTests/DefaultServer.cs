namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
//                endpointConfiguration.UseContainer<ServiceRegistry>(new LamarServiceProviderFactory());
                //                endpointConfiguration.UseContainer(new StructureMapServiceProviderFactory(new Registry()));
                //endpointConfiguration.UseContainer(new DefaultServiceProviderFactory());

                configurationBuilderCustomization(endpointConfiguration);
            });
        }

        static Type IHandleMessagesType = typeof(IHandleMessages<>);
        public static bool IsMessageHandler(Type type)
        {
            if (type.IsAbstract || type.IsGenericTypeDefinition)
            {
                return false;
            }

            return type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType)
                .Select(@interface => @interface.GetGenericTypeDefinition())
                .Any(genericTypeDef => genericTypeDef == IHandleMessagesType);
        }
    }
}