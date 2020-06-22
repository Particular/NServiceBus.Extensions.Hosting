namespace NServiceBus.Extensions.Hosting
{
    using System.Threading.Tasks;

    /// <summary>
    /// Start <see cref="IEndpointInstance" /> instances.
    /// </summary>
    public interface IEndpointInstanceStarter
    {
        /// <summary>
        /// Start endpoint instance.
        /// </summary>
        /// <returns></returns>
        Task<IStoppableEndpoint> Start();
    }
}