
CREATE PROCEDURE [TOneWhS_Deal].[sp_DealZoneRate_GetDealZoneRatesByDealIds]
@isSale bit,
@dealIds nvarchar(max)

AS
BEGIN
	declare @DealIdsTable as table (DealId int)
	if (@dealIds is not null) 
	begin 
		insert into @DealIdsTable 
		select convert(int, ParsedString)
		from [TOneWhS_Deal].[ParseStringList](@dealIds) 
	end

	SELECT	dzr.ID,dzr.DealId,dzr.ZoneGroupNb,dzr.IsSale,dzr.TierNb,dzr.ZoneId,dzr.Rate,dzr.BED,dzr.EED
	FROM	[TOneWhS_Deal].DealZoneRate dzr WITH(NOLOCK)
	JOIN @DealIdsTable aux on aux.DealId = dzr.DealId
	WHERE dzr.IsSale = @isSale

END