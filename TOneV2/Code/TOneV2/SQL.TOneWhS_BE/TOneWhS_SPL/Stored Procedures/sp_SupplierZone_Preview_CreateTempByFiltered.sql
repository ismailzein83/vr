CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZone_Preview_CreateTempByFiltered]
	@TempTableName varchar(200),
	@PriceListId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT 
			   [Name]
			  ,[ChangeType]
			  ,[BED]
			  ,[EED]
		    INTO #RESULT
		    FROM [TOneWhS_SPL].[SupplierZone_Preview]
			WHERE [SupplierZone_Preview].PriceListId = @PriceListId				
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END