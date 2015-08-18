using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Data;
using TOne.LCR.Entities.Routing;

namespace TOne.LCR.Business
{
    public class OldRouteManager
    {
        private readonly IOldRoutingDataManager _datamanager;
        public OldRouteManager()
        {
            _datamanager = LCRDataManagerFactory.GetDataManager<IOldRoutingDataManager>();
        }
        public List<RouteInfo> GetRoutes(bool showBlocks, bool? isBlock, int topValue, int from, int to, string customerId, string supplierId, string code, string zone)
        {
            char showBlocksChar;
            char? isBlockChar;
            if (showBlocks) showBlocksChar = 'Y'; else showBlocksChar = 'N';
            if (isBlock != null) isBlockChar = 'N'; else isBlockChar = null;
            return _datamanager.GetRoutes(showBlocksChar, isBlockChar, topValue, from, to, customerId, supplierId, code, zone);
        }
    }
}
