namespace NServiceBus.AcceptanceTests
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using AcceptanceTesting.Support;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Unity;
    using Unity.Microsoft.DependencyInjection;

    [TestFixture]
    public class Should_inject_properties_in_user_dependencies
    {
        [Test]
        public async Task Should_support_property_injection()
        {
            await Scenario.Define<TestContext>()
                .WithEndpoint<TestEndpoint>(e => e
                    .When(s => s.SendLocal(new TestMessage())))
                .Done(c => c.PropertyInjected)
                .Run();
        }

        class TestContext : ScenarioContext
        {
            public bool PropertyInjected { get; set; }
        }

        class TestEndpoint : EndpointConfigurationBuilder
        {
            public TestEndpoint()
            {
                EndpointSetup<EndpointTemplate>();
            }

            class TestHandler : IHandleMessages<TestMessage>
            {
                [Dependency] // property injection requires attributes defined by the user
                public TestContext TestContext { get; set; }

                public Task Handle(TestMessage message, IMessageHandlerContext context)
                {
                    TestContext.PropertyInjected = TestContext != null;
                    return Task.CompletedTask;
                }
            }
        }

        class TestMessage : IMessage
        {
        }
    }

    class EndpointTemplate : IEndpointSetupTemplate
    {
        public Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            var configuration = new EndpointConfiguration(runDescriptor.ScenarioContext.TestRunId.ToString("D"));
            configuration.UseTransport<LearningTransport>();

            var containerSettings = configuration.UseContainer<IUnityContainer>(new ServiceProviderFactory(null));
            containerSettings.ServiceCollection.AddSingleton(runDescriptor.ScenarioContext.GetType(), runDescriptor.ScenarioContext);

            configurationBuilderCustomization(configuration);
            return Task.FromResult(configuration);
        }
    }
}