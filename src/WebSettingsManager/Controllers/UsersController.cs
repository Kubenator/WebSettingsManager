using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;

namespace WebSettingsManager.Controllers
{
    /// <summary>
    /// Контроллер пользователей с версионируемой конфигурацией текста
    /// </summary>
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UsersController : Controller
    {

        private readonly ILogger<UsersController> _logger;
        private readonly IWebSettingsManagerDbContext _dbContext;

        /// <summary>
        /// Создание экземпляра контроллера на основе контекста БД и логгера
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        public UsersController(IWebSettingsManagerDbContext dbContext, ILogger<UsersController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получить всех пользователей без вложенных сущностей
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = "GetAllUsers")]
        public IActionResult GetUsers()
        {
            return Ok(_dbContext.Users
                .ToList());
        }

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("", Name = "PostNewUser")]
        public async Task<IActionResult> PostNewUser([FromBody] UserData user)
        {
            var userToAdd = new User_Db() { Username = user.Username, Name = user.Name };
            var addedUser = _dbContext.Users.Add(userToAdd);
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(addedUser.Entity);
        }

        /// <summary>
        /// Получить пользователя по id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}", Name = "GetExistingUserById")]
        public async Task<IActionResult> GetUserById([FromRoute] UInt64 userId)
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            return Ok(existingUser);
        }

        /// <summary>
        /// Обновить существующего пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}", Name = "PatchExistingUserById")]
        public async Task<IActionResult> PatchUser([FromRoute] UInt64 userId, [FromBody] UserData user)
        {
            var existingUser = await _dbContext.Users.FirstAsync(x => x.Id == userId);
            existingUser.Username = user.Username;
            existingUser.Name = user.Name;
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Удалить существующего пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpDelete("{userId:long}", Name = "DeleteExistingUserById")]
        public async Task<IActionResult> DeleteUser([FromRoute] UInt64 userId, [FromBody] UserData user)
        {
            var existingUser = await _dbContext.Users.FirstAsync(x => x.Id == userId);
            existingUser.Username = user.Username;
            existingUser.Name = user.Name;
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Получить список конфигураций, связанных с пользователем
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}/configurations")]
        public async Task<IActionResult> GetConfigurationsForSpecificUser([FromRoute] UInt64 userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationActualState)
                .ThenInclude(c => c.TextConfigurationOptions)
                .FirstAsync(u => u.Id == userId);
            if (user == null)
                return new NotFoundResult();
            var configurations = user.TextConfigurations.ToList();
            return Ok(configurations);
        }

        /// <summary>
        /// Получить конкретную конфигурацию для конкретного пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}/configurations/{confId:long}")]
        public async Task<IActionResult> GetSpecificConfigurationForSpecificUser([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            var user = await _dbContext.Users

                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationActualState)
                .ThenInclude(c => c.TextConfigurationOptions)

                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationSavedStates)
                .ThenInclude(ss => ss.TextConfigurationOptions)

                .FirstAsync(u => u.Id == userId);
            var configuration = user.TextConfigurations
                .First(c => c.Id == confId);
            return Ok(configuration);
        }

        /// <summary>
        /// Создать новую конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="configurationData"></param>
        /// <returns></returns>
        [HttpPost("{userId:long}/configurations", Name = "PostNewUserConfiguration")]
        public async Task<IActionResult> PostNewConfiguration([FromRoute] UInt64 userId, [FromBody] TextConfigurationData configurationData)
        {
            var existingUser = await _dbContext.Users
                .Include(x => x.TextConfigurations)
                .FirstAsync(x => x.Id == userId);

            var newConfiguration = new UserTextConfiguration_Db(userId, configurationData);

            existingUser.TextConfigurations.Add(newConfiguration);
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Обновить конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="configurationData"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}/configurations/{confId:long}", Name = "PatchExistingUserConfiguration")]
        public async Task<IActionResult> PatchConfiguration([FromRoute] UInt64 userId, [FromRoute] UInt64 confId, [FromBody] TextConfigurationData configurationData)
        {
            var existingUser = await _dbContext.Users
                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationActualState)
                .ThenInclude(ss => ss.TextConfigurationOptions)
                .FirstAsync(x => x.Id == userId);
            var existingConfiguration = existingUser.TextConfigurations.First(x => x.Id == confId);

            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = configurationData.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = configurationData.TextConfigurationOptions.FontName;
            if (_dbContext.Instance.ChangeTracker.HasChanges())
                existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;

            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Удалить конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpDelete("{userId:long}/configurations/{confId:long}", Name = "DeleteExistingUserConfiguration")]
        public async Task<IActionResult> DeleteConfiguration([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            var existingUser = await _dbContext.Users.Include(u => u.TextConfigurations).FirstAsync(x => x.Id == userId);
            var existingConfiguration = existingUser.TextConfigurations.First(x => x.Id == confId);

            _dbContext.UserTextConfigurations.Remove(existingConfiguration);

            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Получить сохраненные состояния конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}/configurations/{confId:long}/saved-states", Name = "GetExistingUserConfigurationSavedStates")]
        public async Task<IActionResult> GetConfigurationSavedStates([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            var existingUser = await _dbContext.Users
                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationSavedStates)
                .ThenInclude(s => s.TextConfigurationOptions)

                .FirstAsync(x => x.Id == userId);

            var existingConfiguration = existingUser.TextConfigurations.First(x => x.Id == confId);

            return Ok(existingConfiguration.TextConfigurationSavedStates);
        }

        /// <summary>
        /// Сохранить состояние конфигураиции пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}/configurations/{confId:long}/save-state", Name = "SaveExistingUserConfigurationState")]
        public async Task<IActionResult> SaveConfigurationState([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            var existingUser = await _dbContext.Users

                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationActualState)
                .ThenInclude(s => s.TextConfigurationOptions)

                .Include(u => u.TextConfigurations)
                .ThenInclude(c => c.TextConfigurationSavedStates)
                .ThenInclude(s => s.TextConfigurationOptions)

                .FirstAsync(x => x.Id == userId);
            var existingConfiguration = existingUser.TextConfigurations.First(x => x.Id == confId);

            var saveDateTime = DateTime.Now;
            var newSavedState = new TextConfigurationSavedState_Db(existingConfiguration.TextConfigurationActualState, saveDateTime);
            
            
            var addedConfiguration = await _dbContext.TextConfigurationSavedStates.AddAsync(newSavedState);
            existingConfiguration.TextConfigurationSavedStates.Add(newSavedState);

            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Получить скрипт создания базы данных
        /// </summary>
        /// <returns></returns>
        [HttpGet("Dbb", Name = "GetDatabaseCreationScript")]
        public string DbCreationScript()
        {
            return _dbContext.Instance.Database.GenerateCreateScript();
        }

        public class UserData : IUser
        {
            public string Username { get; set; } = "";

            public string Name { get; set; } = "";
        }
        public class TextConfigurationData : ITextConfiguration
        {
            public string ConfigurationName { get; set; } = "";

            public TextConfigurationOptions TextConfigurationOptions { get; set; } = new TextConfigurationOptions();

            ITextConfigurationOptions ITextConfiguration.TextConfigurationOptions => TextConfigurationOptions;
        }
    }
}
