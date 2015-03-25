using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BI.Business;
using TOne.BI.Entities;

namespace TOne.Web.Controllers
{
    public class BIController : ApiController
    {
        public System.Data.DataTable GetProfit(DateTime fromDate, DateTime toDate)
        {
            SalesManager manager = new SalesManager();
            return manager.GetProfit(fromDate, toDate);
        }

        public IEnumerable<ProfitInfo> GetProfit(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            SalesManager manager = new SalesManager();
            return manager.GetProfit(timeDimensionType, fromDate, toDate);
        }
    }
}
