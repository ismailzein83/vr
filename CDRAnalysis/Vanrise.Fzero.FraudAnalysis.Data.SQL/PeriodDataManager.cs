using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class PeriodDataManager : BaseSQLDataManager, IPeriodDataManager
    {

        public PeriodDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<Period> GetPeriods()
        {
            return Enum.GetValues(typeof(PeriodEnum)).Cast<PeriodEnum>().Select(x => new Period { Id = (int)x, Name = x.ToString() }).ToList();
        }

    }
}
