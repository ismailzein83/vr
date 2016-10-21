using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.Postgres
{
    public class PostgresAnalyticDataProvider : AnalyticDataProvider
    {
        public override Guid ConfigId
        {
            get { return new Guid("10631F32-9116-4443-A73D-2D4B77111634"); ; }
        }

        public override IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext)
        {
            return new PostgresAnalyticDataManager(queryContext);
        }
    }
}
