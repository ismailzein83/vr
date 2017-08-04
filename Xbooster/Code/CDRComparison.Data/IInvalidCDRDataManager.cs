using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IInvalidCDRDataManager : IDataManager, IBaseCDRDataManager, IBulkApplyDataManager<InvalidCDR>
    {
        void CreateInvalidCDRTempTable();

        void ApplyInvalidCDRsToDB(object preparedInvalidCDRs);

        IEnumerable<InvalidCDR> GetAllInvalidCDRs(bool isPartnerCDRs);

        int GetInvalidCDRsCount(bool isPartnerCDRs);

        decimal GetInvalidCDRsDuration(bool isPartnerCDRs);
    }
}
