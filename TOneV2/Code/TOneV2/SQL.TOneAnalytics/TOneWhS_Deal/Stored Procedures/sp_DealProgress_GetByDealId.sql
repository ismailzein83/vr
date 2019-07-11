
CREATE PROCEDURE [TOneWhS_Deal].[sp_DealProgress_GetByDealId]
	@dealId int
AS
BEGIN
	
	SELECT [ID],[DealID],[ZoneGroupNb],[IsSale],[CurrentTierNb],[ReachedDurationInSec],[TargetDurationInSec],[IsComplete],[CreatedTime]
	FROM [TOneV2_Dev_Analytics].[TOneWhS_Deal].[DealProgress]
	WHERE DealID = @dealId
END