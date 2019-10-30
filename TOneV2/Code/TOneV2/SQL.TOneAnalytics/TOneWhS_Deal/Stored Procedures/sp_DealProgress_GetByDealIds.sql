
CREATE PROCEDURE [TOneWhS_Deal].[sp_DealProgress_GetByDealIds]
	@dealIds nvarchar(max)
AS
BEGIN
	DECLARE @DealIdsTable TABLE (dealId int)
	INSERT INTO @DealIdsTable (dealId)
	SELECT Convert(int, ParsedString) FROM [TOneWhS_Deal].[ParseStringList](@dealIds)

	SELECT [ID],[DealID],[ZoneGroupNb],[IsSale],[CurrentTierNb],[ReachedDurationInSec],[TargetDurationInSec],[IsComplete],[CreatedTime]
	FROM [TOneWhS_Deal].[DealProgress]
	WHERE (@dealIds is null or [DealID] in ( SELECT dealId FROM @DealIdsTable))
END