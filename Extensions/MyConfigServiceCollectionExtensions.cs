
using LexV2QnABotApp.Services;
using Microsoft.Extensions.Configuration;

namespace LexV2QnABotApp.Extensions
{
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddConfig(
             this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AWSSettings>(config.GetSection("AWSSettings"));
            return services;
        }

        public static IServiceCollection AddMyDependencyGroup(
             this IServiceCollection services)
        {
            services.AddScoped<ILexService, LexService>();
            return services;
        }
    }
}
