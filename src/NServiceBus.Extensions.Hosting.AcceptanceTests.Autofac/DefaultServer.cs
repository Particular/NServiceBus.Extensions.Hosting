namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting.Support;
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Registration;
    using Autofac.Extensions.DependencyInjection;
 
    public class DefaultServer : ExternallyManagedContainerServer
    {
        public override Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointCustomizationConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            return base.GetConfiguration(runDescriptor, endpointCustomizationConfiguration, endpointConfiguration =>
            {
                endpointConfiguration.UseContainer(new AutofacServiceProviderFactory(c => c.RegisterModule<AutofacPropertyInjectionModule>()));

                configurationBuilderCustomization(endpointConfiguration);
            });
        }
    }

    class AutofacPropertyInjectionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            registration.Activating += (sender, args) => args.Context.InjectProperties(args.Instance);
        }
    }
}