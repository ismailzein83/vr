using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace CDRComparison.Data
{
    public interface IPartialMatchCDRDataManager : IDataManager, IBaseCDRDataManager, IBulkApplyDataManager<PartialMatchCDR>
    {
        void ApplyPartialMatchCDRsToDB(object preparedNumberProfiles);
        void CreatePartialMatchCDRTempTable();
        Vanrise.Entities.BigResult<PartialMatchCDR> GetFilteredPartialMatchCDRs(Vanrise.Entities.DataRetrievalInput<PartialMatchCDRQuery> input, out decimal systemDurationInSeconds, out decimal partnerDurationInSeconds, out decimal differenceDurationInSeconds);
        void DeletePartialMatchTable();
        int GetPartialMatchCDRsCount();
        decimal GetDurationOfPartialMatchCDRs(bool isPartner);
        decimal GetTotalDurationDifferenceOfPartialMatchCDRs(string tableKey);
    }
}
