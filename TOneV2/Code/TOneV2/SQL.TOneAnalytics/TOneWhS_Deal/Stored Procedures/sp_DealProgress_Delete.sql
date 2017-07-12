CREATE PROCEDURE [TOneWhS_Deal].[sp_DealProgress_Delete]
	@IsSale bit,
	@DealZoneGroups [TOneWhS_Deal].[DealZoneGroupType] Readonly
AS
BEGIN
	delete dp FROM [TOneWhS_Deal].[DealProgress] dp
	Join @DealZoneGroups dzg on dp.DealID = dzg.DealId and dzg.ZoneGroupNb = dp.ZoneGroupNb   
	where dp.IsSale = @IsSale
END