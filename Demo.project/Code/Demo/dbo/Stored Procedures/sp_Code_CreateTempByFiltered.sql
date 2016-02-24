
CREATE PROCEDURE [dbo].[sp_Code_CreateTempByFiltered]
	@TempTableName varchar(200),
	@EffectiveOn dateTime = null,
	@ZonesIDs varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [dbo].[ParseStringList](@ZonesIDs)
			SELECT  
			   sc.[ID]
			  ,sc.[Code]
			  ,sc.[ZoneID]
			  ,sc.[CodeGroupID]
			  ,sc.[BED]
			  ,sc.[EED]
			INTO #RESULT
			FROM 
			dbo.Code sc 
			join dbo.Zone sz on  sc.ZoneID = sz.ID   
            WHERE 
                (@EffectiveOn is null or sc.BED < = @EffectiveOn)
            and (@EffectiveOn is null or sc.EED is null or sc.EED  > @EffectiveOn)        
            and (@ZonesIDs  is null or sc.ZoneID in (select ZoneID from @ZonesIDsTable))
            DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END