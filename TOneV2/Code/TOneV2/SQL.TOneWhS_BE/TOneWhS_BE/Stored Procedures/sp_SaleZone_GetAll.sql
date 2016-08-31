CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetAll] 
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT	[ID],[SellingNumberPlanID],[CountryID],[Name],[BED],[EED],SourceID
	FROM	[TOneWhS_BE].[SaleZone] sz WITH(NOLOCK) 
END