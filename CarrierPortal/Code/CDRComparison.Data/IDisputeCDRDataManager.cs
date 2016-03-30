using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IDisputeCDRDataManager : IDataManager, IBulkApplyDataManager<DisputeCDR>
    {
        void ApplyDisputeCDRsToDB(object preparedNumberProfiles);

        IEnumerable<DisputeCDR> GetDisputeCDRs();

        int GetDisputeCDRsCount();
    }
}
