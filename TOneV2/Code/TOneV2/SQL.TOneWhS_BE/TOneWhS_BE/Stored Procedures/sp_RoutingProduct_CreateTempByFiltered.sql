CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_CreateTempByFiltered]
	@TempTableName varchar(200),
	@RoutingProductName nvarchar(255)=null,
	@SaleZonePackageIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			
		DECLARE @SaleZonePackageIdsTable TABLE (SaleZonePackageId int)
		INSERT INTO @SaleZonePackageIdsTable (SaleZonePackageId)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SaleZonePackageIds)
			SELECT
				  [ID]
				  ,[Name]
				  ,[SaleZonePackageID]
				  ,[Settings]
			INTO #RESULT
			FROM TOneWhS_BE.RoutingProduct                           
            WHERE  
				(@RoutingProductName is Null or Name = @RoutingProductName)
                AND (@SaleZonePackageIds is Null or SaleZonePackageID IN (SELECT SaleZonePackageId FROM @SaleZonePackageIdsTable))
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END