using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IPartialMatchCDRDataManager : IDataManager, IBulkApplyDataManager<PartialMatchCDR>
    {
        void ApplyPartialMatchCDRsToDB(object preparedNumberProfiles);
    }
}
