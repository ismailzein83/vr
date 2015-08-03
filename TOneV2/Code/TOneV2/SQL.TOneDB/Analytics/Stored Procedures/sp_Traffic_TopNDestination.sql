

CREATE PROCEDURE [Analytics].[sp_Traffic_TopNDestination]
 @TopRecord    INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @sortOrder   VARCHAR(25) = 'DESC',
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25) = NULL,
 @SwitchID	 varchar(50) = NULL,
 @GroupByCodeGroup char(1) = 'N',
 @CodeGroup varchar(10) = NULL,
 @ShowSupplier Char(1) = 'N',
 @OrderTarget VARCHAR(10) = 'Quantity',
 @From int = 1,
 @To int = 10,
 @TableName nvarchar(255)
AS

DECLARE @Sql nvarchar(4000)
	declare @tempTableName nvarchar(1000)
	declare @PageIndexTemp bit
	declare @exists bit
	SET @todate=DATEADD(dd,1,@todate)
	set @Sql=''
	set @tempTableName='tempdb.dbo.['+@TableName + ']'
	set @exists=dbo.CheckGlobalTableExists (@TableName)
	
	if(@From=1 and  @exists=1)
	begin
		declare @DropTable varchar(100)
		set @DropTable='Drop table ' + @tempTableName
		exec(@DropTable)
	end
if(@From = 1)
BEGIN
	--SET ROWCOUNT @TopRecord

    SET @SwitchID =isnull(@SwitchID,'')
	SET NOCOUNT ON

	Declare @FromDateStr varchar(50)
	Declare @ToDateStr  varchar(50)

	SELECT @FromDateStr = CONVERT(varchar(50), @FromDate, 121)
	SELECT @ToDateStr = CONVERT(varchar(50), @ToDate, 121)

	
	SET @Sql = 'SELECT '

	IF @GroupByCodeGroup = 'Y'
	BEGIN
		SET @Sql = @Sql + 'CG.Code AS OurZoneID, CG.Code AS Code, CG.[Name] AS [Name], '
	END
	ELSE
	BEGIN
		SET @Sql = @Sql + 'S.OurZoneID, Z.[Name] AS [Name], '
	END
	SET @Sql = @Sql + '
	CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					CASE WHEN ''' + ISNULL(@SupplierID,'') + ' '' = '''' THEN SUM(S.NumberOfCalls) ELSE SUM(S.Attempts) END AS Attempts,
					CONVERT(DECIMAL(10,2),SUM(DurationsInSeconds) / 60.0) as DurationsInMinutes,
					CASE WHEN ''' + ISNULL(@SupplierID,'') + ' '' = '''' then (case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls)) ELSE 0 end ) else (case when Sum(S.Attempts) > 0 then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(S.Attempts)) ELSE 0 end ) end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else 0 end as ACD,'+
					
					
					
				--	CASE WHEN ''' + ISNULL(@SupplierID,'') + ' '' = '''' then ( case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(NumberOfCalls)) ELSE 0 end ) else ( case when Sum(S.Attempts) > 0 then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(S.NumberOfCalls)) else 0 end ) end as DeliveredASR,
					
					
					+'CASE WHEN 
                        (Sum(Attempts)) > 0 then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts)*100.00 / sum(Attempts))
                             when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0  then CONVERT(DECIMAL(10,2),Sum(DeliveredNumberOfCalls)*100.0 /  (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)))
                        ELSE 0  END as DeliveredASR,
				    CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD
				    INTO #RESULT 
					FROM TrafficStats S WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID'
	
	IF @GroupByCodeGroup = 'Y'
	BEGIN
		SET @Sql = @Sql + ' join CodeGroup CG ON Z.CodeGroup = CG.Code'
	END
	
	SET @Sql = @Sql + ' LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON S.CustomerID = CA.CarrierAccountID
					WHERE 
						(
							FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
						)'
					--  AND CA.RepresentsASwitch=''' + 'N' + '''   '
	
	IF @CodeGroup IS NOT NULL
	BEGIN
		SET @Sql = @Sql + ' AND Z.CodeGroup = ' + @CodeGroup
	END	
	
	IF @SupplierID IS NOT NULL
	BEGIN
		SET @Sql = @Sql + ' AND S.SupplierID =''' + @SupplierID + ''''
	END
	
	IF @CustomerID IS NOT NULL
	BEGIN
		SET @Sql = @Sql + ' AND S.CustomerID =''' + @CustomerID + ''''
	END
	
	SET @Sql = @Sql + ' GROUP BY '
	
	IF @GroupByCodeGroup = 'Y'
		BEGIN
			SET @Sql = @Sql + 'CG.Code, CG.[Name], '
		END
	ELSE
		BEGIN
			SET @Sql = @Sql + 'S.OurZoneID, Z.[Name], '
		END
	IF @OrderTarget = 'Quantity'
	BEGIN
		SET @Sql = @Sql + ' CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
						 Attempts DESC'
	END
	ELSE
	BEGIN
		SET @Sql = @Sql + ' CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
						 ASR DESC'
	END
	
	set @Sql = @Sql + '
		select R.*
	'		
	if(@GroupByCodeGroup = 'Y')
		set @Sql = @Sql + ',(R.Name + '' ('' + R.Code + '')'') as CodeGroupName '
	else
		set @sql =@sql + ','''' as CodeGroupName '
	IF @OrderTarget = 'Quantity'
		BEGIN	
			set @sql =  @sql + ' into ' + @tempTableName + ' from #RESULT R order by attempts ' + @sortOrder + ' , DurationsInMinutes desc'
		END
	ELSE
		BEGIN
			set @sql =  @sql + ' into ' + @tempTableName + ' from #RESULT R WHERE  ATTEMPTS>1000 order by  DeliveredASR  ' + @sortOrder +',ACD DESC '
		END
	END 
	
	set @Sql = @Sql +
	--' select count(1) from ' + @tempTableName + ' 
	'
	;with final as ( select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber from ' + @tempTableName + ')'
	
	if(@TopRecord IS NULL)
		set @Sql = @Sql + ' select * from final WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar)	
	ELSE
		set @Sql = @Sql + ' select top ' + CAST( @TopRecord as varchar) + '* from final WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar)-- + 'Order by Attempts ' + @sortOrder
	PRINT @Sql		 
	execute sp_executesql @Sql