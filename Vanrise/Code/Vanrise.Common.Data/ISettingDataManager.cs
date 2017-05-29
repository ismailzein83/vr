using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ISettingDataManager : IDataManager
    {
        IEnumerable<Setting> GetSettings();

        bool UpdateSetting(SettingToEdit setting);

        bool AreSettingsUpdated(ref object updateHandle);

        void GenerateScript(List<Setting> settings, Action<string, string> addEntityScript);
    }
}
