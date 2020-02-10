namespace NServiceBus.AcceptanceTests {
    using AcceptanceTesting.Support;

    public partial class TestSuiteConstraints
    {
        public bool SupportsDtc { get; } = false;
        public bool SupportsCrossQueueTransactions { get; } = false;
        public bool SupportsNativePubSub { get; } = true;
        public bool SupportsNativeDeferral { get; } = true;
        public bool SupportsOutbox { get; } = false;

        public IConfigureEndpointTestExecution CreateTransportConfiguration()
        {
            return new ConfigureEndpointAcceptanceTestingTransport(SupportsNativePubSub, SupportsNativeDeferral);
        }

        public IConfigureEndpointTestExecution CreatePersistenceConfiguration()
        {
            return new ConfigureEndpointLearningPersistence(true);
        }
    }
}