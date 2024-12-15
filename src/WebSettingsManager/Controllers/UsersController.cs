using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;

namespace WebSettingsManager.Controllers
{
    /// <summary>
    /// Контроллер пользователей с версионируемой конфигурацией текста
    /// </summary>
    [ApiController]
    [Route("/api/v1/users")]
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _dbContext.Users
                .ToListAsync();
            return Ok(users);
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
            if (existingUser == null)
                return NotFound();
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
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (existingUser == null)
                return NotFound();
            existingUser.Username = user.Username;
            existingUser.Name = user.Name;
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Удалить существующего пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId:long}", Name = "DeleteExistingUserById")]
        public async Task<IActionResult> DeleteUser([FromRoute] UInt64 userId)
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (existingUser == null)
                return NotFound();
            _dbContext.Users.Remove(existingUser);
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingUser);
        }

        /// <summary>
        /// Получить список конфигураций, связанных с пользователем
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="configurationFilterOptions"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}/configurations", Name = "GetConfigurationsForSpecificUser")]
        public async Task<IActionResult> GetConfigurationsForSpecificUser([FromRoute] UInt64 userId, [FromQuery] ConfigurationFilterOptions configurationFilterOptions)
        {
            var iQueryableConfigurations = _dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationOptions)
                .Where(x => x.UserId == userId);

            FilterItems(ref iQueryableConfigurations, configurationFilterOptions);

            var configurations = await iQueryableConfigurations
                .ToListAsync();
            return Ok(configurations);

            void FilterItems(ref IQueryable<UserTextConfiguration_Db> items, ConfigurationFilterOptions filterOptions)
            {
                if (filterOptions.CreationDateTimeOlderThanOrEqual != null)
                    items = items.Where(c => c.TextConfigurationActualState.CreationDateTime >= filterOptions.CreationDateTimeOlderThanOrEqual);
                if (filterOptions.CreationDateTimeEarlierThanOrEqual != null)
                    items = items.Where(c => c.TextConfigurationActualState.CreationDateTime <= filterOptions.CreationDateTimeEarlierThanOrEqual);
                if (filterOptions.ConfigurationNameTemplate != null)
                    items = items.Where(c => Regex.IsMatch(c.ConfigurationName, filterOptions.ConfigurationNameTemplate.Replace("*", ".*")));
            }
        }

        /// <summary>
        /// Получить конкретную конфигурацию для конкретного пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}/configurations/{confId:long}", Name = "GetConfigurationForUser")]
        public async Task<IActionResult> GetSpecificConfigurationForSpecificUser([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            var configuration = await _dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(c => c.TextConfigurationOptions)
                .Include(ss => ss.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationSavedState)
                        .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (configuration == null)
                return NotFound();
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
            var newConfiguration = new UserTextConfiguration_Db(userId, configurationData);
            _dbContext.UserTextConfigurations.Add(newConfiguration);

            await _dbContext.Instance.SaveChangesAsync();
            return Ok(newConfiguration);
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
            var existingConfiguration = await _dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(c => c.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();

            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = configurationData.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = configurationData.TextConfigurationOptions.FontName;
            if (_dbContext.Instance.ChangeTracker.HasChanges())
                existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;

            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingConfiguration);
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
            var existingConfiguration = await _dbContext.UserTextConfigurations
               .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();
            var removedConfiguration = _dbContext.UserTextConfigurations
                .Remove(existingConfiguration);

            await _dbContext.Instance.SaveChangesAsync();
            return Ok(removedConfiguration.Entity);
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
            var existingConfiguration = await _dbContext.UserTextConfigurations
               .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();
            var configurationSavedStates = await _dbContext.TextConfigurationSavedStates
                .Where(ss => ss.UserTextConfigurationId == confId)
                .Include(ss => ss.TextConfigurationOptions)
                .ToListAsync();
            return Ok(configurationSavedStates);
        }

        /// <summary>
        /// Получить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}/configurations/{confId:long}/saved-states/{stateId:long}", Name = "GetSpecificSavedStateForExistingUserConfiguration")]
        public async Task<IActionResult> GetSpecificSavedStateForConfiguration([FromRoute] UInt64 userId, [FromRoute] UInt64 confId, [FromRoute] UInt64 stateId)
        {
            var existingConfiguration = await _dbContext.UserTextConfigurations
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();
            var savedState = await _dbContext.TextConfigurationSavedStates
                .FirstOrDefaultAsync(ss => ss.Id == stateId && ss.UserTextConfigurationId == confId);
            if (savedState == null)
                return NotFound();
            return Ok(savedState);
        }

        /// <summary>
        /// Удалить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpDelete("{userId:long}/configurations/{confId:long}/saved-states/{stateId:long}", Name = "DeleteSpecificSavedStateForExistingUserConfiguration")]
        public async Task<IActionResult> DeleteSpecificSavedStateForConfiguration([FromRoute] UInt64 userId, [FromRoute] UInt64 confId, [FromRoute] UInt64 stateId)
        {
            var existingConfiguration = await _dbContext.UserTextConfigurations
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();
            var savedState = await _dbContext.TextConfigurationSavedStates
                .FirstOrDefaultAsync(ss => ss.Id == stateId && ss.UserTextConfigurationId == confId);
            if (savedState == null)
                return NotFound();
            var removedState = _dbContext.TextConfigurationSavedStates
                .Remove(savedState);
            return Ok(removedState.Entity);
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
            var existingConfiguration = await _dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();
            var saveDateTime = DateTime.Now;
            var newSavedState = new TextConfigurationSavedState_Db(existingConfiguration.TextConfigurationActualState, saveDateTime);
            var addedSavedState = _dbContext.TextConfigurationSavedStates.Add(newSavedState);
            existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState = addedSavedState.Entity;
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(addedSavedState.Entity);
        }

        /// <summary>
        /// Восстановить определённое сохраненное состояние конфигураиции пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}/configurations/{confId:long}/restore-last-saved-state", Name = "RestoreUserConfigurationSpecificSavedState")]
        public async Task<IActionResult> RestoreConfigurationLastSavedState([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            var existingConfiguration = await _dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationSavedState)
                        .ThenInclude(ss => ss.TextConfigurationOptions)
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();
            if (existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState == null)
                return this.NotFound("No last saved state");

            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState.TextConfigurationOptions.FontName;
            if (_dbContext.Instance.ChangeTracker.HasChanges())
                existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingConfiguration.TextConfigurationActualState.TextConfigurationOptions);
        }

        /// <summary>
        /// Восстановить последнее сохраненное состояние конфигураиции пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}/configurations/{confId:long}/restore-saved-state", Name = "RestoreUserConfigurationLastSavedState")]
        public async Task<IActionResult> RestoreConfigurationLastSavedState([FromRoute] UInt64 userId, [FromRoute] UInt64 confId, [FromBody] UInt64 savedStateId)
        {
            var existingConfiguration = await _dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId);
            if (existingConfiguration == null)
                return NotFound();

            var savedStateToRestore = await _dbContext.TextConfigurationSavedStates
                .Include(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(ss => ss.Id == savedStateId && ss.UserTextConfigurationId == confId);
            if (savedStateToRestore == null)
                return NotFound();

            
            existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState = savedStateToRestore;
            existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = savedStateToRestore.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = savedStateToRestore.TextConfigurationOptions.FontName;
            
            await _dbContext.Instance.SaveChangesAsync();
            return Ok(existingConfiguration.TextConfigurationActualState.TextConfigurationOptions);
        }

        /// <summary>
        /// Опции фильтрации конфигураций пользователя
        /// </summary>
        public class ConfigurationFilterOptions
        {
            /// <summary>
            /// Шаблон поиска конфигурации по имени через * и наличие последовательностей
            /// </summary>
            public string? ConfigurationNameTemplate { get; set; } = null;

            /// <summary>
            /// Время создание должно быть раньше или совпадать со значением
            /// </summary>
            public DateTime? CreationDateTimeEarlierThanOrEqual { get; set; } = null;

            /// <summary>
            /// Время создание должно быть позже или совпадать со значением
            /// </summary>
            public DateTime? CreationDateTimeOlderThanOrEqual { get; set; } = null;
        }

#pragma warning disable CS1591
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
#pragma warning restore CS1591
    }
}
