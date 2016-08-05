-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetExistingByZoneIDs]
	@OwnerType INT,
	@OwnerID INT,
	@ZoneIDs VARCHAR(MAX),
	@MinEED DATETIME
AS
BEGIN
	IF (@ZoneIDs IS NOT NULL)
	BEGIN
		DECLARE @ZoneIDsTable AS TABLE (ZoneID BIGINT)
		INSERT INTO @ZoneIDsTable SELECT CONVERT(BIGINT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@ZoneIDs)
	END;
	
	WITH PriceListIDsCTE (PriceListID) AS (SELECT ID FROM TOneWhS_BE.SalePriceList WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID)
	SELECT ID,
		PriceListID,
		ZoneID,
		CurrencyID,
		RateTypeID,
		Rate,
		OtherRates,
		BED,
		EED,
		Change
	FROM TOneWhS_BE.SaleRate
	WHERE PriceListID IN (SELECT PriceListID FROM PriceListIDsCTE)
		AND ZoneID IN (SELECT ZoneID FROM @ZoneIDsTable)
		AND (EED IS NULL OR EED > @MinEED)
END