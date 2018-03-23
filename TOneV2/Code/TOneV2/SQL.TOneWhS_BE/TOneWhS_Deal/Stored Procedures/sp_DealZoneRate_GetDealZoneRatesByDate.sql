CREATE PROCEDURE [TOneWhS_Deal].[sp_DealZoneRate_GetDealZoneRatesByDate]
@isSale bit,
@fromDate datetime,
@toDate datetime
AS
BEGIN

	SELECT	dzr.ID,dzr.DealId,dzr.ZoneGroupNb,dzr.IsSale,dzr.TierNb,dzr.ZoneId,dzr.Rate,dzr.BED,dzr.EED
	FROM	[TOneWhS_Deal].DealZoneRate dzr WITH(NOLOCK) 
	Where dzr.IsSale = @isSale and dzr.BED < @toDate and (dzr.EED is null or (dzr.EED <> dzr.BED and dzr.EED > @fromDate))

END