using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class ReportingDataManager : BaseMySQLDataManager, IReportingDataManager 
    {
        public ReportingDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public Vanrise.Entities.BigResult<CaseProductivity> GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityResultQuery> input)
        {
            throw new NotImplementedException();
        }


        public Vanrise.Entities.BigResult<BlockedLines> GetFilteredBlockedLines(Vanrise.Entities.DataRetrievalInput<BlockedLinesResultQuery> input)
        {
            throw new NotImplementedException();
        }
    }
}
