using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceMessage;

namespace PublishService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddMassTransit(configure =>
            {
                configure.AddBus(serviceProvider =>
                {
                    return Bus.Factory.CreateUsingRabbitMq(factoryConfigurator
                        => factoryConfigurator.Host("rabbitmq://localhost", hostConfigurator =>
                        {
                            hostConfigurator.Username("guest");
                            hostConfigurator.Password("guest");
                        }));
                });
            });

            services.AddSingleton<IHostedService, MasstransitHostedService>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISendEndpointProvider endpointProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHealthChecks("/health", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                async Task RequestDelegate(HttpContext context)
                {
                    int value = 0;
                    int count = 0;

                    do
                    {
                        value += 1;
                        await context.Response.WriteAsync($"Message: {value} \n");
                        count += 1;

                        var endpoint = await endpointProvider.GetSendEndpoint(new Uri("rabbitmq://localhost/test-work-queue"));

                        await endpoint.Send<INotification>(new { ThreadId = value, Name = $"Message: {value}", CreatedAt = DateTime.UtcNow });
                    } while (count <= 100);
                }

                endpoints.MapGet("/", RequestDelegate);
            });
        }
    }

    public class MasstransitHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public MasstransitHostedService(IBusControl busControl) => _busControl = busControl;

        public async Task StartAsync(CancellationToken cancellationToken) => await _busControl.StartAsync(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken) => await _busControl.StopAsync(cancellationToken);
    }
}
