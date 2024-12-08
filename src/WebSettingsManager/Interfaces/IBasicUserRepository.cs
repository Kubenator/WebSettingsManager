
namespace WebSettingsManager.Interfaces
{
    public interface IBasicUserRepository
    {
        public Task<IReadOnlyList<IUser>> GetAllUsers();
        public Task<IUser> AddUser(string username);
        public Task<IUser> RemoveUser(string username);
    }
}
