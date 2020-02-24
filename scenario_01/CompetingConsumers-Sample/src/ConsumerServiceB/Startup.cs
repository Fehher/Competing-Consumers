using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsumerServiceB
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddMassTransit(collectionConfigurator =>
            {
                collectionConfigurator.AddBus(serviceProvider =>
                {
                    return Bus.Factory.CreateUsingRabbitMq(factory =>
                    {
                        factory.Host("rabbitmq://localhost/", hostConfigurator =>
                        {
                            hostConfigurator.Username("guest");
                            hostConfigurator.Password("guest");
                        });

                        factory.ReceiveEndpoint("test-work-queue", endpoint =>
                        {
                            endpoint.PrefetchCount = 5;
                            endpoint.UseMessageRetry(r => r.Interval(2, 100));
                            endpoint.ConfigureConsumer<NotificationConsumer>(serviceProvider);
                        });
                    });
                });

                collectionConfigurator.AddConsumer<NotificationConsumer>();
            });

            services.AddSingleton<IHostedService, MasstransitHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            logger.LogInformation("In Development environment");

            app.UseHealthChecks("/health", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapGet("/", async context => await context.Response.WriteAsync("Consumer 02")));
        }
    }
}
