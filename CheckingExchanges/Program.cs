
using CheckingExchanges.External;
using CheckingExchanges.Interfaces;

namespace CheckingExchanges
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register IHttpClientFactory
            builder.Services.AddHttpClient();

            // Register exchange clients
            builder.Services.AddSingleton<IExchangeApiClient, BinanceApiClient>();
            builder.Services.AddSingleton<IExchangeApiClient, KuCoinApiClient>();

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
