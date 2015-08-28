using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.CDRImport.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class NormalCDRDataManager : BaseMySQLDataManager, INormalCDRDataManager 
    {
        public NormalCDRDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }





        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRResultQuery> input)
        {
            throw new NotImplementedException();
        }
    }
}
