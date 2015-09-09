using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ISaleRateDataManager:IDataManager
    {
         List<Rate> GetSaleRates(string customerId, DateTime when);
    }
}
