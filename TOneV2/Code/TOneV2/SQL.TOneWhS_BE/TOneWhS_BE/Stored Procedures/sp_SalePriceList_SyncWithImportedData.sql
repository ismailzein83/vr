-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_SyncWithImportedData]
	@ProcessInstanceID Bigint,
	@SellingNumberPlanId int
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRAN
	
	Insert into TOneWhS_BE.SaleZone (ID,SellingNumberPlanID, CountryID, Name, BED, EED)
	Select sznew.ID,sznew.SellingNumberPlanID, sznew.CountryID, sznew.Name, sznew.BED, sznew.EED 
	from TOneWhS_BE.CP_SaleZone_New sznew Where sznew.ProcessInstanceID = @ProcessInstanceID AND sznew.SellingNumberPlanID = @SellingNumberPlanId 
	
	Insert into TOneWhS_BE.SaleCode (ID, Code, ZoneID, CodeGroupID, BED, EED)
	Select scnew.ID, scnew.Code, scnew.ZoneID, scnew.CodeGroupID, scnew.BED, scnew.EED
	from TOneWhS_BE.CP_SaleCode_New scnew Where scnew.ProcessInstanceID = @ProcessInstanceID

	
	Update ToneWhs_be.SaleZone
	Set EED = szchanged.EED
	from ToneWhs_be.SaleZone sz join TOneWhS_BE.CP_SaleZone_Changed szchanged
	on sz.ID = szchanged.ID Where szchanged.ProcessInstanceID = @ProcessInstanceID
	
	Update TOneWhs_BE.SaleCode
	Set EED = scchanged.EED
	from TOneWhs_BE.SaleCode sc join TOneWhS_BE.CP_SaleCode_Changed scchanged
	on sc.ID = scchanged.ID Where scchanged.ProcessInstanceID = @ProcessInstanceID
	
	
	COMMIT TRAN
END