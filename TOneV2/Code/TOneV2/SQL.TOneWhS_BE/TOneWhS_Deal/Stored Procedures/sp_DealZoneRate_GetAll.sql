CREATE PROCEDURE [TOneWhS_Deal].[sp_DealZoneRate_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	dzr.ID,dzr.DealId,dzr.ZoneGroupNb,dzr.IsSale,dzr.TierNb,dzr.ZoneId,dzr.CurrencyId, dzr.Rate,dzr.BED,dzr.EED
	FROM	[TOneWhS_Deal].DealZoneRate dzr WITH(NOLOCK) 
	SET NOCOUNT OFF
END