using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class PostDataDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {

        public PostDataDBSyncDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public bool FixSellingProductRates(int sellingNumberPlanId, int sellingProductId, int currencyId, decimal defaultRate)
        {
            int result = (int)ExecuteNonQueryText(query_FixSellingProductRates, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@SellingNumberPlanId", sellingNumberPlanId));
                cmd.Parameters.Add(new SqlParameter("@SellingProductID", sellingProductId));
                cmd.Parameters.Add(new SqlParameter("@CurrencyID", currencyId));
                cmd.Parameters.Add(new SqlParameter("@DefaultRate", defaultRate));
            });
            return result > 0;
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetSchema()
        {
            return "";
        }

        const string query_FixSellingProductRates = @"

DECLARE @SalePriceListTypeId int, @SaleRateTypeId int, @SalePriceListStartingId bigint, @SaleRateStartingId bigint,  @SaleRateCount int

--Start adding selling product rates when missing
Select @SaleRateCount = (Select Count(*) from [TOneWhS_BE].[SaleZone] sz where sz.SellingNumberPlanID = @SellingNumberPlanId)

declare @commonType as table (TypeId bigint)

insert into @commonType exec [common].[sp_Type_InsertIfNotExistsAndGetID] 'TOne.WhS.BusinessEntity.Business.SalePriceListManager, TOne.WhS.BusinessEntity.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
SELECT @SalePriceListTypeId = TypeId from @commonType
delete @commonType

insert into @commonType exec [common].[sp_Type_InsertIfNotExistsAndGetID] 'TOne.WhS.BusinessEntity.Business.SaleRateManager, TOne.WhS.BusinessEntity.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
SELECT @SaleRateTypeId = TypeId from @commonType
delete @commonType

insert into @commonType exec [common].[sp_IDManager_ReserveIDRange] @SalePriceListTypeId, 1
SELECT @SalePriceListStartingId = TypeId from @commonType
delete @commonType

insert into @commonType exec [common].[sp_IDManager_ReserveIDRange] @SaleRateTypeId, @SaleRateCount
SELECT @SaleRateStartingId = TypeId from @commonType
delete @commonType

Insert into [TOneWhS_BE].[SalePriceList] (ID, OwnerType, OwnerID, CurrencyID, EffectiveOn)
Values (@SalePriceListStartingId, 0, @SellingProductID, @CurrencyID, GETDATE())

Insert into [TOneWhS_BE].[SaleRate] (ID, PriceListID, ZoneID, Rate, BED, EED, Change)
Select (ROW_NUMBER () OVER ( ORDER BY [ID])) + @SaleRateStartingId - 1,
 @SalePriceListStartingId, sz.ID, @DefaultRate, sz.BED, sz.EED, 1 
 from [TOneWhS_BE].[SaleZone] sz 
 where 	sz.SellingNumberPlanID = @SellingNumberPlanId 
		and 	Not Exists(Select top 1 * 
							from [TOneWhS_BE].[SaleRate] sr join [TOneWhS_BE].SalePriceList spl on sr.PriceListID = spl.ID 
							where sr.ZoneID = sz.ID and spl.OwnerType = 0 and spl.OwnerID = @SellingProductID)
--End adding selling product rates when missing

--run this query after assigning a default routing product for same selling number plan used in migration

Declare @BEDSaleZone datetime

select top 1 @BEDSaleZone=[BED] from [TOneWhS_BE].[SaleZone] 
where	[SellingNumberPlanID]=@SellingNumberPlanId --default used in migration
order by [BED]

update	TOneWhS_BE.SaleEntityRoutingProduct
set		BED= isnull(dateadd(year,-1,@BEDSaleZone),'2000-01-01')
where	(OwnerType = 0) AND (OwnerID = @SellingProductID) 
		AND (ZoneID IS NULL)

Declare @BEDSupplierZone datetime

Select top 1 @BEDSupplierZone=[BED] from [TOneWhS_BE].[SupplierZone] 
order by [BED]

declare @ExchDateToInsert datetime = Case When @BEDSaleZone < @BEDSupplierZone 
               Then @BEDSaleZone Else @BEDSupplierZone End

;With CTEMinDate (CurrencyID, ExDate)
AS (select CurrencyID, MIN(ExchangeDate) ExDate from common.CurrencyExchangeRate group by CurrencyID),

CTEFirstRate as (SELECT exRate.CurrencyID, ExDate as MinExchangeDate, exRate.Rate
FROM CTEMinDate JOIN common.CurrencyExchangeRate exRate ON CTEMinDate.CurrencyID = exRate.CurrencyID AND CTEMinDate.ExDate = exRate.ExchangeDate)

Insert into common.CurrencyExchangeRate (CurrencyID, Rate, ExchangeDate)
select CurrencyID, Rate, @ExchDateToInsert from CTEFirstRate
WHERE @ExchDateToInsert < MinExchangeDate

";
    }
}
