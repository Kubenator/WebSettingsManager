using System;
using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    public class VersioningTextConfiguration : IVersioningTextConfiguration
    {        
        public string Name { get; }

        public DateTime CreationDateTime { get; private set; }

        public DateTime ModificationDateTime { get; private set; }

        public DateTime SaveDateTime { get; private set; }

        public ITextConfigurationData TextConfigurationData => _textConfigurationData;
        private TextConfigurationData _textConfigurationData;

        public IReadOnlyDictionary<DateTime, ITextConfigurationData> VersionsByPointInTimeDict => (IReadOnlyDictionary<DateTime, ITextConfigurationData>)_versionsByPointInTimeDict;
        private SortedDictionary<DateTime, TextConfigurationData> _versionsByPointInTimeDict = new();

        public ICollection<DateTime> VersionsTimes => _versionsByPointInTimeDict.Keys.ToList();

        private Mutex _mutex = new Mutex();

        public VersioningTextConfiguration(string configurationName)
        {
            this.Name = configurationName;
            var initUtcDateTime = DateTime.UtcNow;
            this.CreationDateTime = initUtcDateTime;
            this.ModificationDateTime = initUtcDateTime;
            this.SaveDateTime = initUtcDateTime;
            _textConfigurationData = new TextConfigurationData();
            _versionsByPointInTimeDict.Add(initUtcDateTime, _textConfigurationData);
        }

        public VersioningTextConfiguration(string configurationName, ITextConfigurationData textConfigurationData)
        {
            this.Name = configurationName;
            var initUtcDateTime = DateTime.UtcNow;
            this.CreationDateTime = initUtcDateTime;
            this.ModificationDateTime = initUtcDateTime;
            this.SaveDateTime = initUtcDateTime;
            _textConfigurationData = new TextConfigurationData(textConfigurationData);
            _versionsByPointInTimeDict.Add(initUtcDateTime, _textConfigurationData);
        }
        public VersioningTextConfiguration(IVersioningTextConfiguration versioningTextConfiguration)
        { 
            this.Name = versioningTextConfiguration.Name;
            this.CreationDateTime = versioningTextConfiguration.CreationDateTime;
            this.ModificationDateTime = versioningTextConfiguration.ModificationDateTime;
            this.SaveDateTime = versioningTextConfiguration.SaveDateTime;
            _textConfigurationData = new TextConfigurationData(versioningTextConfiguration.TextConfigurationData);
            foreach (var state in versioningTextConfiguration.VersionsByPointInTimeDict)
                _versionsByPointInTimeDict.Add(state.Key, new TextConfigurationData(state.Value));
        }

        public bool RestoreLastVersion()
        {
            _mutex.WaitOne();
            try
            {
                if (!_versionsByPointInTimeDict.TryGetValue(SaveDateTime, out var lastSaveData))
                {
                    throw new Exception("No last saved state was found at " + SaveDateTime.ToString("s"));
                }
                var isChanged = !_textConfigurationData.Equals(lastSaveData);
                _textConfigurationData = lastSaveData;
                this.ModificationDateTime = SaveDateTime;
                return isChanged;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public bool RestoreVersion(DateTime dateTime)
        {
            _mutex.WaitOne();
            try
            {
                if (!_versionsByPointInTimeDict.TryGetValue(dateTime, out var savedState))
                {
                    throw new Exception("No saved state was found at " + dateTime.ToString("s"));
                }
                var isChanged = !_textConfigurationData.Equals(savedState);
                this.ModificationDateTime = dateTime;
                _textConfigurationData = savedState;
                return isChanged;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public bool SaveState()
        {
            _mutex.WaitOne();
            try
            {
                if (!_versionsByPointInTimeDict.TryGetValue(SaveDateTime, out var lastSaveData))
                {
                    throw new Exception("No last saved state was found at " + SaveDateTime.ToString("s"));
                }
                var saveDateTime = DateTime.UtcNow;
                _versionsByPointInTimeDict.Add(saveDateTime, _textConfigurationData);
                this.SaveDateTime = saveDateTime;
                return !_textConfigurationData.Equals(lastSaveData);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public bool UpdateCurrentState(ITextConfigurationData newState)
        {
            _mutex.WaitOne();
            try
            {
                bool isChanged = !_textConfigurationData.Equals(newState);
                if (isChanged)
                {
                    _textConfigurationData = new TextConfigurationData(newState);
                    this.ModificationDateTime = DateTime.UtcNow;
                }
                return isChanged;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public ITextConfigurationData GetVersion(DateTime dateTime)
        {
            _mutex.WaitOne();
            try
            {
                if (!_versionsByPointInTimeDict.TryGetValue(dateTime, out var lastSaveData))
                {
                    throw new Exception("No last saved state was found at " + dateTime.ToString("s"));
                }
                return lastSaveData;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public ITextConfigurationData GetLastVersion()
        {
            _mutex.WaitOne();
            try
            { 
                if (!_versionsByPointInTimeDict.TryGetValue(SaveDateTime, out var lastSaveData))
                {
                    throw new Exception("No last saved state was found at " + SaveDateTime.ToString("s"));
                }
                return lastSaveData;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public ITextConfigurationData RemoveVersion(DateTime dateTime)
        {
            _mutex.WaitOne();
            try
            {
                if (_versionsByPointInTimeDict.TryGetValue(dateTime, out var removedVersion))
                {
                    _versionsByPointInTimeDict.Remove(dateTime);
                    return removedVersion;
                }
                else
                    throw new Exception("No existing version to remove at " + dateTime.ToString("s"));
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public ITextConfigurationData RemoveLastVersion()
        {
            _mutex.WaitOne();
            try
            {
                if (_versionsByPointInTimeDict.TryGetValue(SaveDateTime, out var removedVersion))
                {
                    _versionsByPointInTimeDict.Remove(SaveDateTime);
                    return removedVersion;
                }
                else
                    throw new Exception("No existing version to remove at " + SaveDateTime.ToString("s"));
            }
            finally
            {
                _mutex.ReleaseMutex();
            }            
        }
    }
}
