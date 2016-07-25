using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface ICDRDataManager : IDataManager, IBaseCDRDataManager, IBulkApplyDataManager<CDR>
    {
        void LoadCDRs(Action<CDR> onBatchReady);
        void CreateCDRTempTable();
        void ApplyCDRsToDB(object preparedCDRs);
        void DeleteCDRTable();
        Vanrise.Entities.BigResult<CDR> GetFilteredCDRs(Vanrise.Entities.DataRetrievalInput<CDRQuery> input);
        int GetCDRsCount(bool? isPartner);
        decimal GetDurationOfCDRs(bool? isPartner);
    }
}
