using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class PredefinedDataManager : BaseMySQLDataManager, IPredefinedDataManager 
    {
        public PredefinedDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public List<CallClass> GetCallClasses()
        {
            throw new NotImplementedException();
        }

        public List<Period> GetPeriods()
        {
            throw new NotImplementedException();
        }

       
    }
}
