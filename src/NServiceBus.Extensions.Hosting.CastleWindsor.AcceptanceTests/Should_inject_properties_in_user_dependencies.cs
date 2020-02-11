namespace NServiceBus.AcceptanceTests
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.MsDependencyInjection;
    using NServiceBus;
    using NServiceBus.AcceptanceTesting.Customization;
    using NServiceBus.AcceptanceTesting.Support;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.Configuration.AdvancedExtensibility;
    using NUnit.Framework;

    [TestFixture]
    public class When_injecting_custom_types
    {
        [Test]
        public async Task Should_allow_registration_via_native_api()
        {
            var context = await Scenario.Define<TestContext>()
                .WithEndpoint<TestEndpoint>(e => e.When(s => s.SendLocal(new MyMessage())))
                .Done(c => c.GotTheMessage)
                .Run();

            Assert.True(context.DependencyAvailable);
        }

        class TestContext : ScenarioContext
        {
            public bool DependencyAvailable { get; set; }
            public bool GotTheMessage { get; set; }
        }

        class TestEndpoint : EndpointConfigurationBuilder
        {
            public TestEndpoint()
            {
                EndpointSetup<CastleEndpoint>(c => c.GetSettings()
                .Set<Action<ContainerSettings<IWindsorContainer>>>(cs =>
                //windsor doesn't provide a config API via the service provider factory so we need to use our custom api
                cs.ConfigureContainer(w => w.Register(Component.For<MyDependency>()))));
            }

            class MyHandler : IHandleMessages<MyMessage>
            {
                public MyHandler(TestContext testContext, MyDependency myDependency)
                {
                    this.testContext = testContext;
                    this.myDependency = myDependency;
                }

                public Task Handle(MyMessage message, IMessageHandlerContext context)
                {
                    testContext.DependencyAvailable = myDependency != null;
                    testContext.GotTheMessage = true;
                    return Task.CompletedTask;
                }

                readonly TestContext testContext;
                readonly MyDependency myDependency;
            }
        }

        class MyDependency
        {
        }

        class MyMessage : IMessage
        {
        }

        class CastleEndpoint : IEndpointSetupTemplate
        {
            public Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
            {
                var configuration = new EndpointConfiguration(runDescriptor.ScenarioContext.TestRunId.ToString("D"));

                configuration.TypesToIncludeInScan(endpointConfiguration.GetTypesScopedByTestClass());
                configuration.RegisterComponentsAndInheritanceHierarchy(runDescriptor);

                configuration.UseTransport<LearningTransport>();

                var containerSettings = configuration.UseContainer(new WindsorServiceProviderFactory());

                if (configuration.GetSettings().TryGet<Action<ContainerSettings<IWindsorContainer>>>(out var containerCustomizations))
                {
                    containerCustomizations(containerSettings);
                }

                configurationBuilderCustomization(configuration);

                return Task.FromResult(configuration);
            }
        }
    }
}