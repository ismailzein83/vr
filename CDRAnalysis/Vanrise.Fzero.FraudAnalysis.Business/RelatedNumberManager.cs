using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class RelatedNumberManager
    {
        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            IRelatedNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<IRelatedNumberDataManager>();
            return dataManager.GetRelatedNumbersByAccountNumber(accountNumber);
        }

    }
}
