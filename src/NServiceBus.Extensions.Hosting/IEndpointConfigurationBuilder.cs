namespace NServiceBus.Extensions.Hosting
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the ability to influence the endpoint configuration at the time when dependency injection is available.
    /// </summary>
    public interface IEndpointConfigurationBuilder
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<EndpointConfiguration> Build();
    }
}