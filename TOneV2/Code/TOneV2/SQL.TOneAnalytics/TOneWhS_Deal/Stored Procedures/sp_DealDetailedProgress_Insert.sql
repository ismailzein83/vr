
CREATE PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_Insert]
	@DealDetailedProgresses [TOneWhS_Deal].[DealDetailedProgressType] Readonly
AS
BEGIN
	INSERT [TOneWhS_Deal].[DealDetailedProgress] ([DealID],[ZoneGroupNb],[IsSale],[TierNb],[RateTierNb],[FromTime],[ToTime],[ReachedDurationInSec]) 
	SELECT [DealID],[ZoneGroupNb],[IsSale],[TierNb],[RateTierNb],[FromTime],[ToTime],[ReachedDurationInSec] 
	FROM @DealDetailedProgresses
END