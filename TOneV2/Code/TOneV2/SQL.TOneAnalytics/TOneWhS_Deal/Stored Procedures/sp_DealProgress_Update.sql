
Create PROCEDURE [TOneWhS_Deal].[sp_DealProgress_Update]
	@DealProgresses [TOneWhS_Deal].[DealProgressType] Readonly
AS
BEGIN
	UPDATE dp
	SET dp.[DealID] = tempdp.[DealID], dp.[ZoneGroupNb] = tempdp.[ZoneGroupNb], dp.[IsSale] = tempdp.[IsSale], dp.[CurrentTierNb] = tempdp.[CurrentTierNb],
		dp.[ReachedDurationInSec] = tempdp.[ReachedDurationInSec], dp.[TargetDurationInSec] = tempdp.[TargetDurationInSec]
	FROM [TOneWhS_Deal].[DealProgress] dp
	JOIN @DealProgresses tempdp on dp.ID = tempdp.DealProgressID
END