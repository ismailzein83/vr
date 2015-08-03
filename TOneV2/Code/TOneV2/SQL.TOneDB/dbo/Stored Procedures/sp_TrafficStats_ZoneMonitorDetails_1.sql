




CREATE PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitorDetails]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
	@GroupingField varchar(10)='CUSTOMERS',
    @OurZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = NULL,
	@Port VARCHAR(1)=null,
    @PortInValue VARCHAR(21)= NULL,
    @PortOutValue VARCHAR(21)= NULL,
    @CustomerCarrierGroupID INT = NULL,
    @SupplierCarrierGroupID INT = NULL,
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @RecordCount INT OUTPUT,
    @TotalAttempt INT Output,
    @TotalDurations NUMERIC(19,6) OUTPUT
	
AS
	SET NOCOUNT ON
	
	IF @CustomerID IS NOT NULL AND @GroupingField IS NULL SET @GroupingField = 'SUPPLIERS'
	IF @SupplierID IS NOT NULL AND @GroupingField IS NULL SET @GroupingField = 'CUSTOMERS'
	
	
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
			AND cg.[Path] LIKE ('%' + @CarrierGroupPath + '%')
						
						
						
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
			ca.IsDeleted = 'N' AND
			cg.[Path] LIKE (@SupplierCarrierGroupPath + '%')
						
						
						
	
if @CustomerID is null and @SupplierID IS NULL
BEGIN
--PRINT 1
    select 
		 ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end
		END as ASR,
		
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end
		END as DeliveredASR,

 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
       1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls  - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    INTO #RESULT
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
         LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
		 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
		 LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR (TS.SwitchID = @SwitchID AND CA.RepresentsASwitch='N' ))
	--	AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND (@CustomerCarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM @FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
		 AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
	
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc
	SELECT @RecordCount = COUNT(*)
	FROM #RESULT
				
	SELECT @TotalAttempt = SUM(Attempts)
	FROM #RESULT
				
	SELECT @TotalDurations = SUM(DurationsInMinutes)
	FROM #RESULT
				
	SELECT * from #RESULT
	WHERE rowNumber  between (@PageIndex -1)* @PageSize +1 AND ((( @PageIndex -1) * @PageSize + 1) + @PageSize ) -1
				
	DROP TABLE #RESULT	
END
if @CustomerID is NOT null and @SupplierID IS NULL
BEGIN
--PRINT 2
--PRINT @GroupingField
    select 
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end
		 END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end
		 END as DeliveredASR,


		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    INTO #RESULT1
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.CustomerID = @CustomerID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND (@CustomerCarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM @FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
		 AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc
	SELECT @RecordCount = COUNT(*)
	FROM #RESULT1
				
	SELECT @TotalAttempt = SUM(Attempts)
	FROM #RESULT1
				
	SELECT @TotalDurations = SUM(DurationsInMinutes)
	FROM #RESULT1
				
	SELECT * from #RESULT1
	WHERE rowNumber  between (@PageIndex -1)* @PageSize +1 AND ((( @PageIndex -1) * @PageSize + 1) + @PageSize ) -1
				
	DROP TABLE #RESULT1
END
if @CustomerID is null and @SupplierID IS NOT NULL
BEGIN
--PRINT 3
    select 
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID ELSE TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end
		 END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		

		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end
		 END as DeliveredASR,

		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    INTO #RESULT2
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
		--AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND (@CustomerCarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM @FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
		 AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE Ts.SupplierID END)
	ORDER by Attempts desc
	SELECT @RecordCount = COUNT(*)
	FROM #RESULT2
				
	SELECT @TotalAttempt = SUM(Attempts)
	FROM #RESULT2
				
	SELECT @TotalDurations = SUM(DurationsInMinutes)
	FROM #RESULT2
				
	SELECT * from #RESULT2
	WHERE rowNumber  between (@PageIndex -1)* @PageSize +1 AND ((( @PageIndex -1) * @PageSize + 1) + @PageSize ) -1
				
	DROP TABLE #RESULT2
END
if @CustomerID is NOT null and @SupplierID IS NOT NULL
BEGIN
--PRINT 4
--PRINT @GroupingField
    select 
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' then case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end
		 END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		

		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END 
		ELSE case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end
		 END as DeliveredASR,

		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
		1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    INTO #RESULT3
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
			 LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID		
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (TS.CustomerID = @CustomerID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
		AND (@CustomerCarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM @FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
		 AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc
	SELECT @RecordCount = COUNT(*)
	FROM #RESULT3
				
	SELECT @TotalAttempt = SUM(Attempts)
	FROM #RESULT3
				
	SELECT @TotalDurations = SUM(DurationsInMinutes)
	FROM #RESULT3
				
	SELECT * from #RESULT3
	WHERE rowNumber  between (@PageIndex -1)* @PageSize +1 AND ((( @PageIndex -1) * @PageSize + 1) + @PageSize ) -1
				
	DROP TABLE #RESULT3
END
set @TotalAttempt =ISNULL(@TotalAttempt,0)
set @TotalDurations=ISNULL(@TotalDurations,0)