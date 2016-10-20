using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class ClearTempTablesDBSyncDataManager : BaseSQLDataManager
    {
        public ClearTempTablesDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        public void ClearTempTables()
        {
            ExecuteNonQueryText("delete from [TOneWhS_SPL].[SupplierPriceListTemplate] "
            + " delete from [TOneWhS_BE].[SPL_SupplierCode_Changed] "
            + " delete from [TOneWhS_BE].[SPL_SupplierCode_New] "
            + " delete from [TOneWhS_BE].[SPL_SupplierRate_Changed] "
            + " delete from [TOneWhS_BE].[SPL_SupplierRate_New] "
            + " delete from [TOneWhS_BE].[SPL_SupplierZone_Changed] "
            + " delete from [TOneWhS_BE].[SPL_SupplierZone_New] "
            + " delete from [TOneWhS_BE].[SPL_SupplierZoneService_Changed] "
            + " delete from [TOneWhS_BE].[SPL_SupplierZoneService_New] "
            + " delete from [TOneWhS_SPL].[SupplierCode_Preview] "
            + " delete from [TOneWhS_SPL].[SupplierOtherRate_Preview] "
            + " delete from [TOneWhS_SPL].[SupplierZoneRate_Preview] "
            + " delete from [TOneWhS_BE].[CP_SaleCode_Changed] "
            + " delete from [TOneWhS_BE].[CP_SaleCode_New] "
            + " delete from [TOneWhS_BE].[CP_SalePriceList_New] "
            + " delete from [TOneWhS_BE].[CP_SaleRate_Changed] "
            + " delete from [TOneWhS_BE].[CP_SaleRate_New] "
            + " delete from [TOneWhS_BE].[CP_SaleZone_Changed] "
            + " delete from [TOneWhS_BE].[CP_SaleZone_New] "
            + " delete from [TOneWhS_BE].[CP_SaleZoneRoutingProducts_Changed] " 
            + " delete from [TOneWhS_BE].[CP_SaleZoneServices_Changed] "
            + " delete from [TOneWhs_CP].[SaleCode_Preview] "
            + " delete from [TOneWhs_CP].[SaleRate_Preview] "
            + " delete from [TOneWhs_CP].[SaleZone_Preview] "
            + " delete from [TOneWhS_Sales].[RP_DefaultRoutingProduct_Changed] "
            + " delete from [TOneWhS_Sales].[RP_DefaultRoutingProduct_New] "
            + " delete from [TOneWhS_Sales].[RP_DefaultService_Changed] "
            + " delete from [TOneWhS_Sales].[RP_DefaultService_New] "
            + " delete from [TOneWhS_Sales].[RP_SaleRate_Changed] "
            + " delete from [TOneWhS_Sales].[RP_SaleRate_New] "
            + " delete from [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_Changed] "
            + " delete from [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_New] "
            + " delete from [TOneWhS_Sales].[RP_SaleZoneService_Changed] "
            + " delete from [TOneWhS_Sales].[RP_SaleZoneService_New] "
            + " delete from [TOneWhS_Sales].[RP_DefaultRoutingProduct_Preview] "
            + " delete from [TOneWhS_Sales].[RP_DefaultService_Preview] "
            + " delete from [TOneWhS_Sales].[RP_SaleRate_Preview] "
            + " delete from [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_Preview] "
            + " delete from [TOneWhS_Sales].[RP_SaleZoneService_Preview] ", null);
        }
    }
}
