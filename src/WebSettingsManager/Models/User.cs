using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    public class User : IUser
    {
        public User(string username) 
        {
            this.Username = username;
        }
        public string Username { get; }
    }
}
