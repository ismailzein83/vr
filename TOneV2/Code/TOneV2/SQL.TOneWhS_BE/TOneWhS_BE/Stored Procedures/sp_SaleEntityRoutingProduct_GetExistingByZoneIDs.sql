-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetExistingByZoneIDs]
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
	SELECT ID,ZoneID,RoutingProductID,BED,EED,OwnerType,OwnerID
	FROM [TOneWhS_BE].SaleEntityRoutingProduct WITH(NOLOCK) 
	WHERE (OwnerType = @OwnerType AND OwnerID = @OwnerID)
		AND ((ZoneID IN (SELECT ZoneID FROM @ZoneIDsTable)) OR ZoneID is NULL)
		AND (EED IS NULL OR (EED!=BED and EED > @MinEED))
END