
CREATE PROCEDURE [dbo].[sp_TrafficStats_LiveMonitor]
	@PeriodType varchar(50) = 'Hours',
	@ZoneID INT = NULL ,
	@SupplierID VarChar(15) = NULL ,
	@CustomerID Varchar(15) = NULL ,
	@CodeGroup varchar(10) = NULL ,
	@FromDate DATETIME ,
	@TillDate DATETIME,
	@CompFromDate DATETIME,
	@CompToDate DATETIME,
	@CustomerCarrierGroupID INT = NULL,
	@SupplierCarrierGroupID INT = NULL
AS
BEGIN 


    DECLARE @CarrierGroupPath VARCHAR(255)
	SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @CustomerCarrierGroupID
	
	DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @CarrierGroupPath IS NULL
		INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredCustomers 
			SELECT DISTINCT ca.CarrierAccountID 
				FROM CarrierAccount ca WITH(NOLOCK)
				LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
			WHERE
					ca.IsDeleted = 'N'
				AND cg.[Path] LIKE (@CarrierGroupPath + '%')
	
	
	
	
	
	DECLARE @SupplierCarrierGroupPath VARCHAR(255)
	SELECT @SupplierCarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @SupplierCarrierGroupID
	
	DECLARE @FilteredSuppliers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @SupplierCarrierGroupPath IS NULL
		INSERT INTO @FilteredSuppliers SELECT ca.CarrierAccountID FROM CarrierAccount ca where ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredSuppliers
			SELECT DISTINCT ca.CarrierAccountID
			FROM CarrierAccount ca WITH(NOLOCK)
			LEFT JOIN CarrierGroup cg WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
			WHERE
					ca.IsDeleted = 'N'
				AND cg.[Path] LIKE (@SupplierCarrierGroupPath + '%')
	
	
	
	

	if @CustomerID is null and @SupplierID IS NULL
	BEGIN
		WITH FilteredCustomers AS 
	 (
	 	SELECT  CarrierAccountID FROM @FilteredCustomers
	 	)
	 	,FilteredSuppliers AS 
	 (
	 	SELECT  CarrierAccountID FROM @FilteredSuppliers
	 	)
	 	,Traffic_Stats_Data AS (
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
			FROM TrafficStats ts WITH(NOLOCK)  
			WHERE 
				((FirstCDRAttempt BETWEEN @FromDate AND @TillDate) OR (FirstCDRAttempt BETWEEN @CompFromDate AND @CompToDate))
				AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
				AND ts.SupplierID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	 ),
	 
	
	Traffic AS (
		 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)
			END AS [Date],
			MIN(FirstCDRAttempt) AS FirstCDRAttempt,
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM Traffic_Stats_Data TS WITH(NOLOCK) 
		LEFT 	JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
		WHERE 
		   (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			AND EXISTS  (SELECT * FROM FilteredCustomers Fc WHERE Fc.CarrierAccountID = Ts.CustomerID )
			AND EXISTS  (SELECT * FROM FilteredSuppliers Fc WHERE Fc.CarrierAccountID = Ts.SupplierID )
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)	
			END
			
			)
--		ORDER BY Min(FirstCDRAttempt) DESC 
SELECT * FROM Traffic 
  ORDER BY [Date]
END
if @CustomerID IS NOT NULL and @SupplierID IS NULL
BEGIN
with
	FilteredCustomers AS 
	 (
	 	SELECT  CarrierAccountID FROM @FilteredCustomers
	 	),
	Traffic_Stats_Data AS (
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
		 FROM TrafficStats ts WITH(NOLOCK)  
		 WHERE 
			((FirstCDRAttempt BETWEEN @FromDate AND @TillDate) OR (FirstCDRAttempt BETWEEN @CompFromDate AND @CompToDate))
			AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			--AND (TS.CustomerID=@CustomerID) 
	            
	 ),
	 
	Traffic AS (
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)
			END AS [Date],
			MIN(FirstCDRAttempt) AS FirstCDRAttempt,
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM Traffic_Stats_Data TS WITH(NOLOCK) 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			
		WHERE 
			
     	 ts.CustomerID = @CustomerID
			AND 
			(@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)	
			END
	)
	
	SELECT * FROM traffic
 ORDER BY [Date]
 END
if @CustomerID is null and @SupplierID IS not NULL
BEGIN
	with
	Traffic_Stats_Data AS (
		SELECT 
		        
		       
		        ts.OurZoneID AS OurZoneID
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
		 FROM TrafficStats ts WITH(NOLOCK)  WHERE 
		
	 ((FirstCDRAttempt BETWEEN @FromDate AND @TillDate) OR (FirstCDRAttempt BETWEEN @CompFromDate AND @CompToDate))
	   AND (ts.SupplierID = @SupplierID) AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc )      
	 ),
	 
	 Traffic AS (
	 
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			MIN(FirstCDRAttempt) AS FirstCDRAttempt,
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM Traffic_Stats_Data TS WITH(NOLOCK) 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
		WHERE 
     		 
			 (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)
			END
	
	 )
	 SELECT * FROM traffic
	 ORDER BY [Date]
END
if @CustomerID  is not null and @SupplierID IS not NULL
BEGIN
	 	with
	Traffic_Stats_Data AS (
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
		 FROM TrafficStats ts WITH(NOLOCK)  WHERE 
		
	  ((FirstCDRAttempt BETWEEN @FromDate AND @TillDate) OR (FirstCDRAttempt BETWEEN @CompFromDate AND @CompToDate))
	       --AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) 
	       --AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)    
	 ),
	 
	 Traffic AS (
	 
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)
			END AS [Date],
			MIN(FirstCDRAttempt) AS FirstCDRAttempt,
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM Traffic_Stats_Data TS WITH(NOLOCK) 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
		WHERE 
			
     		 (ts.CustomerID = @CustomerID) 
     		AND (ts.SupplierID = @SupplierID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			 AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) 
	       AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Hours' THEN CONVERT(varchar(13), TS.FirstCDRAttempt, 121)
				WHEN @PeriodType like 'Minutes' THEN CONVERT(varchar(16), TS.FirstCDRAttempt, 121)	
			END
		
)
	SELECT * FROM Traffic ORDER BY [Date] 
	END
END