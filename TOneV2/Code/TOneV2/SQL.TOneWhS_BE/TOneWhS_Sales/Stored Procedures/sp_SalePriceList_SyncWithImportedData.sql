﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SalePriceList_SyncWithImportedData]
	@ProcessInstanceId bigint,
	@ReservedSalePriceListId int = null,
	@OwnerType int,
	@OwnerId int,
	@CurrencyId int,
	@EffectiveOn datetime
AS
BEGIN
	begin try
		begin tran

			if @ReservedSalePriceListId is not null
			begin
				-- Sync rates

				insert into TOneWhS_BE.SalePriceList (ID, OwnerType, OwnerID, CurrencyID, EffectiveOn)
				values (@ReservedSalePriceListId, @OwnerType, @OwnerID, @CurrencyID, @EffectiveOn)
				
				insert into TOneWhS_BE.SaleRate (ID, PriceListID, ZoneID, CurrencyID, RateTypeID, Rate, Change, BED, EED)
				select ID, @ReservedSalePriceListId, ZoneID, CurrencyId, RateTypeID, Rate, 1, BED, EED
				from TOneWhS_Sales.RP_SaleRate_New newRate WITH(NOLOCK) 
				where newRate.ProcessInstanceID = @ProcessInstanceId
				
				update TOneWhS_BE.SaleRate
				set EED = changedRate.EED
				from TOneWhS_BE.SaleRate rate  WITH(NOLOCK) inner join TOneWhS_Sales.RP_SaleRate_Changed changedRate  WITH(NOLOCK) on rate.ID = changedRate.ID
				where changedRate.ProcessInstanceID = @ProcessInstanceId

				-- Sync default services

				insert into TOneWhS_BE.SaleEntityService (ID, PriceListID, ZoneID, [Services], BED, EED)
				select ID, @ReservedSalePriceListId, null, [Services], BED, EED
				from TOneWhS_Sales.RP_DefaultService_New newService WITH(NOLOCK) 
				where newService.ProcessInstanceID = @ProcessInstanceID
			
				update TOneWhS_BE.SaleEntityService
				set EED = changedService.EED
				from TOneWhS_BE.SaleEntityService ses WITH(NOLOCK) inner join TOneWhS_Sales.RP_DefaultService_Changed changedService on ses.ID = changedService.ID
				where changedService.ProcessInstanceID = @ProcessInstanceID

				-- Sync zone services

				insert into TOneWhS_BE.SaleEntityService (ID, PriceListID, ZoneID, [Services], BED, EED)
				select ID, @ReservedSalePriceListId, ZoneID, [Services], BED, EED
				from TOneWhS_Sales.RP_SaleZoneService_New newService
				where newService.ProcessInstanceID = @ProcessInstanceID
			
				update TOneWhS_BE.SaleEntityService
				set EED = changedService.EED
				from TOneWhS_BE.SaleEntityService ses inner join TOneWhS_Sales.RP_SaleZoneService_Changed changedService on ses.ID = changedService.ID
				where changedService.ProcessInstanceID = @ProcessInstanceID
			end
			
			-- Sync default routing product
			
			insert into TOneWhS_BE.SaleEntityRoutingProduct (ID, OwnerType, OwnerID, RoutingProductID, BED, EED)
			select ID, @OwnerType, @OwnerID, RoutingProductID, BED, EED
			from TOneWhS_Sales.RP_DefaultRoutingProduct_New newDRP WITH(NOLOCK) 
			where newDRP.ProcessInstanceID = @ProcessInstanceID
			
			update TOneWhS_BE.SaleEntityRoutingProduct
			set EED = changedDRP.EED
			from TOneWhS_BE.SaleEntityRoutingProduct serp  WITH(NOLOCK) inner join TOneWhS_Sales.RP_DefaultRoutingProduct_Changed changedDRP on serp.ID = changedDRP.ID
			where changedDRP.ProcessInstanceID = @ProcessInstanceID
			
			-- Sync zone routing products
			
			insert into TOneWhS_BE.SaleEntityRoutingProduct (ID, OwnerType, OwnerID, RoutingProductID, ZoneID, BED, EED)
			select ID, @OwnerType, @OwnerID, RoutingProductID, ZoneID, BED, EED
			from TOneWhS_Sales.RP_SaleZoneRoutingProduct_New newSZRP
			where newSZRP.ProcessInstanceID = @ProcessInstanceID
			
			update TOneWhS_BE.SaleEntityRoutingProduct
			set EED = changedSZRP.EED
			from TOneWhS_BE.SaleEntityRoutingProduct szrp inner join TOneWhS_Sales.RP_SaleZoneRoutingProduct_Changed changedSZRP on szrp.ID = changedSZRP.ID
			where changedSZRP.ProcessInstanceID = @ProcessInstanceID
			
		commit tran
	end try
	
	begin catch
		if @@TranCount > 0
			rollback tran
		
		declare @ErrorMessage nvarchar(4000);
		declare @ErrorSeverity int;
		declare @ErrorState int;
		
		select @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
		raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
	end catch
END