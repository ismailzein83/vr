using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IMissingCDRDataManager : IDataManager, IBaseCDRDataManager, IBulkApplyDataManager<MissingCDR>
    {
        void ApplyMissingCDRsToDB(object preparedNumberProfiles);
        void CreateMissingCDRTempTable();
        Vanrise.Entities.BigResult<MissingCDR> GetFilteredMissingCDRs(Vanrise.Entities.DataRetrievalInput<MissingCDRQuery> input, out decimal durationInSeconds);
        void DeleteMissingCDRTable();
        int GetMissingCDRsCount(bool isPartnerCDRs);
        decimal GetDurationOfMissingCDRs(bool? isPartner);
    }
}
