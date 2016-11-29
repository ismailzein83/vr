CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetFiltered]
	@EffectiveOn dateTime ,
	@Code varchar(20),
	@SellingNumberPlanID int ,
	@ZonesIDs varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [VR_NumberingPlan].[ParseStringList](@ZonesIDs)
			SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[CodeGroupID],sc.[BED],sc.[EED],sc.[SourceID]
			FROM [VR_NumberingPlan].SaleCode sc WITH(NOLOCK) 
			inner join  [VR_NumberingPlan].SaleZone sz WITH(NOLOCK) on  sc.ZoneID = sz.ID   
            WHERE 
                (sc.BED < = @EffectiveOn)
            and (sc.EED is null or sc.EED  > @EffectiveOn)
            and (@Code IS NULL OR sc.Code LIKE @Code + '%')
            and (@SellingNumberPlanID is null or @SellingNumberPlanID = sz.SellingNumberPlanID)
            and (@ZonesIDs  is null or sc.ZoneID in (select ZoneID from @ZonesIDsTable))
		END
	SET NOCOUNT OFF
END