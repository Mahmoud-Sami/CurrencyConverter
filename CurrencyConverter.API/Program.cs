
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Core.Services;
using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;
using Polly;

namespace CurrencyConverter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader()
                );
            });

            

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
