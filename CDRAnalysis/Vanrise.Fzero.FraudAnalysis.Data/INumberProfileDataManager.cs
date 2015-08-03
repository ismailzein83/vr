using System;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INumberProfileDataManager : IDataManager, IBulkApplyDataManager<NumberProfile>
    {
        void ApplyNumberProfilesToDB(object preparedNumberProfiles);

        BigResult<NumberProfile> GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileResultQuery> input);
    }
}
