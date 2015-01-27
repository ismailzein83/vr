







create PROCEDURE [dbo].[sp_TrafficStats_CarrierMonitordashboard]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME
	--@CustomerID   varchar(10) = NULL,
	--@SupplierID   varchar(10) = NULL,
 --   @OurZoneID 	  INT = NULL,
	--@SwitchID	  tinyInt = NULL, 
	--@GroupBySupplier CHAR(1) = 'N',
	--@IncludeCarrierGroupSummary CHAR(1) = 'N'
WITH recompile 
AS
BEGIN

	DECLARE @Results TABLE(
			CarrierAccountID VARCHAR(5),
			Attempts INT, 
			DurationsInMinutes NUMERIC(19,6),          
			ASR NUMERIC(19,6),
			ACD NUMERIC(19,6),
			DeliveredASR NUMERIC(19,6), 
			AveragePDD NUMERIC(19,6),
			MaxDuration NUMERIC(19,6),
			LastAttempt DATETIME,
			SuccessfulAttempts BIGINT,
			FailedAttempts BIGINT,
			DeliveredAttempts BIGINT,
			PDDInSeconds NUMERIC(19,6)
	)	
	
	SET rowcount 5

                 
	-- No Customer, No Supplier, GroupByCustomer
	--IF @CustomerID IS NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'N'
		INSERT INTO @Results
		SELECT	CustomerID As CarrierAccountID,
			Sum(NumberOfCalls) as Attempts,
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			case when Sum(NumberOfCalls) > 0 THEN Sum(deliveredNumberOfCalls) * 100.0 / SUM(NumberOfCalls) ELSE 0 end as DeliveredASR, 
			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
			SUM(TS.DeliveredNumberOfCalls) AS DeiveredAttempts,
			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
		FROM TrafficStats TS WITH(NOLOCK)
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
		LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
     WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			--AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' AND SupplierID IS NOT NULL AND CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
			Group By CustomerID
			ORDER BY Sum(DurationsInSeconds/60.) DESC
			
	-- No Customer, No Supplier, GroupBySupplier
--	IF @CustomerID IS NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'Y'
--		INSERT INTO @Results
--		SELECT	SupplierID As CarrierAccountID,
--			Sum(Attempts) as Attempts, 
--			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
--			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
--			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
--			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
--			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--			Max(LastCDRAttempt) as LastAttempt,
--			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
--			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
--			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
--			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 
--		FROM TrafficStats TS WITH(NOLOCK)
--		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
--		LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID

--			WHERE 
--				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--			AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
--			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' AND SupplierID IS NOT NULL AND CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
----			
--			Group By SupplierID
--			ORDER BY SUM(Attempts) DESC
			
--	-- Customer, No Supplier
--	ELSE IF (@CustomerID IS NOT NULL AND @SupplierID IS NULL) OR @GroupBySupplier = 'Y' 
--		INSERT INTO @Results
--		SELECT	SupplierID As CarrierAccountID,
--			Sum(Attempts) as Attempts, 
--			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
--			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
--			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
--			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
--			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--			Max(LastCDRAttempt) as LastAttempt,
--			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
--			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
--			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
--			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
--		FROM TrafficStats TS WITH(NOLOCK)
--		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
--		LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
			
--			WHERE 
--				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--			AND CustomerID = @CustomerID 
--			AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
--			--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR TS.SwitchID = @SwitchID)
--			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' AND SupplierID IS NOT NULL AND CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
--		    Group By SupplierID 
--			ORDER BY SUM(Attempts) DESC
--	-- No Customer, Supplier
--	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
--			INSERT INTO @Results
--			SELECT	CustomerID As CarrierAccountID,
--					Sum(Attempts) as Attempts, 
--					Sum(DurationsInSeconds/60.0) as DurationsInMinutes,          
--					CASE WHEN Sum(Attempts) > 0 THEN Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 END AS ASR,
--					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
--					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--					CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
--					MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--					Max(LastCDRAttempt) as LastAttempt,
--					Sum(SuccessfulAttempts)AS SuccessfulAttempts,
--					Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
--					SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
--					SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 
--				FROM TrafficStats TS WITH(NOLOCK)
--				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
--				LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID

--					WHERE 
--						FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--					AND SupplierID = @SupplierID 
--					AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
--					--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR TS.SwitchID = @SwitchID)
--					AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' AND SupplierID IS NOT NULL AND CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
			       			       
--			        Group By CustomerID 
--					ORDER BY SUM(Attempts) DESC
--	-- Customer, Supplier, GroupByCustomer
--	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
--			INSERT INTO @Results
--			SELECT CustomerID As CarrierAccountID,
--					Sum(Attempts) as Attempts, 
--					Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
--					case when Sum(Attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
--					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
--					case when Sum(Attempts) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end  as DeliveredASR, 
--					CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
--					MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--					Max(LastCDRAttempt) as LastAttempt,
--					Sum(SuccessfulAttempts)AS SuccessfulAttempts,
--					Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
--					SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
--					SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 
--				FROM TrafficStats TS WITH(NOLOCK)
--				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
--				LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID

--					WHERE 
--						FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--					AND CustomerID = @CustomerID 
--					AND SupplierID = @SupplierID 
--					AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
--					--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  ) OR TS.SwitchID = @SwitchID)
--					AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' AND SupplierID IS NOT NULL AND CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)

--			        Group By CustomerID 
--					ORDER BY SUM(Attempts) DESC
			
--	-- Customer, Supplier, GroupBySupplier
--	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'Y'
--			INSERT INTO @Results
--			SELECT	SupplierID As CarrierAccountID,
--					Sum(Attempts) as Attempts, 
--					Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
--					Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
--					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
--					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--					CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
--					MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--					Max(LastCDRAttempt) as LastAttempt,
--					Sum(SuccessfulAttempts)AS SuccessfulAttempts,
--					Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
--					SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
--					SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 				
--				FROM TrafficStats TS WITH(NOLOCK)
--				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
--				LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID

--					WHERE 
--						FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--					AND CustomerID = @CustomerID 
--					AND SupplierID = @SupplierID 
--					AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
--					--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  ) OR TS.SwitchID = @SwitchID)
--					AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' AND SupplierID IS NOT NULL AND CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)

--			        Group By SupplierID 
--					ORDER BY SUM(Attempts) DESC

--	-- Return the results
	SELECT * FROM @Results

--	-- In case Carrier Grouping is required
--	IF @IncludeCarrierGroupSummary = 'Y' 
--	BEGIN
--		SELECT 
--			cg.CarrierGroupID, 
--			cg.[Path], 
--			SUM(R.Attempts) as Attempts,
--			SUM(R.DurationsInMinutes) as DurationsInMinutes,
--			(SUM(SuccessfulAttempts) * 100.0 / SUM(R.Attempts)) AS ASR,
--			(SUM(DurationsInMinutes) / SUM(R.SuccessfulAttempts)) AS ACD,
--			(SUM(R.DeliveredAttempts) * 100.0 / SUM(R.Attempts)) AS DeliveredASR,
--			CASE WHEN SUM(R.SuccessfulAttempts) > 0 THEN SUM(R.PDDinSeconds) / SUM(R.SuccessfulAttempts) ELSE NULL END as AveragePDD,
--			MAX(R.MaxDuration) AS MaxDuration,
--			MAX(R.LastAttempt) AS LastAttempt,
--			SUM(R.SuccessfulAttempts) AS SuccessfulAttempts, 
--			SUM(R.FailedAttempts) AS FailedAttempts				
--		FROM 
--			@Results R 
--				INNER JOIN CarrierAccount ca WITH(NOLOCK) ON ca.CarrierAccountID = R.CarrierAccountID 
--				LEFT JOIN CarrierGroup g WITH(NOLOCK) ON ca.CarrierGroupID = g.CarrierGroupID
--				LEFT JOIN CarrierGroup cg WITH(NOLOCK) ON g.Path LIKE (cg.[Path] + '%')
				  
--		GROUP BY 
--			cg.CarrierGroupID, cg.[Path]
--		ORDER BY cg.[Path]
--	END

END