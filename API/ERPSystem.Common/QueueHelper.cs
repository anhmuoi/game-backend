using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using ERPSystem.Common.Infrastructure;

namespace ERPSystem
{
    public class QueueHelper
    {
        /// <summary>
        /// Get connection factory information from environment variables first. If it has been not found then get from appsettings.json file.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ConnectionFactory
        GetConnectionFactory(IConfiguration configuration)
        {
            ConnectionFactory factory;
            Boolean enableSsl;
            var queueHost =
                Environment
                    .GetEnvironmentVariable(Common
                        .Infrastructure
                        .Constants
                        .Settings
                        .QueueEnvironmentConnectionSettingsHost);
            if (!string.IsNullOrEmpty(queueHost))
            {
                factory =
                    new ConnectionFactory {
                        HostName =
                            Environment
                                .GetEnvironmentVariable(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueEnvironmentConnectionSettingsHost),
                        VirtualHost =
                            Environment
                                .GetEnvironmentVariable(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueEnvironmentConnectionSettingsVirtualHost),
                        Port =
                            int
                                .Parse(Environment
                                    .GetEnvironmentVariable(Common
                                        .Infrastructure
                                        .Constants
                                        .Settings
                                        .QueueEnvironmentConnectionSettingsPort)),
                        UserName =
                            Environment
                                .GetEnvironmentVariable(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueEnvironmentConnectionSettingsUserName),
                        Password =
                            Environment
                                .GetEnvironmentVariable(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueEnvironmentConnectionSettingsPassword)
                    };
            }
            else
            {
                factory =
                    new ConnectionFactory {
                        HostName =
                            configuration
                                .GetValue<string>(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueConnectionSettingsHost),
                        VirtualHost =
                            configuration
                                .GetValue<string>(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueConnectionSettingsVirtualHost),
                        Port =
                            configuration
                                .GetValue<int>(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueConnectionSettingsPort),
                        UserName =
                            configuration
                                .GetValue<string>(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueConnectionSettingsUserName),
                        Password =
                            configuration
                                .GetValue<string>(Common
                                    .Infrastructure
                                    .Constants
                                    .Settings
                                    .QueueConnectionSettingsPassword)
                    };
            }

            return factory;
        }
    }
}
