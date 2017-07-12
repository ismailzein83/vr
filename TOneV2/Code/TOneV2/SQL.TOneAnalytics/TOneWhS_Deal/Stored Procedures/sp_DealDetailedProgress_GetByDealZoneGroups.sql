CREATE PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_GetByDealZoneGroups]
	@IsSale bit,
	@BeginDate datetime,
	@EndDate datetime,
	@DealZoneGroups [TOneWhS_Deal].[DealZoneGroupType] Readonly
AS
BEGIN
	SELECT [ID],ddp.[DealID],ddp.[ZoneGroupNb],[IsSale],[TierNb],[RateTierNb],[FromTime],[ToTime],[ReachedDurationInSec],[CreatedTime]
	FROM [TOneWhS_Deal].[DealDetailedProgress] ddp  WITH(NOLOCK) 
	Join @DealZoneGroups dzg on ddp.DealID = dzg.DealId and dzg.ZoneGroupNb = ddp.ZoneGroupNb   
	where ddp.IsSale = @IsSale 
	and (@EndDate is null or [FromTime] < @EndDate)
	and (@BeginDate is null or [ToTime] > @BeginDate)
END