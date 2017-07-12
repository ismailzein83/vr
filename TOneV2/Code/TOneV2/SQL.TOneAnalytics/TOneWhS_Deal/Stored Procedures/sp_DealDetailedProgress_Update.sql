
Create PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_Update]
	@DealDetailedProgresses [TOneWhS_Deal].[DealDetailedProgressType] Readonly
AS
BEGIN
	UPDATE ddp
	SET ddp.[DealID] = tempddp.[DealID], ddp.[ZoneGroupNb] = tempddp.[ZoneGroupNb], ddp.[IsSale] = tempddp.[IsSale], ddp.[TierNb] = tempddp.[TierNb],
		ddp.[RateTierNb] = tempddp.[RateTierNb], ddp.[ReachedDurationInSec] = tempddp.[ReachedDurationInSec], ddp.[FromTime] = tempddp.[FromTime], ddp.[ToTime] = tempddp.[ToTime]
	FROM [TOneWhS_Deal].[DealDetailedProgress] ddp
	JOIN @DealDetailedProgresses tempddp on ddp.ID = tempddp.DealDetailedProgressID
END