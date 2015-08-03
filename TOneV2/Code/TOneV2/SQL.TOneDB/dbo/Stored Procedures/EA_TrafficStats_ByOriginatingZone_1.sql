
CREATE PROCEDURE [dbo].[EA_TrafficStats_ByOriginatingZone]
    @FromDateTime DATETIME,
    @ToDateTime   DATETIME,
    @CustomerID   varchar(10) = NULL,
    @SupplierID   varchar(10)= NULL,
    @SwitchID      tinyInt = NULL,
    @OurZoneID    VarChar(MAX),
    @ByCodeGroup char(1) = 'N',
    @ShowDestinationZones char(1) = 'N',
    @AllAccounts varchar(max) = NULL
AS
    SET NOCOUNT ON
   
    Declare @Results TABLE (Originating int, Attempts bigint, SuccessfulAttempts bigint, DeliveredAttempts bigint, DurationsInMinutes numeric(13,5), CeiledDuration numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime,OurZone int,Percentage numeric(13,5))
    Declare @TotalAttempts BIGINT

    -- No Customer, No Supplier
    
       
    -- Customer, No Supplier
    IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
        BEGIN
        INSERT INTO @Results  
            (Originating, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, CeiledDuration, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT  CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END AS OriginatingGrouping,
            Sum(cast(Attempts AS bigint)) as Attempts,
            Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
            Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
            Sum(DurationsInSeconds/60.) as DurationsInMinutes,
            SUM(CeiledDuration)/60.0 as CeiledDuration,
            Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
            case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
            Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
            Avg(PDDinSeconds) as AveragePDD,
            Max (MaxDurationInSeconds)/60. as MaxDuration,
            Max(LastCDRAttempt) as LastAttempt,
            (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END),0
        FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
        INNER JOIN Zone Z ON TS.OriginatingZoneID = Z.ZoneID
            WHERE
            (FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
            AND (TS.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
            AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) OR TS.SwitchID = @SwitchID)
            AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
            Group By (CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END),(CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END)
            ORDER by Sum(cast(Attempts AS bigint)) DESC

        SELECT @TotalAttempts = SUM(Attempts) FROM @Results
        Update @Results SET Percentage = CASE WHEN @TotalAttempts>0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0.0 END
   
        INSERT INTO @Results
            (Originating, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, CeiledDuration, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), SUM(CeiledDuration), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt),0, 100
        FROM @Results
        UPDATE @Results
        SET ASR = SuccessfulAttempts * 100 / Attempts,
            ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
            DeliveredASR = DeliveredAttempts * 100.0 / Attempts
        WHERE Originating = 0
   
        SELECT * from @Results  Order By Attempts DESC  
        END
        
        
        
        
    -- NO Customer, Supplier
    ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
        BEGIN
        INSERT INTO @Results  
            (Originating , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, CeiledDuration, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT  CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END AS OriginatingGrouping,
             Sum(cast(Attempts AS bigint)) as Attempts,
             Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
             Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
             Sum(DurationsInSeconds/60.) as DurationsInMinutes,
             SUM(CeiledDuration)/60.0 as CeiledDuration,
             Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
             case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
             Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
             Avg(PDDinSeconds) as AveragePDD,
             Max (MaxDurationInSeconds)/60. as MaxDuration,
             Max(LastCDRAttempt) as LastAttempt,
             (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END),0
        FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
        INNER JOIN Zone Z ON TS.OriginatingZoneID = Z.ZoneID
        WHERE
            (FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
            AND (TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
            AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc  ) OR TS.SwitchID = @SwitchID)
			AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
        Group By (CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END),(CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END)
        ORDER by Sum(cast(Attempts AS bigint)) DESC

        SELECT @TotalAttempts = SUM(Attempts) FROM @Results
        Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
   
        INSERT INTO @Results
            (Originating, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, CeiledDuration, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), SUM(CeiledDuration), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt),0, 100
        FROM @Results
        UPDATE @Results
        SET ASR = SuccessfulAttempts * 100 / Attempts,
            ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
            DeliveredASR = DeliveredAttempts * 100.0 / Attempts
        WHERE Originating = 0
   
        SELECT * from @Results  Order By Attempts DESC  
        END