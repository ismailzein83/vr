using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IDisputeCDRDataManager : IDataManager, IBaseCDRDataManager, IBulkApplyDataManager<DisputeCDR>
    {
        void ApplyDisputeCDRsToDB(object preparedNumberProfiles);
        void CreateDisputeCDRTempTable();
        Vanrise.Entities.BigResult<DisputeCDR> GetFilteredDisputeCDRs(Vanrise.Entities.DataRetrievalInput<DisputeCDRQuery> input);
        void DeleteDisputeCDRTable();
        int GetDisputeCDRsCount();
        decimal GetDurationOfDisputeCDRs(bool isPartner);
    }
}
