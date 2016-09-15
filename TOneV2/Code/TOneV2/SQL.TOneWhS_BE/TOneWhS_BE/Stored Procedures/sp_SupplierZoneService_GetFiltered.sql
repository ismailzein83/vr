CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_GetFiltered]	
	@SupplierID int ,
	@ZonesIDs varchar(max),
	@EffectiveOn dateTime = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)
			SELECT  supzs.ID,
					supzs.ZoneID,
					supzs.ReceivedServicesFlag,
					supzs.EffectiveServiceFlag,
					supzs.BED,
					supzs.EED
			FROM [TOneWhS_BE].SupplierZoneService supzs WITH(NOLOCK) 
			inner join  [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) on  supzs.ZoneID = sz.ID   
            WHERE 
                (@EffectiveOn is null or supzs.BED < = @EffectiveOn)
            and (@EffectiveOn is null or supzs.EED is null or supzs.EED  > @EffectiveOn)
            and (@SupplierID is null or @SupplierID = sz.SupplierID)
            and (@ZonesIDs  is null or supzs.ZoneID in (select ZoneID from @ZonesIDsTable))            
            and (supzs.BED <>supzs.EED  or  supzs.EED is null)
		END
	SET NOCOUNT OFF
END