





create PROCEDURE [dbo].[SP_TrafficStats_TopNDestination_current]
 @TopRecord    INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @sortOrder   VARCHAR(25) = 'DESC',
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25) = NULL,
 @SwitchID     varchar(50) = NULL,
 @CarrierGroupID varchar(5) = NULL,
 @SupplierCarrierGroupID varchar(5) = NULL,
 @GroupByCodeGroup char(1) = 'N',
 @CodeGroup varchar(10) = NULL,
 @ShowSupplier Char(1) = 'N',
 @From int = 1,
 @To int = 10,
 @TableName nvarchar(255)
AS

DECLARE @Sql nvarchar(max)
    declare @tempTableName nvarchar(1000)
    declare @PageIndexTemp bit
    declare @exists bit
   
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
    SET ROWCOUNT @TopRecord

--    Set @CustomerID = ISNULL(@CustomerID, '')
--    Set @SupplierID = ISNULL(@SupplierID, '')
    SET @SwitchID =isnull(@SwitchID,'')
    SET NOCOUNT ON

    Declare @FromDateStr varchar(50)
    Declare @ToDateStr  varchar(50)

    SELECT @FromDateStr = CONVERT(varchar(50), @FromDate, 121)
    SELECT @ToDateStr = CONVERT(varchar(50), @ToDate, 121)

--    DECLARE @Sql varchar(8000)
SET @Sql =
'DECLARE @CarrierGroupPath VARCHAR(255)
            SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = ' +convert(varchar(5),ISNULL(@CarrierGroupID,'0')) +'
          
            DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
          
            IF @CarrierGroupPath IS NULL
                INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = ''N''
            ELSE
                INSERT INTO @FilteredCustomers
                    SELECT DISTINCT ca.CarrierAccountID
                        FROM CarrierAccount ca WITH(NOLOCK)
                        LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,'',''))
                    WHERE
                            ca.IsDeleted = ''N''
                        AND cg.[Path] LIKE (@CarrierGroupPath + ''%'')
                       
DECLARE @SupplierCarrierGroupPath VARCHAR(255)
            SELECT @SupplierCarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = ' +convert(varchar(5),ISNULL(@SupplierCarrierGroupID,'0')) +'
          
            DECLARE @FilteredSuppliers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
          
            IF @SupplierCarrierGroupPath IS NULL
                INSERT INTO @FilteredSuppliers SELECT ca.CarrierAccountID FROM CarrierAccount ca where ca.IsDeleted = ''N''
            ELSE
                INSERT INTO @FilteredSuppliers
                    SELECT DISTINCT ca.CarrierAccountID
                    FROM CarrierAccount ca WITH(NOLOCK)
                    LEFT JOIN CarrierGroup cg WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,'',''))
                    WHERE
                         ca.IsDeleted = ''N'' AND
                         cg.[Path] LIKE (@SupplierCarrierGroupPath + ''%'')
                        ;WITH FilteredCustomers AS
                (
                 SELECT CarrierAccountID FROM @FilteredCustomers
                ),
                FilteredSuppliers AS
                (
                SELECT CarrierAccountID FROM @FilteredSuppliers
                ) '
    SET @Sql = @Sql+ 'SELECT '
    --SET @Sql = @Sql + 'ROW_NUMBER() OVER (ORDER BY SUM(DurationsInSeconds) / 60.0 '+ @sortOrder + ',
    --                         Sum(NumberOfCalls) DESC) AS RowNumber, '
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
                    case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
                    CASE WHEN ''' + ISNULL(@SupplierID,'') + ' '' = '''' then ( case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(NumberOfCalls)) ELSE 0 end ) else ( case when Sum(S.Attempts) > 0 then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(S.Attempts)) else 0 end ) end as DeliveredASR,
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
                        )
                        and(('''+@SwitchID+ '''=''''  AND CustomerID IS NOT NULL AND CA.RepresentsASwitch=''' + 'N' + ''' )   OR S.SwitchID ='''+@SwitchID +''')'
   
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
    if @CarrierGroupID IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND (s.CustomerID IN (SELECT CarrierAccountID FROM FilteredCustomers)) '
    END
    IF @SupplierCarrierGroupID IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND (s.SupplierID IN (SELECT CarrierAccountID FROM FilteredSuppliers)) '
    END
    SET @Sql = @Sql + ' GROUP BY '
   
    IF @GroupByCodeGroup = 'Y'
    BEGIN
        SET @Sql = @Sql + 'CG.Code, CG.[Name], '
    END
    ELSE
    BEGIN
        SET @Sql = @Sql + 'S.OurZoneID, Z.[Name] '
    END
        SET @Sql = @Sql + ' ,CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END
                        ORDER BY DurationsInMinutes '+ @sortOrder + ',
                             Attempts DESC'

   
    set @Sql = @Sql + '
        select R.*
    '       
    if(@GroupByCodeGroup = 'Y')
        set @Sql = @Sql + ',(R.Name + '' ('' + R.Code + '')'') as CodeGroupName '
    else
        set @sql =@sql + ','''' as CodeGroupName '
    set @sql =  @sql + ' into ' + @tempTableName + ' from #RESULT R order by attempts desc'
    END
   
    set @Sql = @Sql +
    ' select count(1) from ' + @tempTableName + '
    ;with final as ( select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber from ' + @tempTableName + ')
    select * from final WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar)   
    --PRINT @Sql         
    execute sp_executesql @Sql