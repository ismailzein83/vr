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
        List<NewOrders> GetNewOrders();
        List<ActiveServices> GetActiveServices();
    }
}
