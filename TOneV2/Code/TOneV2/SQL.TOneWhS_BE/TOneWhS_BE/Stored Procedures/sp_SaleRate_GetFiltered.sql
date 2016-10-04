﻿CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetFiltered]
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
	
	SELECT	sr.[ID],sr.[PriceListID],sr.[ZoneID],sr.[CurrencyID],sr.RateTypeID,sr.[Rate],sr.[OtherRates],sr.[BED],sr.[EED],sr.Change, sr.CurrencyID
	FROM	[TOneWhS_BE].SaleRate sr WITH(NOLOCK) 
			inner join   [TOneWhS_BE].SalePriceList sp WITH(NOLOCK) on sr.PriceListID = sp.ID
			inner join  [TOneWhS_BE].SaleZone sz WITH(NOLOCK) on  sr.ZoneID = sz.ID          
    WHERE	(sr.EED is null or sr.BED <> sr.EED) -- Don't select deleted rates
			and (sr.BED < = @EffectiveOn)
			and (sr.EED is null or sr.EED  > @EffectiveOn)
			and (@SellingNumberPlanID is null or @SellingNumberPlanID = sz.SellingNumberPlanID)
			and (@ZonesIDs  is null or sr.ZoneID in (select ZoneID from @ZonesIDsTable))
			and (@OwnerID = sp.OwnerID)
			and (@OwnerType = sp.OwnerType);
END
SET NOCOUNT OFF
END