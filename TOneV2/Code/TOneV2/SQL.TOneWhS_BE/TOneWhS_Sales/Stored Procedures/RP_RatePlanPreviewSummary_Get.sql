-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[RP_RatePlanPreviewSummary_Get]
	@ProcessInstanceID bigint
AS
BEGIN
	select NumberOfNewRates,
		NumberOfIncreasedRates,
		NumberOfDecreasedRates,
		NumberOfClosedRates,
		NameOfNewDefaultRoutingProduct,
		NameOfClosedDefaultRoutingProduct,
		NumberOfNewSaleZoneRoutingProducts,
		NumberOfClosedSaleZoneRoutingProducts,
		NewDefaultServices,
		ClosedDefaultServiceEffectiveOn,
		NumberOfNewSaleZoneServices,
		NumberOfClosedSaleZoneServices
	
	from [TOneWhS_Sales].RP_RatePlanPreview_Summary WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
END