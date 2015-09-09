using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class ClassManager
    {

        public List<CallClass> GetClasses()
        {
            IClassDataManager dataManager = FraudDataManagerFactory.GetDataManager<IClassDataManager>();

            return dataManager.GetCallClasses();
        }

    }
}
