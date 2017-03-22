-- =============================================
-- Description:	Adding try catch will rollback.
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SalePriceList_SyncWithImportedData]
	@ProcessInstanceID Bigint,
	@SellingNumberPlanId int
AS
BEGIN
	SET NOCOUNT ON;

Begin Try
	BEGIN TRAN
	
	Insert into VR_NumberingPlan.SaleZone (ID,SellingNumberPlanID, CountryID, Name, BED, EED)
	Select sznew.ID,sznew.SellingNumberPlanID, sznew.CountryID, sznew.Name, sznew.BED, sznew.EED 
	from VR_NumberingPlan.CP_SaleZone_New sznew WITH(NOLOCK) Where sznew.ProcessInstanceID = @ProcessInstanceID AND sznew.SellingNumberPlanID = @SellingNumberPlanId 
	
	Insert into VR_NumberingPlan.SaleCode (ID, Code, ZoneID, CodeGroupID, BED, EED)
	Select scnew.ID, scnew.Code, scnew.ZoneID, scnew.CodeGroupID, scnew.BED, scnew.EED
	from VR_NumberingPlan.CP_SaleCode_New scnew WITH(NOLOCK) Where scnew.ProcessInstanceID = @ProcessInstanceID

	Update VR_NumberingPlan.SaleZone
	Set EED = szchanged.EED
	from VR_NumberingPlan.SaleZone sz join VR_NumberingPlan.CP_SaleZone_Changed szchanged
	on sz.ID = szchanged.ID Where szchanged.ProcessInstanceID = @ProcessInstanceID
	
	Update VR_NumberingPlan.SaleCode
	Set EED = scchanged.EED
	from VR_NumberingPlan.SaleCode sc join VR_NumberingPlan.CP_SaleCode_Changed scchanged
	on sc.ID = scchanged.ID Where scchanged.ProcessInstanceID = @ProcessInstanceID
	
	 
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