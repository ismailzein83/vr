using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class ReportingManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CaseProductivity> GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityResultQuery> input)
        {
            IReportingDataManager manager = FraudDataManagerFactory.GetDataManager<IReportingDataManager>();

            BigResult<CaseProductivity> casesProductivity = manager.GetFilteredCasesProductivity(input);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, casesProductivity);
        }


    }
}
