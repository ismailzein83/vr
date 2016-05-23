using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ISettingDataManager : IDataManager
    {
        IEnumerable<Setting> GetSettings();

        bool UpdateSetting(Setting setting);

        bool AreSettingsUpdated(ref object updateHandle);
    }
}
