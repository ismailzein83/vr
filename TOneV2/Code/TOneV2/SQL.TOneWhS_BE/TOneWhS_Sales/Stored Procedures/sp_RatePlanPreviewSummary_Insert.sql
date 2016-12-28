-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlanPreviewSummary_Insert]
	@ProcessInstanceID bigint,
	@NumberOfNewRates int,
	@NumberOfIncreasedRates int,
	@NumberOfDecreasedRates int,
	@NumberOfClosedRates int,
	@NameOfNewDefaultRoutingProduct nvarchar(255),
	@NameOfClosedDefaultRoutingProduct nvarchar(255),
	@NumberOfNewSaleZoneRoutingProducts int,
	@NumberOfClosedSaleZoneRoutingProducts int,
	@NewDefaultServices nvarchar(max),
	@ClosedDefaultServiceEffectiveOn datetime = null,
	@NumberOfNewSaleZoneServices int,
	@NumberOfClosedSaleZoneServices int,
	@NumberOfChangedCountries int
AS
BEGIN
	insert into TOneWhS_Sales.RP_RatePlanPreview_Summary
	(
		ProcessInstanceID,
		NumberOfNewRates,
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
		NumberOfClosedSaleZoneServices,
		NumberOfChangedCountries
	)
	
	values
	(
		@ProcessInstanceID,
		@NumberOfNewRates,
		@NumberOfIncreasedRates,
		@NumberOfDecreasedRates,
		@NumberOfClosedRates,
		@NameOfNewDefaultRoutingProduct,
		@NameOfClosedDefaultRoutingProduct,
		@NumberOfNewSaleZoneRoutingProducts,
		@NumberOfClosedSaleZoneRoutingProducts,
		@NewDefaultServices,
		@ClosedDefaultServiceEffectiveOn,
		@NumberOfNewSaleZoneServices,
		@NumberOfClosedSaleZoneServices,
		@NumberOfChangedCountries
	)
END