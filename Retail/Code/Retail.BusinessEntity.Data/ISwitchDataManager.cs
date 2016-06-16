using Retail.BusinessEntity.Entities;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        IEnumerable<Switch> GetSwitches();
        bool AreSwitchUpdated(ref object updateHandle);
    }
}
