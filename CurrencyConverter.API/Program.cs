
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Core.Services;
using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;
using Polly;
using CurrencyConverter.API.Middlewares;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

namespace CurrencyConverter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.Seq("http://localhost:5341")
                            .CreateLogger();


            #region Extenstion Methods
            builder.Logging.AddOpenTelemetry();
            builder.Services.AddOpenTelemetryService();
            builder.Services.AddVersioning();
            #endregion

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ExchangeRateService>();
            builder.Services.AddScoped<IExchangeRateProvider, FrankfurterExchangeRateProvider>();

            builder.Services.AddHttpClient<IExchangeRateProvider, FrankfurterExchangeRateProvider>(c =>
            {
                c.DefaultRequestVersion = new Version(3, 0);
            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(5)))
              .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

             
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }

    public static class Extenstions
    {
        public static ILoggingBuilder AddOpenTelemetry(this ILoggingBuilder loggingBuilder)
        {
            return loggingBuilder.AddOpenTelemetry(options =>
            {
                options
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService("CurrencyConverter"))
                    .AddConsoleExporter();
            });
        }

        public static void AddOpenTelemetryService(this IServiceCollection services)
        {
             services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource.AddService("CurrencyConverter"))
                    .WithTracing(tracing => tracing
                            .AddAspNetCoreInstrumentation()
                            .AddConsoleExporter())
                    .WithMetrics(metrics => metrics
                        .AddAspNetCoreInstrumentation()
                        .AddConsoleExporter());
        }

        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
           return services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader()
                );
            });
        }
    }
}
