using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IMissingCDRDataManager : IDataManager, IBulkApplyDataManager<MissingCDR>
    {
        void ApplyMissingCDRsToDB(object preparedNumberProfiles);

        IEnumerable<MissingCDR> GetMissingCDRs(bool isPartnerCDRs);

        int GetMissingCDRsCount();
    }
}
