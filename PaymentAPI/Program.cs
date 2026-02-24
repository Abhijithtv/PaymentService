using PaymentAPI.Services;
using Serilog;

namespace PaymentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<ILogService, LogService>();

            if (builder.Configuration.GetValue<bool>("UseSerilog"))
            {
                builder.Host.UseSerilog();
            }
            var app = builder.Build();

            /* // Configure the HTTP request pipeline.
             if (app.Environment.IsDevelopment())
             {
                 app.UseSwagger();
                 app.UseSwaggerUI();
             }*/

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
