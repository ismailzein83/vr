﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[RP_RatePlanPreviewSummary_Get]
	@ProcessInstanceID_IN bigint
AS
BEGIN

DECLARE @ProcessInstanceID bigint

Select @ProcessInstanceID = @ProcessInstanceID_IN
-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

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

	SET NOCOUNT OFF

END