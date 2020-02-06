﻿namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using Autofac.Extensions.DependencyInjection;
    using Castle.Windsor.MsDependencyInjection;
    using Extensions.Hosting;
    using Lamar;
    using StructureMap;
    using Unity;
    using Unity.Microsoft.DependencyInjection;

    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
//                endpointConfiguration.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//                endpointConfiguration.UseServiceProviderFactory<ServiceRegistry>(new LamarServiceProviderFactory());
//                endpointConfiguration.UseServiceProviderFactory(new WindsorServiceProviderFactory());
//                endpointConfiguration.UseServiceProviderFactory<IUnityContainer>(new ServiceProviderFactory(null));
                endpointConfiguration.UseServiceProviderFactory(new StructureMapServiceProviderFactory(new Registry()));

                configurationBuilderCustomization(endpointConfiguration);
            });
        }
    }
}