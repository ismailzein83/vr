CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_CreateTempByFiltered]
	@TempTableName varchar(200),
	@RoutingProductName nvarchar(255)=null,
	@SellingNumberPlanIDs varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			
		DECLARE @SellingNumberPlanIDsTable TABLE (SellingNumberPlanID int)
		INSERT INTO @SellingNumberPlanIDsTable (SellingNumberPlanID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SellingNumberPlanIDs)
			SELECT	[ID],[Name],SellingNumberPlanID,[Settings]
			INTO #RESULT
			FROM TOneWhS_BE.RoutingProduct WITH(NOLOCK)                        
            WHERE  
				(@RoutingProductName is Null or Name = @RoutingProductName)
                AND (@SellingNumberPlanIDs is Null or SellingNumberPlanID IN (SELECT SellingNumberPlanID FROM @SellingNumberPlanIDsTable))
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END