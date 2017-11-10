-- =============================================
-- Description:	Adding try catch will rollback.
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_SyncWithImportedData]
	@ProcessInstanceID Bigint,
	@SellingNumberPlanId int,
	@PricelistStateBackupID BIGINT
AS
BEGIN
	SET NOCOUNT ON;

BEGIN TRY
	exec [TOneWhS_BE].[sp_SalePriceList_SyncWithImportedData_SubProcedure]	
	@ProcessInstanceID = @ProcessInstanceID,
	@SellingNumberPlanId = @SellingNumberPlanId,
	@PricelistStateBackupID = @PricelistStateBackupID
END TRY

BEGIN CATCH
	If @@TranCount>0
		RollBack Tran
		
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;
		
		SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH 

END