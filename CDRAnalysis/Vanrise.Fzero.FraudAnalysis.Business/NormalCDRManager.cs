using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class NormalCDRManager
    {
        public IEnumerable<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {

            INormalCDRDataManager manager = FraudDataManagerFactory.GetDataManager<INormalCDRDataManager>();

            return manager.GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn);

        }

    }
}
