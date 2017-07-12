
Create PROCEDURE [TOneWhS_Deal].[sp_DealProgress_Insert]
	@DealProgresses [TOneWhS_Deal].[DealProgressType] Readonly
AS
BEGIN
	INSERT [TOneWhS_Deal].[DealProgress] ([DealID],[ZoneGroupNb],[IsSale],[CurrentTierNb],[ReachedDurationInSec],[TargetDurationInSec]) 
	SELECT [DealID],[ZoneGroupNb],[IsSale],[CurrentTierNb],[ReachedDurationInSec],[TargetDurationInSec] 
	FROM @DealProgresses
END