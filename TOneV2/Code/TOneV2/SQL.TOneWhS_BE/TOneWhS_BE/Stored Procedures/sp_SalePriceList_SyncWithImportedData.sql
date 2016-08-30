-- =============================================
-- Author:		Mostafa Jawhari
-- Modify date: 04-13-2016
-- Description:	Adding try catch will rollback.
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_SyncWithImportedData]
	@ProcessInstanceID Bigint,
	@SellingNumberPlanId int
AS
BEGIN
	SET NOCOUNT ON;

Begin Try
	BEGIN TRAN
	
	Insert into TOneWhS_BE.SalePriceList (ID, OwnerType, OwnerID, CurrencyID, EffectiveOn)
	Select	splnew.ID, splnew.OwnerType, splnew.OwnerID, splnew.CurrencyID, splnew.EffectiveOn	
	from TOneWhS_BE.CP_SalePriceList_New splnew WITH(NOLOCK) Where splnew.ProcessInstanceID = @ProcessInstanceID 
	
	Insert into TOneWhS_BE.SaleZone (ID,SellingNumberPlanID, CountryID, Name, BED, EED)
	Select sznew.ID,sznew.SellingNumberPlanID, sznew.CountryID, sznew.Name, sznew.BED, sznew.EED 
	from TOneWhS_BE.CP_SaleZone_New sznew WITH(NOLOCK) Where sznew.ProcessInstanceID = @ProcessInstanceID AND sznew.SellingNumberPlanID = @SellingNumberPlanId 
	
	Insert into TOneWhS_BE.SaleCode (ID, Code, ZoneID, CodeGroupID, BED, EED)
	Select scnew.ID, scnew.Code, scnew.ZoneID, scnew.CodeGroupID, scnew.BED, scnew.EED
	from TOneWhS_BE.CP_SaleCode_New scnew WITH(NOLOCK) Where scnew.ProcessInstanceID = @ProcessInstanceID

	Insert into TOneWhS_BE.SaleRate (ID, PriceListID, ZoneID, CurrencyID, Rate, BED, EED)
	select newRate.ID, newRate.PriceListID, newRate.ZoneID, newRate.CurrencyId, newRate.NormalRate, newRate.BED, newRate.EED
	from TOneWhS_BE.CP_SaleRate_New newRate WITH(NOLOCK) where newRate.ProcessInstanceID = @ProcessInstanceId
	
	Update ToneWhs_be.SaleZone
	Set EED = szchanged.EED
	from ToneWhs_be.SaleZone sz join TOneWhS_BE.CP_SaleZone_Changed szchanged
	on sz.ID = szchanged.ID Where szchanged.ProcessInstanceID = @ProcessInstanceID
	
	Update TOneWhs_BE.SaleCode
	Set EED = scchanged.EED
	from TOneWhs_BE.SaleCode sc join TOneWhS_BE.CP_SaleCode_Changed scchanged
	on sc.ID = scchanged.ID Where scchanged.ProcessInstanceID = @ProcessInstanceID
	
	Update TOneWhs_BE.SaleRate
	Set EED = srchanged.EED
	from TOneWhs_BE.SaleRate sr join TOneWhS_BE.CP_SaleRate_Changed srchanged
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