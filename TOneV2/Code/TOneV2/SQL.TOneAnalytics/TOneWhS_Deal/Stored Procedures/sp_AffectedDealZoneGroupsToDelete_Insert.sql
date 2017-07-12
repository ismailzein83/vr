
CREATE PROCEDURE [TOneWhS_Deal].[sp_AffectedDealZoneGroupsToDelete_Insert]
	@IsSale bit,
	@DealZoneGroups [TOneWhS_Deal].[DealZoneGroupType] Readonly
AS
BEGIN
	INSERT [TOneWhS_Deal].[AffectedDealZoneGroupsToDelete] ([DealID],[ZoneGroupNb],[IsSale]) 
	SELECT [DealID],[ZoneGroupNb],@IsSale
	FROM @DealZoneGroups
END