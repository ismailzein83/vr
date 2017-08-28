﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierPriceList_SyncWithImportedData]
	@PriceListId INT,
	@ProcessInstanceId BIGINT,
	@StateBackupId BIGINT = null,
	@SupplierID int,
	@CurrencyID INT,
	@FileID BIGINT,
	@EffectiveOn DateTime
AS
BEGIN
	SET NOCOUNT ON;
	Begin 
	BEGIN TRY
	
	exec [TOneWhS_BE].[sp_SupplierPriceList_SyncWithImportedData_SubProcedure]
	@PriceListId = @PriceListId,
	@ProcessInstanceId = @ProcessInstanceId,
	@StateBackupId=@StateBackupId,
	@SupplierID = @SupplierID,
	@CurrencyID = @CurrencyID,
	@FileID = @FileID,
	@EffectiveOn = @EffectiveOn
	END TRY
	
	Begin Catch
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
	End Catch 
	END
END