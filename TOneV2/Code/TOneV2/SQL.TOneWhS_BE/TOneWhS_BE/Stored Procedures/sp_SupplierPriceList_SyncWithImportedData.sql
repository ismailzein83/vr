﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierPriceList_SyncWithImportedData]
	@PriceListId INT,
	@ProcessInstanceId BIGINT,
	@SupplierID int,
	@CurrencyID INT,
	@FileID BIGINT,
	@EffectiveOn DateTime
AS
BEGIN
	SET NOCOUNT ON;
 
IF NOT Exists (Select 1 from TOneWhS_BE.SPL_SupplierZone_New Where ProcessInstanceID = @ProcessInstanceId) 
AND NOT Exists (Select 1 from TOneWhS_BE.SPL_SupplierZone_Changed Where ProcessInstanceID = @ProcessInstanceId)
AND NOT Exists (Select 1 from TOneWhS_BE.SPL_SupplierCode_New Where ProcessInstanceID = @ProcessInstanceId)
AND NOT Exists (Select 1 from TOneWhS_BE.SPL_SupplierCode_Changed Where ProcessInstanceID = @ProcessInstanceId)
AND NOT Exists (Select 1 from TOneWhS_BE.SPL_SupplierRate_New Where ProcessInstanceID = @ProcessInstanceId)
AND NOT Exists (Select 1 from TOneWhS_BE.SPL_SupplierRate_Changed Where ProcessInstanceID = @ProcessInstanceId)
	RAISERROR ('Price list has no changes', 12, 1);
ELSE
	Begin 
	Begin Try
	BEGIN TRAN

    Insert into Tonewhs_be.SupplierPriceList (ID, SupplierID, CurrencyID, FileID, EffectiveOn)
	values(@PriceListId, @SupplierID, @CurrencyID, @FileID, @EffectiveOn)
	
	Insert into TOneWhS_BE.SupplierZone (ID, CountryID, Name, SupplierID, BED, EED)
	Select sznew.ID, sznew.CountryID, sznew.Name, sznew.SupplierID, sznew.BED, sznew.EED 
	from TOneWhS_BE.SPL_SupplierZone_New sznew WITH(NOLOCK) Where sznew.ProcessInstanceID = @ProcessInstanceId AND sznew.SupplierID = @SupplierID 
	
	Insert into TOneWhS_BE.SupplierCode (ID, Code, ZoneID, CodeGroupID, BED, EED)
	Select scnew.ID, scnew.Code, scnew.ZoneID, scnew.CodeGroupID, scnew.BED, scnew.EED
	from TOneWhS_BE.SPL_SupplierCode_New scnew WITH(NOLOCK) Where scnew.ProcessInstanceID = @ProcessInstanceId
	
	Insert into TOnewhs_BE.SupplierRate (ID, PriceListID, ZoneID, CurrencyID, NormalRate, OtherRates, RateTypeID,BED, EED)
	Select srnew.ID, @PriceListId, srnew.ZoneID, srnew.CurrencyID, srnew.NormalRate, srnew.OtherRates, srnew.RateTypeID, srnew.BED, srnew.EED
	from TOneWhS_BE.SPL_SupplierRate_New srnew WITH(NOLOCK) Where srnew.ProcessInstanceID = @ProcessInstanceId
	
	Update ToneWhs_be.SupplierZone
	Set EED = szchanged.EED
	from ToneWhs_be.SupplierZone sz join TOneWhS_BE.SPL_SupplierZone_Changed szchanged
	on sz.ID = szchanged.ID Where szchanged.ProcessInstanceID = @ProcessInstanceId
	
	Update TOneWhs_BE.SupplierCode
	Set EED = scchanged.EED
	from TOneWhs_BE.SupplierCode sc join TOneWhS_BE.SPL_SupplierCode_Changed scchanged
	on sc.ID = scchanged.ID Where scchanged.ProcessInstanceID = @ProcessInstanceId
	
	Update TOneWhs_BE.SupplierRate
	Set EED = srchanged.EED
	from TOneWhs_BE.SupplierRate sr join TOneWhS_BE.SPL_SupplierRate_Changed srchanged
	on sr.ID = srchanged.ID Where srchanged.ProcessInstanceID = @ProcessInstanceId
	
	COMMIT TRAN
	End Try
	
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