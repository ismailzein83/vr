using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
    public class SaleEntityServiceManager
    {
        public long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }
    }
}
