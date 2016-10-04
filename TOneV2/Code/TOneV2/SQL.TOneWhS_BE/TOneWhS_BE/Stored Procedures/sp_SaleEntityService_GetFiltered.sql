CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityService_GetFiltered]
	@EffectiveOn dateTime,
	@SellingNumberPlanID int ,
	@ZonesIDs varchar(max),
	@OwnerType int,
	@OwnerID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;
	
BEGIN
DECLARE @ZonesIDsTable TABLE (ZoneID int)
INSERT INTO @ZonesIDsTable (ZoneID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)
	
	SELECT	ses.[ID],ses.[PriceListID],ses.[ZoneID],ses.[Services],ses.[BED],ses.[EED]
	FROM	[TOneWhS_BE].SaleEntityService ses WITH(NOLOCK) 
			inner join   [TOneWhS_BE].SalePriceList sp WITH(NOLOCK) on ses.PriceListID = sp.ID
			inner join  [TOneWhS_BE].SaleZone sz WITH(NOLOCK) on  ses.ZoneID = sz.ID          
    WHERE	(ses.EED is null or ses.BED <> ses.EED) 
			and (ses.BED < = @EffectiveOn)
			and (ses.EED is null or ses.EED  > @EffectiveOn)
			and (@SellingNumberPlanID is null or @SellingNumberPlanID = sz.SellingNumberPlanID)
			and (@ZonesIDs  is null or ses.ZoneID in (select ZoneID from @ZonesIDsTable))
			and (@OwnerID = sp.OwnerID)
			and (@OwnerType = sp.OwnerType);
END
SET NOCOUNT OFF
END