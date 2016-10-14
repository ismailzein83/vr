-- =============================================
-- Description:	Get all sale entity zone services effective by sellingNumberPlanId
-- =============================================

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityZoneServices_GetEffectiveBySellingNumberPlan]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanId bigint,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  se.[ID], se.[PriceListID], se.[ZoneID], se.[Services], se.[BED], se.[EED], se.[SourceID]
FROM	[TOneWhS_BE].[SaleEntityService] se WITH(NOLOCK) 
		JOIN	[TOneWhS_BE].[SaleZone] sz WITH(NOLOCK) ON se.ZoneID=sz.ID
WHERE	sz.[SellingNumberPlanID]=@SellingNumberPlanId
	    and (se.EED is null or se.EED > @when) and se.ZoneID is not NULL        
END