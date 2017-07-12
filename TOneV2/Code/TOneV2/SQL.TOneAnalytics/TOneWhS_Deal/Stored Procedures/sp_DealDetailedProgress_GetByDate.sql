CREATE PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_GetByDate]
	@IsSale bit,
	@BeginDate datetime,
	@EndDate datetime
AS
BEGIN
	SELECT [ID],ddp.[DealID],ddp.[ZoneGroupNb],[IsSale],[TierNb],[RateTierNb],[FromTime],[ToTime],[ReachedDurationInSec],[CreatedTime]
	FROM [TOneWhS_Deal].[DealDetailedProgress] ddp  WITH(NOLOCK) 
	where ddp.IsSale = @IsSale 
	and (@EndDate is null or [FromTime] < @EndDate)
	and (@BeginDate is null or [ToTime] > @BeginDate)
END