
-- =============================================
-- Description:	Get all sale entity zone routing Products effective by sellingNumberPlanId
-- =============================================

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityZoneRoutingProducts_GetEffectiveBySellingNumberPlan]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanId bigint,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  rp.[ID], rp.[OwnerType], rp.[OwnerID], rp.[ZoneID], rp.[RoutingProductID], rp.[BED], rp.[EED]
FROM	[TOneWhS_BE].[SaleEntityRoutingProduct] rp WITH(NOLOCK) 
		JOIN	[TOneWhS_BE].[SaleZone] sz WITH(NOLOCK) ON rp.ZoneID=sz.ID
WHERE	sz.[SellingNumberPlanID]=@SellingNumberPlanId
	     and (rp.EED is null or rp.EED > @when) and rp.ZoneID is not  NULL        
END