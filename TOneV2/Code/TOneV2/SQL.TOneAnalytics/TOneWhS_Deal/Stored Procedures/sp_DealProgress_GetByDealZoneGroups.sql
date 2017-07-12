CREATE PROCEDURE [TOneWhS_Deal].[sp_DealProgress_GetByDealZoneGroups]
	@IsSale bit,
	@DealZoneGroups [TOneWhS_Deal].[DealZoneGroupType] Readonly
AS
BEGIN
	SELECT [ID],dp.[DealID],dp.[ZoneGroupNb],[IsSale],[CurrentTierNb],[ReachedDurationInSec],[TargetDurationInSec],[IsComplete],[CreatedTime]
	FROM [TOneWhS_Deal].[DealProgress] dp  WITH(NOLOCK) 
	Join @DealZoneGroups dzg on dp.DealID = dzg.DealId and dzg.ZoneGroupNb = dp.ZoneGroupNb   
	where dp.IsSale = @IsSale
END