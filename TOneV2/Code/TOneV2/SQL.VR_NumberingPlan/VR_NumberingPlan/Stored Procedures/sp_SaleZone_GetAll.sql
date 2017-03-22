CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleZone_GetAll] 
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT	[ID],[SellingNumberPlanID],[CountryID],[Name],[BED],[EED],SourceID
	FROM	[VR_NumberingPlan].[SaleZone] sz WITH(NOLOCK) 
END