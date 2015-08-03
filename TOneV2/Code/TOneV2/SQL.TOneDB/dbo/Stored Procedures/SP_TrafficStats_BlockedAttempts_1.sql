-- =============================================
-- Author: Ali Youness
-- =============================================
CREATE PROCEDURE [dbo].[SP_TrafficStats_BlockedAttempts]
    
    @FromDateTime  datetime,
	@ToDateTime    datetime,
    @CustomerID   varchar(10) = NULL,
    @OurZoneID 	  INT = NULL,
	@SwitchID	  tinyInt = NULL,
    @GroupByNumber CHAR(1) = 'N',
    @From INT = 0,
    @To INT = 10,
    @TableName nvarchar(255)
    
WITH RECOMPILE
AS

	DECLARE @SQLString nvarchar(4000)
	declare @tempTableName nvarchar(1000)
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

IF(@From = 1)
BEGIN 
SET NOCOUNT ON

if @CustomerID IS NULL
BEGIN
	SELECT 
		OurZoneID,
		Count (*) AS BlockAttempt, 
		ReleaseCode,
		--CASE WHEN @SwitchID IS NOT NULL then SwitchID ELSE '' END AS SwitchID,
		SwitchID,
		ReleaseSource,
		CustomerID,
		DATEADD(ms,-datepart(ms,Min(Attempt)),Min(Attempt)) AS FirstCall,
		DATEADD(ms,-datepart(ms,Max(Attempt)),Max(Attempt)) AS LastCall,
		case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	INTO #RESULT
	FROM  Billing_CDR_Invalid  WITH(NOLOCK)
	WHERE
			Attempt Between @FromDateTime And @ToDateTime
		AND DurationInSeconds = 0
		AND SupplierID IS NULL
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
	--GROUP BY ReleaseCode,CASE WHEN @SwitchID IS NOT NULL then SwitchID ELSE '' END,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	GROUP BY ReleaseCode,SwitchID,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	--GROUP BY ReleaseCode,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	ORDER BY Count (*) DESC
	
	SET @SQLString = '
	DECLARE @ShowNameSuffix nvarchar(1)
	SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
	;WITH
		SupplierTables as 
		(
			SELECT 
			( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
					,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
		)
		select R.*, Z.Name as Zone, C.SupplierName as Customer INTO ' + @tempTableName + ' FROM #RESULT R
		left join Zone Z on R.OurZoneID = Z.ZoneID
		left join SupplierTables C ON R.CustomerID = C.SupplierID
	'
	END
else
BEGIN
	SELECT  
		OurZoneID,
		Count (*) AS BlockAttempt, 
		ReleaseCode,
		--CASE WHEN @SwitchID IS NOT NULL then SwitchID ELSE '' END AS SwitchID,
		SwitchID,
		ReleaseSource,
		CustomerID,
		DATEADD(ms,-datepart(ms,MIN(Attempt)),MIN(Attempt)) AS FirstCall,
		DATEADD(ms,-datepart(ms,Max(Attempt)),Max(Attempt)) AS LastCall,
		case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	INTO #RESULT1
	FROM  Billing_CDR_Invalid  WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Customer))
	WHERE
			Attempt Between @FromDateTime And @ToDateTime
		AND CustomerID = @CustomerID
		AND DurationInSeconds = 0
		AND SupplierID IS NULL
		AND (@SwitchID IS NULL OR  SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR  OurZoneID = @OurZoneID)		
	--GROUP BY ReleaseCode,CASE WHEN @SwitchID IS NOT NULL then SwitchID ELSE '' END,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	GROUP BY ReleaseCode,SwitchID,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	--GROUP BY ReleaseCode,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	ORDER BY Count (*) DESC

	SET @SQLString = '
	DECLARE @ShowNameSuffix nvarchar(1)
	SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
	;WITH
		SupplierTables as 
		(
			SELECT 
			( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
					,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
		)
		select R.*, Z.Name as Zone, C.SupplierName as Customer INTO ' + @tempTableName + ' FROM #RESULT1 R
		left join Zone Z on R.OurZoneID = Z.ZoneID
		left join SupplierTables C ON R.CustomerID = C.SupplierID
	'
	
	END
	


 

END
	SET @SQLString = @SQLString + '
	select count(1) from ' + @tempTableName + '
	 ;with FINAL AS 
                        (
                        select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                        from ' + @tempTableName + ' 
                         )
						SELECT * FROM FINAL WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar)
						
	execute sp_executesql @SQLString