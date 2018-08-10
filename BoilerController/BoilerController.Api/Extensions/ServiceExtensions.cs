using BoilerController.Api.Contracts;
using BoilerController.Api.Repository;
using BoilerController.Api.Services;
using BoilerController.Api.Services.Scheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace BoilerController.Api.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Initialize CORS policy for this project.
        /// </summary>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
            });
        }

        /// <summary>
        /// Initialize IIS integration.
        /// </summary>
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        /// <summary>
        /// Initialize logger service
        /// </summary>
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        /// <summary>
        /// Initalize SQL connection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config">Congig containing the connection string.</param>
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            //var connectionString = config["SQliteconnection:connectionString"];
            services.AddDbContext<RepositoryContext>(o => o.UseInMemoryDatabase("BoilerDB"));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureScheduler(this IServiceCollection services)
        {
            services.AddSingleton<IScheduleManager, ScheduleManager>();
        }

        public static void ConfigureDeviceService(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceService, DeviceService>();
        }
    }
}
