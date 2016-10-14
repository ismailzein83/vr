-- =============================================
-- Description:	Get Effective Sale Rates by Selling Number Plan
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetBySellingNumberPlanAndEffective]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlan INT,
	@Effective DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.OtherRates,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED
FROM	[TOneWhS_BE].SaleRate sr WITH(NOLOCK) 
		INNER JOIN [TOneWhS_BE].SaleZone sz WITH(NOLOCK) ON sr.ZoneID=sz.ID 
Where	(sr.BED<=@Effective and (sr.EED is null or sr.EED > @Effective))
		and sz.SellingNumberPlanID=@SellingNumberPlan
END