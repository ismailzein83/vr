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
        public List<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {
            throw new NotImplementedException();
        }
    }
}
