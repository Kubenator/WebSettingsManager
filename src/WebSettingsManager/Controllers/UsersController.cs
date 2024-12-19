using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;
using static WebSettingsManager.Models.UserWithVersioningTextConfigurationsRepository;

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
        private readonly IUserWithVersioningTextConfigurationsRepository _userRepository;
        //private readonly IWebSettingsManagerDbContext _dbContext;

        /// <summary>
        /// Создание экземпляра контроллера на основе контекста БД и логгера
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="logger"></param>
        public UsersController(/*IWebSettingsManagerDbContext dbContext, */IUserWithVersioningTextConfigurationsRepository userRepository, ILogger<UsersController> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            //_dbContext = dbContext;
        }

        /// <summary>
        /// Получить всех пользователей без вложенных сущностей
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = "GetAllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsers();
                return Ok(users);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("", Name = "PostNewUser")]
        public async Task<IActionResult> PostNewUser([FromBody] UserData user)
        {
            try
            {
                var addedUser = await _userRepository.AddUser(user);
                return Ok(addedUser);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Получить пользователя по id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId:long}", Name = "GetExistingUserById")]
        public async Task<IActionResult> GetUserById([FromRoute] UInt64 userId)
        {
            try
            {
                var existingUser = await _userRepository.GetUser(userId);
                return Ok(existingUser);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var updatedUser = await _userRepository.UpdateUser(userId, user);
                return Ok(updatedUser);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Удалить существующего пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId:long}", Name = "DeleteExistingUserById")]
        public async Task<IActionResult> DeleteUser([FromRoute] UInt64 userId)
        {
            try
            {
                var deletedUser = await _userRepository.RemoveUser(userId);
                return Ok(deletedUser);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var configurations = await _userRepository.GetUserConfigurations(userId, configurationFilterOptions);
                return Ok(configurations);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
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
            try
            {
                var configuration = await _userRepository.GetUserConfiguration(userId, confId);
                return Ok(configuration);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var configuration = await _userRepository.AddUserConfiguration(userId, configurationData);
                return Ok(configuration);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var configuration = await _userRepository.UpdateUserConfiguration(userId, confId, configurationData);
                return Ok(configuration);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var configuration = await _userRepository.RemoveUserConfiguration(userId, confId);
                return Ok(configuration);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var savedStates = await _userRepository.GetUserConfigurationSavedStates(userId, confId);
                return Ok(savedStates);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var savedState = await _userRepository.GetUserConfigurationSavedState(userId, confId, stateId);
                return Ok(savedState);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var removedState = await _userRepository.RemoveUserConfigurationSavedState(userId, confId, stateId);
                return Ok(removedState);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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
            try
            {
                var savedState = await _userRepository.SaveUserConfigurationState(userId, confId);
                return Ok(savedState);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Восстановить последнее сохраненное состояние конфигураиции пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}/configurations/{confId:long}/restore-last-saved-state", Name = "RestoreUserConfigurationLastSavedState")]
        public async Task<IActionResult> RestoreConfigurationLastSavedState([FromRoute] UInt64 userId, [FromRoute] UInt64 confId)
        {
            try
            {
                var configuration = await _userRepository.RestoreUserConfigurationLastSavedState(userId, confId);
                return Ok(configuration);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Восстановить последнее сохраненное состояние конфигураиции пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        [HttpPatch("{userId:long}/configurations/{confId:long}/restore-specific-saved-state", Name = "RestoreUserConfigurationSpecificSavedState")]
        public async Task<IActionResult> RestoreConfigurationSpecificSavedState([FromRoute] UInt64 userId, [FromRoute] UInt64 confId, [FromBody] UInt64 savedStateId)
        {
            try
            {
                var configuration = await _userRepository.RestoreUserConfigurationSavedState(userId, confId, savedStateId);
                return Ok(configuration);
            }
            catch (UserRepositoryItemNotFoundExceprion ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
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


    }
#pragma warning disable CS1591
    public class UserData
    {
        public string Username { get; set; } = "";

        public string Name { get; set; } = "";
    }
    public class TextConfigurationData
    {
        public string ConfigurationName { get; set; } = "";

        public TextConfigurationOptions TextConfigurationOptions { get; set; } = new TextConfigurationOptions();
    }
    public class TextConfigurationOptions
    {
        [JsonConstructor]
        public TextConfigurationOptions(string fontName = "Consolas", int fontSize = 12)
        {
            this.FontName = fontName;
            this.FontSize = fontSize;
        }
        public string FontName { get; }

        public int FontSize { get; }

    }
#pragma warning restore CS1591
}
