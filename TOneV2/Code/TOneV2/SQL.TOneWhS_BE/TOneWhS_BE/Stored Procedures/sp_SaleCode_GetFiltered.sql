CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetFiltered]
	@SellingNumberPlanID int,
	@ZonesIDs varchar(max),
	@Code varchar(20),
	@EffectiveOn datetime,
	@GetEffectiveAfter bit
AS
BEGIN
	set nocount on;
	begin
	
		declare @ZonesIDsTable table (ZoneID int)
		insert into @ZonesIDsTable (ZoneID) select convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)

		select sc.[ID], sc.[Code], sc.[ZoneID], sc.[CodeGroupID], sc.[BED], sc.[EED], sc.[SourceID]
		from [TOneWhS_BE].SaleCode sc with(nolock)
		inner join [TOneWhS_BE].SaleZone sz with(nolock) on sc.ZoneID = sz.ID
        
		where
		(
			-- Get effective codes; EED is necessarily > BED
			(@GetEffectiveAfter = 0 and sc.BED <= @EffectiveOn and (sc.EED is null or sc.EED > @EffectiveOn))
			or
			-- Get codes effective after; Exclude deleted codes
			(@GetEffectiveAfter = 1 and (sc.EED is null or (sc.EED > sc.BED and sc.EED > @EffectiveOn)))
		)
		and (@Code is null or sc.Code like @Code + '%')
		and (@SellingNumberPlanID is null or @SellingNumberPlanID = sz.SellingNumberPlanID)
		and (@ZonesIDs is null or sc.ZoneID in (select ZoneID from @ZonesIDsTable))
	END
	set nocount off
END