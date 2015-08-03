using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class NumberProfileDataManager : BaseMySQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }






        public void ApplyNumberProfilesToDB(object preparedNumberProfiles)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<NumberProfile> GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(NumberProfile record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }
    }
}
