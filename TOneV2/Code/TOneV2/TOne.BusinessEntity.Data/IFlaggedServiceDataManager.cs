using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IFlaggedServiceDataManager : IDataManager
    {
        Dictionary<short, FlaggedService> GetServiceFlags();

        FlaggedService GetServiceFlag(short id);
    }
}
