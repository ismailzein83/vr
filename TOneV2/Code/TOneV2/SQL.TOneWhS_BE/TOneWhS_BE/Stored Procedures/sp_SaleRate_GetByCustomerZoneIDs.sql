-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetByCustomerZoneIDs]
	@ownerType INT,
	@customerId INT,
	@zoneIds VARCHAR(MAX),
	@effectiveOn DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ZoneIdsTable TABLE (ZoneId INT)
	INSERT INTO @ZoneIdsTable (ZoneId)
	SELECT Convert(INT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@zoneIds)
	
	SELECT rate.ID,
		rate.PriceListID,
		rate.ZoneID,
		rate.RoutingProductID,
		rate.Rate,
		rate.OffPeakRate,
		rate.WeekendRate,
		rate.BED,
		rate.EED
		
	FROM TOneWhS_BE.SaleRate rate INNER JOIN TOneWhS_BE.SalePriceList pricelist ON rate.PriceListID = pricelist.ID
	
	WHERE pricelist.OwnerType = @ownerType
	AND pricelist.OwnerID = @customerId
	AND rate.ZoneID IN (SELECT ZoneId FROM @ZoneIdsTable)
	AND (rate.BED <= @effectiveOn AND (rate.EED IS NULL OR rate.EED > @effectiveOn))
	
	SET NOCOUNT OFF;
END