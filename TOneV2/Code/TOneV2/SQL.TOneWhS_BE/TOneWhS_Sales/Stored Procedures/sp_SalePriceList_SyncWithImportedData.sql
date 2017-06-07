-- =============================================
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
	BEGIN TRY
		exec [TOneWhS_Sales].[sp_SalePriceList_SyncWithImportedData_SubProcedure]
		@ProcessInstanceId = @ProcessInstanceId,
		@ReservedSalePriceListId = @ReservedSalePriceListId,
		@OwnerType = @OwnerType,
		@OwnerId = @OwnerId,
		@CurrencyId = @CurrencyId,
		@EffectiveOn = @EffectiveOn
	END TRY
	BEGIN CATCH
		if @@TranCount > 0
			rollback tran
		
		declare @ErrorMessage nvarchar(4000);
		declare @ErrorSeverity int;
		declare @ErrorState int;
		
		select @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
		raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END