using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    //public class UserWithVersioningTextConfigurations : IUserWithVersioningTextConfigurations
    //{        
    //    public IReadOnlyDictionary<string, IVersioningTextConfiguration> TextConfigurationsByNameDict => (IReadOnlyDictionary<string, IVersioningTextConfiguration>)_textConfigurationsByNameDict;
    //    private SortedDictionary<string, VersioningTextConfiguration> _textConfigurationsByNameDict = new();

    //    public string Username { get; }

    //    private Mutex _mutex = new();

    //    public UserWithVersioningTextConfigurations(string username)
    //    {
    //        this.Username = username;
    //    }

    //    public IVersioningTextConfiguration CreateVersioningConfiguration(string configurationName)
    //    {
    //        _mutex.WaitOne();
    //        try
    //        {
    //            if (_textConfigurationsByNameDict.ContainsKey(configurationName))
    //                throw new Exception("Configuration already exists with specified name: " + configurationName);
    //            var newVersioningConfiguration = new VersioningTextConfiguration(configurationName);
    //            _textConfigurationsByNameDict.Add(configurationName, newVersioningConfiguration);
    //            return newVersioningConfiguration;
    //        }
    //        finally
    //        {
    //            _mutex.ReleaseMutex();
    //        }
    //    }

    //    public IVersioningTextConfiguration AddVersioningConfiguration(IVersioningTextConfiguration versioningTextConfiguration)
    //    {
    //        _mutex.WaitOne();
    //        try
    //        {
    //            if (_textConfigurationsByNameDict.ContainsKey(versioningTextConfiguration.Name))
    //                throw new Exception("Configuration already exists with specified name: " + versioningTextConfiguration.Name);
    //            var newVersioningConfiguration = new VersioningTextConfiguration(versioningTextConfiguration);
    //            _textConfigurationsByNameDict.Add(versioningTextConfiguration.Name, newVersioningConfiguration);
    //            return newVersioningConfiguration;
    //        }
    //        finally
    //        { 
    //            _mutex.ReleaseMutex();
    //        }
    //    }

    //    public IVersioningTextConfiguration RemoveVersioningConfiguration(string versioningTextConfigurationName)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IVersioningTextConfiguration GetVersioningConfiguration(string versioningTextConfigurationName)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
