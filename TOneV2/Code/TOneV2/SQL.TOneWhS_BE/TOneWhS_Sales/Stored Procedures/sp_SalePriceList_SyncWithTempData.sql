-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_SalePriceList_SyncWithTempData
	@ProcessInstanceId bigint,
	@SalePriceListId int = null,
	@OwnerType int,
	@OwnerId int,
	@CurrencyId int
AS
BEGIN
	begin tran
		-- Sync rates
		if @SalePriceListId is not null
		begin
			set identity_insert TOneWhS_BE.SaleRate on
			insert into TOneWhS_BE.SaleRate (ID, PriceListID, ZoneID, CurrencyID, Rate, OtherRates, BED, EED)
			select ID, @SalePriceListId, ZoneID, CurrencyId, NormalRate, OtherRates, BED, EED
			from TOneWhS_Sales.RP_SaleRate_New newRate
			where newRate.ProcessInstanceID = @ProcessInstanceId
			set identity_insert TOneWhS_BE.SaleRate off
			
			update TOneWhS_BE.SaleRate
			set EED = changedRate.EED
			from TOneWhS_BE.SaleRate rate inner join TOneWhS_Sales.RP_SaleRate_Changed changedRate on rate.ID = changedRate.ID
			where changedRate.ProcessInstanceID = @ProcessInstanceId
		end
		
		-- Sync routing products
		-- Do work!
	commit tran
END