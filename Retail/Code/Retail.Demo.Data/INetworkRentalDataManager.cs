using Retail.Demo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Data
{
    public interface IDemoDataManager : IDataManager
    {
        List<NewOrders> GetNewOrders(long accountId, DateTime fromDate, DateTime toDate);
        List<ActiveServices> GetActiveServices(long accountId, DateTime fromDate, DateTime toDate);
    }
}
