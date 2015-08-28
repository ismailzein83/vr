using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class NormalCDRManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRResultQuery> input)
        {
            INormalCDRDataManager manager = FraudDataManagerFactory.GetDataManager<INormalCDRDataManager>();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetNormalCDRs(input));
        }

    }
}
