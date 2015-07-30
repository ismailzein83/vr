using System;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INumberProfileDataManager : IDataManager, IBulkApplyDataManager<NumberProfile>
    {
        void ApplyNumberProfilesToDB(object preparedNumberProfiles);

        List<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber);
    }
}
