using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Controllers
{
#if !DEBUG
    [NonController]
#endif
    [Route("/-/info")]
    public class InfoController : Controller
    {
        private readonly IEnumerable<EndpointDataSource> _endpointSources;
        public InfoController(
            IEnumerable<EndpointDataSource> endpointSources
        )
        {
            _endpointSources = endpointSources;
        }

        [HttpGet("endpoints")]
        public Task<ActionResult> ListAllEndpoints()
        {
            var endpoints = _endpointSources
                .SelectMany(es => es.Endpoints)
                .OfType<RouteEndpoint>().ToList();
            var output = endpoints.Select(
                e =>
                {
                    var controller = e.Metadata
                        .OfType<ControllerActionDescriptor>()
                        .FirstOrDefault();
                    var action = controller != null
                        ? $"{controller.ControllerName}.{controller.ActionName}"
                        : null;
                    var controllerMethod = controller != null
                        ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}"
                        : null;
                    return new
                    {
                        Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                        Route = $"/{(e.RoutePattern.RawText ?? "UNDEFINED_RAW_TEXT").TrimStart('/')}",
                        Action = action,
                        ControllerMethod = controllerMethod
                    };
                }
            );

            return Task.FromResult<ActionResult>(Json(output));
        }

        /// <summary>
        /// Получить скрипт создания базы данных
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-db-creation-script", Name = "GetDatabaseCreationScript")]
        public string DbCreationScript([FromServices] IWebSettingsManagerDbContext dbContext)
        {
            return dbContext.Instance.Database.GenerateCreateScript();
        }
    }
}
