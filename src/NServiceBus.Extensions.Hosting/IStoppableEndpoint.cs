namespace NServiceBus.Extensions.Hosting
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stop started <see cref="IEndpointInstance" />.
    /// </summary>
    public interface IStoppableEndpoint
    {
        /// <summary>
        /// Stop endpoint.
        /// </summary>
        /// <returns>
        /// </returns>
        Task Stop();
    }
}