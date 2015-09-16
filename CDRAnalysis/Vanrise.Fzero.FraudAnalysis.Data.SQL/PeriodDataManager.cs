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

            var enumerationType = typeof(PeriodEnum);
            List<Period> periods = new List<Period>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                periods.Add(new Period() { Id = value, Name = name });
            }

            return periods;


            //return Enum.GetValues(typeof(Period)).Cast<Period>().ToList<Period>();
        }

    }
}
