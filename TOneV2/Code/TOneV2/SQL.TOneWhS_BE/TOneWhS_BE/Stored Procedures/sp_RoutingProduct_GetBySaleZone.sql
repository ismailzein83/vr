-- =============================================
-- Author:		Rabih
-- Create date: 2015-11-19
-- Description:	Get Routing Products by Sale Zone Id
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_GetBySaleZone]
	@SaleZoneId int
AS
BEGIN

	SET NOCOUNT ON;

	select	rp.ID,rp.Name,rp.SellingNumberPlanId, rp.Settings 
	from	TOneWhS_BE.RoutingProduct rp WITH(NOLOCK) 
			inner join	TOneWhS_BE.SellingNumberPlan snp WITH(NOLOCK) on rp.SellingNumberPlanId = snp.id
			inner join	TOneWhS_BE.SaleZone sz WITH(NOLOCK) on sz.[SellingNumberPlanID] = rp.[SellingNumberPlanID]
	where	sz.Id = @SaleZoneId	
END