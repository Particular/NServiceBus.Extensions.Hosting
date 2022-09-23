namespace NServiceBus.Extensions.Hosting.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class NServiceBusHostedServiceTests
    {
        [Test]
        public void When_stopping_without_starting_should_not_throw()
        {
            var hostedService = new NServiceBusHostedService(null, null, null, null, null);

            Assert.DoesNotThrowAsync(async () => await hostedService.StopAsync());
        }
    }
}