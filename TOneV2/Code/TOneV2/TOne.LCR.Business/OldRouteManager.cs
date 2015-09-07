using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
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
            List<RouteInfo> routeInfo = _datamanager.GetRoutes(showBlocksChar, isBlockChar, topValue, from, to, customerId, supplierId, code, zone);
            foreach (RouteInfo RI in routeInfo)
            {
                foreach (RouteOptionInfo SI in RI.SuppliersInfo)
                {
                    // FlaggedService flaggedServiceSymbol = _flaggedServiceDataManager.GetServiceFlag(SI.SupplierServicesFlag);
                    string supplierName = SI.SupplierID != null ? !string.IsNullOrEmpty(SI.SupplierID.NameSuffix) ? SI.SupplierID.ProfileName + "[" + SI.SupplierID.NameSuffix + "]" : SI.SupplierID.ProfileName : "";
                    string supplierZoneName = SI.SupplierZoneID != null ? SI.SupplierZoneID.Name : "";
                    SI.SupplierInfoString = supplierName + ";" + supplierZoneName + ";" + SI.SupplierActiveRate.ToString() + ";" + SI.SupplierServicesFlag.Symbol;
                }
            }
            return routeInfo;

        }
    }
}
