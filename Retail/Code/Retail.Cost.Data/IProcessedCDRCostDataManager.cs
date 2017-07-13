using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Data
{
    public interface ICDRCostDataManager : IDataManager
    {
        List<CDRCost> GetCDRCostByCDPNs(CDRCostBatchRequest cdrCostBatchRequests);
    }
}
