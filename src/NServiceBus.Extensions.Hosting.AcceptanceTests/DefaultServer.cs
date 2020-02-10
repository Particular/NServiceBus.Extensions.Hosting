namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using Castle.Windsor.MsDependencyInjection;
    using Extensions.Hosting;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using StructureMap;
    using Unity;
    using Unity.Microsoft.DependencyInjection;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
//                endpointConfiguration.UseContainer<ServiceRegistry>(new LamarServiceProviderFactory());
                //                endpointConfiguration.UseServiceProviderFactory(new WindsorServiceProviderFactory());
                //                endpointConfiguration.UseServiceProviderFactory<IUnityContainer>(new ServiceProviderFactory(null));
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