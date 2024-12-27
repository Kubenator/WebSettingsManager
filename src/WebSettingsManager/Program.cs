using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebSettingsManager.Hubs;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;

namespace WebSettingsManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Подробнее о  Swagger/OpenAPI https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WebSettingsManager API",
                    Description = "Описание Web API на базе ASP.NET Core",
                    Contact = new OpenApiContact
                    {
                        Name = "Почта mail.ru",
                        Email = "gulin-aleksey@inbox.ru"
                    }
                });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "WebSettingsManager.xml");
                options.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddSignalR();
            builder.Services.AddDbContext<WebSettingsManagerDbContext>(options => options.UseSqlite("Data Source=WebSettingsManager.db"));
            builder.Services.AddScoped<IWebSettingsManagerDbContext>(serviceProvider => serviceProvider.GetRequiredService<WebSettingsManagerDbContext>());
            builder.Services.AddSingleton<IUserWithVersioningTextConfigurationsRepository, UserWithVersioningTextConfigurationsRepository>();
            builder.Services.AddSingleton<IUserConfigurationChangingSubscriptionManager, UserConfigurationChangingSubscriptionManager>();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                // Доступен на http://localhost:5197/swagger/index.html
                // Доступен на http://localhost:5197/swagger/v1/swagger.json
                // Доступен на http://localhost:5197/swagger/v1/swagger.yaml
                app.UseSwaggerUI();
            }
            app.MapControllers();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapHub<UsersHub>("/users-hub");
            app.Run();
        }
    }
}
