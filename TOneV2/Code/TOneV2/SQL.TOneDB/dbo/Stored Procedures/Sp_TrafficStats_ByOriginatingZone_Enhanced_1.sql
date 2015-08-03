CREATE PROCEDURE [dbo].[Sp_TrafficStats_ByOriginatingZone_Enhanced]
    @FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10)= NULL,
	@SwitchID	  tinyInt = NULL,
	--@WhereInCondition VarChar(MAX),
	@OurZoneID    VarChar(MAX)
AS
	SET NOCOUNT ON
	
	Declare @Results TABLE (OriginatingZoneID int, Attempts bigint, SuccessfulAttempts bigint, DeliveredAttempts bigint, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, Percentage numeric(13,5))
	Declare @TotalAttempts BIGINT

	-- No Customer, No Supplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL
		BEGIN
		INSERT INTO @Results   
				(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
				 Sum(cast(Attempts AS bigint)) as Attempts,
				 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
				 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
				 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				 Avg(PDDinSeconds) as AveragePDD, 
				 Max (MaxDurationInSeconds)/60. as MaxDuration,
				 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE 
					--(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
				(FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc  ) OR TS.SwitchID = @SwitchID)
			AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
				Group By TS.OriginatingZoneID
			ORDER by Sum(cast(Attempts AS bigint)) DESC

		
		--SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		--Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
			
		--INSERT INTO @Results
		--	(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		--SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		--FROM @Results
		--UPDATE @Results 
		--SET ASR = SuccessfulAttempts * 100 / Attempts,
		--	ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
		--	DeliveredASR = DeliveredAttempts * 100.0 / Attempts
		--WHERE OriginatingZoneID = 0
		
		  SELECT * from @Results  Order By Attempts DESC   
		END 
		
		-- No Customer, No Supplier, No destination
		
			IF @CustomerID IS NULL AND @SupplierID IS NULL AND @OurZoneID IS NULL
		BEGIN
		INSERT INTO @Results   
				(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
				 Sum(cast(Attempts AS bigint)) as Attempts,
				 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
				 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
				 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				 Avg(PDDinSeconds) as AveragePDD, 
				 Max (MaxDurationInSeconds)/60. as MaxDuration,
				 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE 
					--(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
				(FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc  ) OR TS.SwitchID = @SwitchID)
			--AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
				Group By TS.OriginatingZoneID
			ORDER by Sum(cast(Attempts AS bigint)) DESC

		
		--SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		--Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
			
		--INSERT INTO @Results
		--	(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		--SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		--FROM @Results
		--UPDATE @Results 
		--SET ASR = SuccessfulAttempts * 100 / Attempts,
		--	ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
		--	DeliveredASR = DeliveredAttempts * 100.0 / Attempts
		--WHERE OriginatingZoneID = 0
		
		SELECT * from @Results  Order By Attempts DESC   
		END 
		
	-- Customer, No Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		BEGIN
		INSERT INTO @Results   
			(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
			Sum(cast(Attempts AS bigint)) as Attempts,
			Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
			Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
			Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
			Avg(PDDinSeconds) as AveragePDD, 
			Max (MaxDurationInSeconds)/60. as MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
			WHERE 
				--(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
			(FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (TS.CustomerID = @CustomerID)
		   AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) OR TS.SwitchID = @SwitchID)
		
			Group By TS.OriginatingZoneID
			ORDER by Sum(cast(Attempts AS bigint)) DESC

		--SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		--Update @Results SET Percentage = CASE WHEN @TotalAttempts>0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0.0 END 
	
		--INSERT INTO @Results
		--	(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		--SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		--FROM @Results
		--UPDATE @Results 
		--SET ASR = SuccessfulAttempts * 100 / Attempts,
		--	ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
		--	DeliveredASR = DeliveredAttempts * 100.0 / Attempts
		--WHERE OriginatingZoneID = 0
	
		SELECT * from @Results  Order By Attempts DESC   
		END
	-- NO Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		BEGIN
		INSERT INTO @Results   
			(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
			 Sum(cast(Attempts AS bigint)) as Attempts,
			 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
			 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
			 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
			 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
			 Avg(PDDinSeconds) as AveragePDD, 
			 Max (MaxDurationInSeconds)/60. as MaxDuration,
			 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
		WHERE 
			--(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
			(FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (SupplierID = @SupplierID) 
			AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc  ) OR TS.SwitchID = @SwitchID)
		
		Group By TS.OriginatingZoneID
		ORDER by Sum(cast(Attempts AS bigint)) DESC

		--SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		--Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
	
		--INSERT INTO @Results
		--	(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		--SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		--FROM @Results
		--UPDATE @Results 
		--SET ASR = SuccessfulAttempts * 100 / Attempts,
		--	ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
		--	DeliveredASR = DeliveredAttempts * 100.0 / Attempts
		--WHERE OriginatingZoneID = 0
	
		--SELECT * from @Results  Order By Attempts DESC   
		END
	-- Cutomer, Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL
		BEGIN
		INSERT INTO @Results   
			(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
			 Sum(cast(NumberOfCalls AS bigint)) as Attempts,
			 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
			 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
			 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
			 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
			 Avg(PDDinSeconds) as AveragePDD, 
			 Max (MaxDurationInSeconds)/60. as MaxDuration,
			 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
		WHERE 
			--(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
			(FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (TS.CustomerID = @CustomerID)  
			AND (SupplierID = @SupplierID) 
			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  ) OR TS.SwitchID = @SwitchID)
		Group By TS.OriginatingZoneID
		ORDER by Sum(cast(Attempts AS bigint)) DESC
	
		--SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		--Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
	
		--INSERT INTO @Results
		--	(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		--SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		--FROM @Results
		--UPDATE @Results 
		--SET ASR = SuccessfulAttempts * 100 / Attempts,
		--	ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
		--	DeliveredASR = DeliveredAttempts * 100.0 / Attempts
		--WHERE OriginatingZoneID = 0
	
		SELECT * from @Results  Order By Attempts DESC		
	END