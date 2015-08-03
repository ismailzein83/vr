CREATE PROCEDURE [dbo].[Bp_GetTestingNumbers] 
	 @Top          INT = 100,
	 @Fromdate     datetime,
	 @ToDate       datetime,
	 @SupplierID   varchar(20)=Null,
	 @ZoneID       INT = Null,
	 @Code		   varchar(50) = NULL,
	 @MaxDuration  decimal = 3.0,
	 @From INT = 0,
     @To INT = 10,
     @TableName nvarchar(255)
	 --@PageIndex INT = 1,
	 --@PageSize INT = 10,
	 --@RecordCount INT OUTPUT
AS
BEGIN
	DECLARE @SQLString nvarchar(4000)
	declare @tempTableName nvarchar(255)
	declare @PageIndexTemp bit
	declare @exists bit
	
	set @SQLString=''
	set @tempTableName='tempdb.dbo.['+@TableName + ']'
	set @exists=dbo.CheckGlobalTableExists (@TableName)
	
	if(@From=1 and  @exists=1)
	begin
		declare @DropTable varchar(100)
		set @DropTable='Drop table ' + @tempTableName
		exec(@DropTable)
	end

	if(@from = 1)
	BEGIN
	SELECT  
		TOP (@TOP)
		--ROW_NUMBER() OVER ( ORDER BY   Count(BM.Attempt) DESC) AS rowNumber,
		Z.[Name] as [Zone],
		cdpn,
		BM.CustomerID as CustomerID,
		Count(*) AS Attempts,
		CONVERT(DECIMAL(10,2),Min(BM.DurationInSeconds) / 60.0) AS MinDuration,
		CONVERT(DECIMAL(10,2),Max(BM.DurationInSeconds) / 60.0) AS MaxDuration,
		CONVERT(DECIMAL(10,2),Sum(DurationInSeconds/60.0)) as DurationsInMinutes
	INTO #RESULT
	FROM dbo.Billing_CDR_Main BM with(nolock) 
		JOIN Zone Z WITH(nolock) ON BM.OurZoneID = Z.ZoneID
	WHERE
		Attempt BETWEEN @FromDate AND @ToDate
		AND (@SupplierID IS NULL OR BM.SupplierID = @SupplierID)
		AND (@ZoneID IS NULL OR BM.OurZoneID = @ZoneID)					
		AND (@Code IS NULL OR BM.CDPN LIKE @Code + '%')					
		AND (DurationInSeconds > (@MaxDuration * 60.0))
	GROUP BY BM.CDPN, Z.Name, BM.CustomerID
	ORDER BY Count(BM.Attempt) DESC
	
	SET @SQLString = '
		DECLARE @ShowNameSuffix nvarchar(1)
		SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
		;WITH SupplierTables as 
		(
			SELECT 
				( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
		)
		SELECT R.*, S.SupplierName as Customer INTO  ' + @tempTableName + ' FROM #RESULT R
		LEFT JOIN SupplierTables S ON R.CustomerID = S.SupplierID
		select count(1) from ' + @tempTableName + '
	    ;with FINAL AS 
        (
			select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
			from ' + @tempTableName + ' 
        )
		SELECT * FROM FINAL WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar)
						
	execute sp_executesql @SQLString
	
--OPTION (recompile)
--	SELECT @RecordCount = COUNT(*)
--	FROM #RESULT
	
--	SELECT * FROM #RESULT
--	WHERE RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1 AND (((@PageIndex -1) * @PageSize + 1) + @PageSize) -1
--	DROP TABLE #RESULT
	END
END