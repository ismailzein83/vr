CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierCode_CreateTempByFiltered]
(
	@TempTableName varchar(200),	
	@Code varchar(20),
	@ZonesIDs varchar(max),
	@EffectiveOn DateTime
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN

		DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)

			 SELECT     rate.[ID]
			          , rate.[Code]
                      , rate.[ZoneID]
					  , rate.[CodeGroupID]
                      , rate.[BED]
                      , rate.[EED]
             into #Result
             FROM [TOneWhS_BE].[SupplierCode] rate 

			 WHERE (@Code IS NULL OR rate.Code like '%'+@Code+'%')
			and (@ZonesIDs  is null or rate.ZoneID in (select ZoneID from @ZonesIDsTable))
			 AND   (@EffectiveOn is null or (rate.BED < = @EffectiveOn   and (rate.EED is null or rate.EED  > @EffectiveOn) ));
           
			
						
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END