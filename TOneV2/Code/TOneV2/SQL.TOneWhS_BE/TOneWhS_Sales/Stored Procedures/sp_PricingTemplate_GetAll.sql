CREATE PROCEDURE [TOneWhS_Sales].[sp_PricingTemplate_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	ID, Name, SellingNumberPlanId, Settings, CreatedTime
	FROM	[TOneWhS_Sales].[PricingTemplate] WITH(NOLOCK) 
END