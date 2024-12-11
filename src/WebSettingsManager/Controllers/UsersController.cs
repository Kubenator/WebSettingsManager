using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;

namespace WebSettingsManager.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UsersController : Controller
    {

        private readonly ILogger<UsersController> _logger;
        private readonly IWebSettingsManagerDbContext _dbContext;

        public UsersController(IWebSettingsManagerDbContext dbContext, ILogger<UsersController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "Users")]
        public IEnumerable<User> Get()
        {
            return _dbContext.Users.ToList();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
        }

        [HttpPut(Name = "PutUser")]
        public User Put()
        { 
            var user = new User() { Username = "Alex_" + new Random().Next(100), Name = "Alex"};
            var addedUser = _dbContext.Users.Add(user);
            _dbContext.Instance.SaveChanges();
            return addedUser.Entity;
        }

        [HttpGet("Dbb", Name = "DbContext")]
        public string DbCreationScript()
        {
            return _dbContext.Instance.Database.GenerateCreateScript();
        }


    }
}
