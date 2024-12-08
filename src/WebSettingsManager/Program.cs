
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;

namespace WebSettingsManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //MvcOptions.Ena
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Подробнее о конфигурации Swagger/OpenAPI https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<WebSettingsManagerDbContext>(options => options.UseSqlite("Data Source=WebSettingsManager.db"));
            builder.Services.AddScoped<IWebSettingsManagerDbContext>(serviceProvider => serviceProvider.GetRequiredService<WebSettingsManagerDbContext>());
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                // Доступен на http://localhost:5197/swagger/index.html
                // Доступен на http://localhost:5197/swagger/v1/swagger.json
                // Доступен на http://localhost:5197/swagger/v1/swagger.yaml
                app.UseSwaggerUI(); 
            }

            app.UseAuthorization();


            app.MapControllers();

            
            //app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources, HttpContext context) =>
            //{
            //    var endpoints = endpointSources
            //        .SelectMany(es => es.Endpoints)
            //        .OfType<RouteEndpoint>();
            //    var output = endpoints.Select(
            //        e =>
            //        {
            //            var controller = e.Metadata
            //                .OfType<ControllerActionDescriptor>()
            //                .FirstOrDefault();
            //            var action = controller != null
            //                ? $"{controller.ControllerName}.{controller.ActionName}"
            //                : null;
            //            var controllerMethod = controller != null
            //                ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}"
            //                : null;
            //            return new
            //            {
            //                Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
            //                Route = $"/{e.RoutePattern.RawText.TrimStart('/')}",
            //                Action = action,
            //                ControllerMethod = controllerMethod
            //            };
            //        }
            //    );
            //    //var sb = new StringBuilder();
            //    //foreach (var ep in endpointSources)
            //    //{
            //    //    if (ep is CompositeEndpointDataSource compositeSource)
            //    //        foreach (var innerSource in compositeSource.DataSources)
            //    //            foreach(var innerSourceEP in innerSource.Endpoints)
            //    //                sb.AppendLine(innerSourceEP.Metadata..DisplayName);
            //    //    else
            //    //        sb.AppendLine(ep.ToString());

            //    //}
            //    return Microsoft.AspNetCore.Mvc.JsonResult.Json(output);
            //});

            app.Run();
        }
    }
}
