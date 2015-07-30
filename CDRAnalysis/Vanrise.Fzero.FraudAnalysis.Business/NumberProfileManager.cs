using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class NumberProfileManager
    {
        public IEnumerable<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {

            INumberProfileDataManager manager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

            return manager.GetNumberProfiles(fromRow, toRow, fromDate, toDate, subscriberNumber);

        }
    }
}
