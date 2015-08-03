-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_TrafficStats_OriginatingZoneMonitorDetails] 
	-- Add the parameters for the stored procedure here
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
	@GroupingField varchar(10)='CUSTOMERS',
    @OriginatingZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = NULL,
	@ourzoneid VARCHAR(20)=null,
	@CustomerCarrierGroupID varchar(10) = NULL,
	@SupplierCarrierGroupID varchar(10) = NULL,   
	@PageIndex INT = 1,
    @PageSize INT = 10,
    @RecordCount INT OUTPUT,
    @TotalAttempt INT Output,
    @TotalDurations NUMERIC(19,6) OUTPUT
   
	
AS
	SET NOCOUNT ON
	--DECLARE @originatingzoneid INT
	--SET @originatingzoneid=@OurZoneID			
	IF @CustomerID IS NOT NULL SET @GroupingField = 'SUPPLIERS'
	IF @SupplierID IS NOT NULL SET @GroupingField = 'CUSTOMERS'
	
	
	 DECLARE @CustomerCarrierGroupPath VARCHAR(255)
	SELECT @CustomerCarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @CustomerCarrierGroupID
	
	DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @CustomerCarrierGroupPath IS NULL OR @CustomerCarrierGroupID IS NULL
		INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredCustomers 
			SELECT DISTINCT ca.CarrierAccountID 
				FROM CarrierAccount ca WITH(NOLOCK)
				LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
			WHERE
					ca.IsDeleted = 'N'
				AND cg.[Path] LIKE (@CustomerCarrierGroupPath + '%')
				
				
				
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
    select
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,

 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(Calldate) as LastAttempts,
        1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls  - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
	INTO #RESULT
       FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
         LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
		 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
		 LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID	
WHERE  Calldate >= @FromDateTime  AND   Calldate <= @ToDateTime
		AND (@OriginatingZoneID IS NULL OR TS.OriginatingZoneID = @OriginatingZoneID)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL ) OR (TS.SwitchID = @SwitchID AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) ))
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND (@CustomerCarrierGroupID IS NULL OR EXISTS (SELECT FC.CarrierAccountID FROM @FilteredCustomers FC WHERE FC.CarrierAccountID = TS.CustomerID))
		and (@SupplierCarrierGroupID IS NULL OR EXISTS (SELECT FS.CarrieraccountID FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = TS.SupplierID))
	
	
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
    select 		
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,


		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(Calldate) as LastAttempts,
        1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
	INTO #RESULT1
     FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID	
WHERE  Calldate >= @FromDateTime  AND   Calldate <= @ToDateTime
		AND (TS.CustomerID = @CustomerID) 
		AND (@OriginatingZoneID IS NULL OR TS.OriginatingZoneID = @OriginatingZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND (@CustomerCarrierGroupID IS NULL OR EXISTS (SELECT FC.CarrierAccountID FROM @FilteredCustomers FC WHERE FC.CarrierAccountID = TS.CustomerID))
		and (@SupplierCarrierGroupID IS NULL OR EXISTS (SELECT FS.CarrieraccountID FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = TS.SupplierID))
	
	AND ((@SwitchID IS NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) ) OR TS.SwitchID = @SwitchID)
	
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
    select 
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID ELSE TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		

		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,

		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(Calldate) as LastAttempts,
        1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
	INTO #RESULT2
       FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID	
WHERE  Calldate >= @FromDateTime  AND   Calldate <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (@OriginatingZoneID IS NULL OR TS.OriginatingZoneID = @OriginatingZoneID)
		AND (@CustomerCarrierGroupID IS NULL OR EXISTS (SELECT FC.CarrierAccountID FROM @FilteredCustomers FC WHERE FC.CarrierAccountID = TS.CustomerID))
		and (@SupplierCarrierGroupID IS NULL OR EXISTS (SELECT FS.CarrieraccountID FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = TS.SupplierID))
	
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	
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
    select 
		ROW_NUMBER() OVER (ORDER BY Sum(Attempts) DESC) AS rowNumber,
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		

		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,

		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(Calldate) as LastAttempts,
		1 as OptionalValue,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
	INTO #RESULT3
    FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
			 LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID		
WHERE  Calldate >= @FromDateTime  AND   Calldate <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (TS.OriginatingZoneID = @OriginatingZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND (@CustomerCarrierGroupID IS NULL OR EXISTS (SELECT FC.CarrierAccountID FROM @FilteredCustomers FC WHERE FC.CarrierAccountID = TS.CustomerID))
		and (@SupplierCarrierGroupID IS NULL OR EXISTS (SELECT FS.CarrieraccountID FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = TS.SupplierID))
	
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc )) OR TS.SwitchID = @SwitchID)
	
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