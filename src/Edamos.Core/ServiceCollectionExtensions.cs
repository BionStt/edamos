using Edamos.Core.Logs;
using Edamos.Core.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edamos.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEdamosDefault(this IServiceCollection services, IConfigurationRoot config)
        {
            services.AddLogging(logsBuilder => logsBuilder.AddEdamosLogs(config));
            services.AddEdamosUsers();
            
            return services;
        }
    }
}