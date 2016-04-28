using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        void ApplySwitchesToDB(List<Switch> switches);
    }
}
