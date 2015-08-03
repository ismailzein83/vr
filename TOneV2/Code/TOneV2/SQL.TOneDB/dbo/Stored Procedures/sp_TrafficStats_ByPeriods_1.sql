

CREATE PROCEDURE [dbo].[sp_TrafficStats_ByPeriods]
@PeriodType varchar(50) = 'Days',
@ZoneID INT = NULL ,
@SupplierID VarChar(15) = NULL ,
@CustomerID Varchar(15) = NULL ,
@CodeGroup varchar(10) = NULL ,
@CarrierGroupID INT = NULL,
@SupplierCarrierGroupID INT = NULL,
@FromDate DATETIME ,
@TillDate DATETIME,
@AllAccounts VARCHAR(MAX) = NULL,
@From INT = 1,
@To INT = 10,
@TableName nvarchar(255)

AS
DECLARE @SQLString nvarchar(4000)
	declare @tempTableName nvarchar(1000)
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
	
if(@From=1)
BEGIN 
SET NOCOUNT ON

	DECLARE @query varchar(max)
    DECLARE @CarrierGroupPath VARCHAR(255)
    SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @CarrierGroupID
   
    DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
   
    IF @CarrierGroupPath IS NULL
        INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
    ELSE
        INSERT INTO @FilteredCustomers
        SELECT DISTINCT ca.CarrierAccountID
        FROM CarrierAccount ca WITH(NOLOCK)
        LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
        WHERE ca.IsDeleted = 'N'
          AND cg.[Path] LIKE (@CarrierGroupPath + '%')
               
    DECLARE @SupplierCarrierGroupPath VARCHAR(255)
    SELECT @SupplierCarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @SupplierCarrierGroupID
   
    DECLARE @FilteredSuppliers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
   
    IF @SupplierCarrierGroupPath IS NULL
        INSERT INTO @FilteredSuppliers 
        SELECT ca.CarrierAccountID 
        FROM CarrierAccount ca 
        WHERE ca.IsDeleted = 'N'
    ELSE
        INSERT INTO @FilteredSuppliers
            SELECT DISTINCT ca.CarrierAccountID
            FROM CarrierAccount ca WITH(NOLOCK)
            LEFT JOIN CarrierGroup cg WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
            WHERE ca.IsDeleted = 'N' 
             AND  cg.[Path] LIKE (@SupplierCarrierGroupPath + '%')

if @ZoneID is null and @CodeGroup is null
Begin
	;WITH
	 FilteredCustomers AS
	 (
		 SELECT  CarrierAccountID FROM @FilteredCustomers
		 )
	 , FilteredSuppliers AS
	 (
		SELECT CarrierAccountID FROM @FilteredSuppliers
	 )
	 , Traffic_Stats_Data AS (
		SELECT 
		        
		        ts.CustomerID AS CustomerID
		       , ts.OurZoneID AS OurZoneID
		       , ts.SupplierID AS SupplierID
		       , ts.SupplierZoneID AS SupplierZoneID
		       , ts.Attempts AS Attempts
		       , ts.DeliveredAttempts AS DeliveredAttempts 
		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
		       , ts.SuccessfulAttempts AS SuccessfulAttempts
		       , ts.DurationsInSeconds AS DurationsInSeconds
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		 FROM TrafficStats ts WITH(NOLOCK,index(IX_TrafficStats_DateTimeFirst))  
		 WHERE FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @TillDate
	       AND ((@CustomerID IS NOT NULL AND ts.CustomerID = @CustomerID) OR (@AllAccounts IS NOT NULL AND ts.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@CustomerID IS NULL AND @AllAccounts IS NULL))
	       AND  ((@SupplierID IS NOT NULL AND ts.SupplierID = @SupplierID) OR (@AllAccounts IS NOT NULL AND ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@SupplierID IS NULL AND @AllAccounts IS NULL))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)   
	       AND (@CarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
		   AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
	 ),
	 Traffic AS (
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			Sum(Attempts) as Attempts ,
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/60.0) as DurationsInMinutes,          
			case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
			case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM Traffic_Stats_Data ts 
		GROUP BY
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
			END
			)
		
		SELECT * INTO #RESULT FROM Traffic 
		ORDER BY [Day]
		
		set @SQLString = '
			select R.*, (Attempts-SuccessfulAttempts) as FailedAttempts
			INTO ' + @tempTableName + ' From #RESULT R
			'
			--select * from ' + @tempTableName
		
	--	execute sp_executesql @SQLString
	End
Else
Begin
	 ;WITH
      FilteredCustomers AS
     (
         SELECT  CarrierAccountID FROM @FilteredCustomers
         )
     , FilteredSuppliers AS
     (
        SELECT CarrierAccountID FROM @FilteredSuppliers
     )
     , Traffic_Stats_Data AS (
		SELECT   ts.CustomerID AS CustomerID
		       , ts.OurZoneID AS OurZoneID
		       , ts.SupplierID AS SupplierID
		       , ts.SupplierZoneID AS SupplierZoneID
		       , ts.Attempts AS Attempts
		       , ts.DeliveredAttempts AS DeliveredAttempts 
		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
		       , ts.SuccessfulAttempts AS SuccessfulAttempts
		       , ts.DurationsInSeconds AS DurationsInSeconds
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  
		 WHERE FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @TillDate
	       AND ((@CustomerID IS NOT NULL AND ts.CustomerID = @CustomerID) OR (@AllAccounts IS NOT NULL AND ts.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@CustomerID IS NULL AND @AllAccounts IS NULL))
	       AND ((@SupplierID IS NOT NULL AND ts.SupplierID = @SupplierID) OR (@AllAccounts IS NOT NULL AND ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@SupplierID IS NULL AND @AllAccounts IS NULL))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	       AND (@CarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
		   AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
	 ),
	Traffic AS (
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			Sum(Attempts) as Attempts ,
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/60.0) as DurationsInMinutes,          
			case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
			case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM Traffic_Stats_Data ts 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE (@ZoneID IS NULL OR ts.OurZoneID= @ZoneID) 
		  AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		 GROUP BY
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
			END
			)
		SELECT * INTO #RESULT1 FROM Traffic 
		ORDER BY [Day]
		
		set @SQLString = '
			select R.*, (R.Attempts-R.SuccessfulAttempts) as FailedAttempts
			INTO ' + @tempTableName + ' From #RESULT1 R
		'
	End
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