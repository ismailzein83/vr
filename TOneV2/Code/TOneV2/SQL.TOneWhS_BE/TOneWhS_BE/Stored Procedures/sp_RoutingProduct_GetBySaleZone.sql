-- =============================================
-- Author:		Rabih
-- Create date: 2015-11-19
-- Description:	Get Routing Products by Sale Zone Id
-- =============================================
Create PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_GetBySaleZone]
	@SaleZoneId int
AS
BEGIN

	SET NOCOUNT ON;

	select	rp.ID, 
			rp.Name,
			rp.SellingNumberPlanId, 
			rp.Settings 
	from	TOneWhS_BE.RoutingProduct rp
	join	TOneWhS_BE.SellingNumberPlan snp on rp.SellingNumberPlanId = snp.id
	join	TOneWhS_BE.SaleZone sz on sz.[SellingNumberPlanID] = rp.[SellingNumberPlanID]
	where	sz.Id = @SaleZoneId	
END