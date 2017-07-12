
CREATE PROCEDURE [TOneWhS_Deal].[sp_AffectedDealZoneGroupsToDelete_Get]
	@IsSale bit
AS
BEGIN
	Select DealID, ZoneGroupNb
	From [TOneWhS_Deal].[AffectedDealZoneGroupsToDelete] 
	where IsSale = @IsSale
END