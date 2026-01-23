namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using Configuration.AdvancedExtensibility;
    using Extensions.Hosting;
    using Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extension methods to configure NServiceBus for the .NET hosted applications builder.
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the host application builder to start an NServiceBus endpoint.
        /// </summary>
        /// <returns></returns>
        public static IHostApplicationBuilder UseNServiceBus(this IHostApplicationBuilder builder, EndpointConfiguration endpointConfiguration)
        {
            if (!builder.Properties.TryAdd(HostBuilderExtensionInUse, null))
            {
                throw new InvalidOperationException("UseNServiceBus can only be used once on the same host instance because subsequent calls would override each other. For multi-endpoint hosting scenarios consult our documentation page.");
            }

            var auditDisable = builder.Configuration.GetValue<bool?>("NSERVICEBUS:AUDIT:DISABLE") ?? false;
            var auditAddress = builder.Configuration.GetValue<string>("NSERVICEBUS:AUDIT:ADDRESS");

            if (auditDisable)
            {
                // NServiceBus.Extensions.Hosting is unfortunately not a friend assembly... so cannot access types used for configuration.
                endpointConfiguration.GetSettings().Set("NServiceBus.AuditConfigReader.Result", null);
            }
            else if (!string.IsNullOrWhiteSpace(auditAddress))
            {
                endpointConfiguration.AuditProcessedMessagesTo(auditAddress);
            }

            var deferredLoggerFactory = new DeferredLoggerFactory();
            LogManager.UseFactory(deferredLoggerFactory);

            builder.Services.AddNServiceBus(endpointConfiguration, deferredLoggerFactory);
            return builder;
        }

        const string HostBuilderExtensionInUse = "NServiceBus.Extension.Hosting.UseNServiceBus";
    }
}