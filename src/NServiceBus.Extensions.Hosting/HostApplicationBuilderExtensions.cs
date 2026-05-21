namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using Extensions.Hosting;
    using Logging;
    using Microsoft.Extensions.Hosting;
    using Particular.Obsoletes;

    /// <summary>
    /// Extension methods to configure NServiceBus for the .NET hosted applications builder.
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the host application builder to start an NServiceBus endpoint.
        /// </summary>
        /// <returns></returns>
        [ObsoleteMetadata(
            Message = "NServiceBus provides built-in support for hosting endpoints within .NET hosted applications, so this extension method is no longer necessary",
            TreatAsErrorFromVersion = "5",
            RemoveInVersion = "6",
            ReplacementTypeOrMember = "IServiceCollection.AddNServiceBusEndpoint")]
        [Obsolete("NServiceBus provides built-in support for hosting endpoints within .NET hosted applications, so this extension method is no longer necessary. Use 'IServiceCollection.AddNServiceBusEndpoint' instead. Will be treated as an error from version 5.0.0. Will be removed in version 6.0.0.", false)]
        public static IHostApplicationBuilder UseNServiceBus(this IHostApplicationBuilder builder, EndpointConfiguration endpointConfiguration)
        {
            if (!builder.Properties.TryAdd(HostBuilderExtensionInUse, null))
            {
                throw new InvalidOperationException("UseNServiceBus can only be used once on the same host instance because subsequent calls would override each other. For multi-endpoint hosting scenarios consult our documentation page.");
            }

            var deferredLoggerFactory = new DeferredLoggerFactory();
            LogManager.UseFactory(deferredLoggerFactory);

            builder.Services.AddNServiceBus(endpointConfiguration, deferredLoggerFactory);
            return builder;
        }

        const string HostBuilderExtensionInUse = "NServiceBus.Extension.Hosting.UseNServiceBus";
    }
}