

CREATE PROCEDURE [dbo].[SP_TrafficStats_RateChange_Enhanced]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
	@OurZoneID 	  INT,
	@ServicesFlag smallint = NULL	
	AS
BEGIN
	
	DECLARE  @AllDays TABLE (DayStart DATETIME,[DayEnd] DATETIME)
    
    DECLARE @date SMALLDATETIME
    SET @date= dbo.DateOf(@FromDateTime)
     
    WHILE(@date <= @ToDateTime)
    BEGIN 
			INSERT INTO @AllDays ( DayStart,[DayEnd]) VALUES (@date,DATEADD(dd,1,@date))
			SET @date = DATEADD(dd,1,@date)
    END 
    
    DECLARE @Results TABLE (
    		[Day] varchar(10), 
    		Rate numeric(13,5),RateUnScaled numeric(13,5), RateScale numeric(13,5), RateScaleName varchar(10), 
    		Attempts numeric(13,5) , AttemptScale numeric(13,5), AttemptScaleName varchar(10), 
    		DurationsInMinutes numeric(13,5), DurationScale numeric(13,5), DurationScaleName varchar(10), 
    		ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5),SuccessfulAttempts numeric(13,5) )
    
  DECLARE @RepresentedAsSwitchCarriers TABLE (
CID VARCHAR(5)
)
INSERT INTO  @RepresentedAsSwitchCarriers SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH (NOLOCK)
                 WHERE ca.RepresentsASwitch='Y'  
    SET NOCOUNT ON
    INSERT INTO @Results
    (
        [Day],
        Rate,
        RateUnScaled,
        Attempts,
        DurationsInMinutes,
        ASR,
        ACD,
        DeliveredASR,
        SuccessfulAttempts
    )	 	
SELECT CONVERT(VARCHAR(10), A.DayStart,121),
       R.Rate,
       R.Rate,
       Sum(Attempts) AS Attempts,
       Sum(DurationsInSeconds)/60.0 AS DurationsInMinutes,
       Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) AS ASR,
       CASE 
            WHEN Sum(SuccessfulAttempts) > 0 THEN Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)
            ELSE 0
       END AS ACD,
       Sum(deliveredAttempts) * 100.0 / SUM(Attempts) AS DeliveredASR,
       Sum(SuccessfulAttempts) AS SuccessfulAttempts
FROM
	 @AllDays A
	 
	 
	 		     LEFT JOIN TrafficStats TS WITH(NOLOCK) 
				ON TS.FirstCDRAttempt >= A.DayStart 
				   AND TS.LastCDRAttempt < A.DayEnd
				      
		INNER JOIN  PriceList P ON P.CustomerID =@CustomerID 
		INNER JOIN  Rate R  ON P.PriceListID = R.PriceListID 
		AND  R.ZoneID = ts.OurZoneID 
							
	 AND (R.BeginEffectiveDate <= A.DayStart AND (R.EndEffectiveDate IS NULL OR  R.EndEffectiveDate > = A.DayStart))
		AND  ( TS.CustomerID = @CustomerID)-- AND TS.CustomerID NOT IN (SELECT rasc.CID
																				--	  FROM @RepresentedAsSwitchCarriers rasc)
					   AND  (@SupplierID IS NULL OR  TS.SupplierID = @SupplierID)	
	--INNER JOIN  PriceList P ON P.CustomerID = @CustomerID 
	--INNER JOIN  Rate R  ON P.PriceListID = R.PriceListID 
	--					   AND  R.ZoneID = @OurZoneID 
	--				       AND (R.BeginEffectiveDate <= A.DayStart AND (R.EndEffectiveDate IS NULL OR  R.EndEffectiveDate > = A.DayStart))
 --   LEFT JOIN TrafficStats TS WITH(NOLOCK) 
	--		ON TS.FirstCDRAttempt >= A.DayStart 
	--		   AND TS.LastCDRAttempt < A.DayEnd
	--			   --AND  (@ourzoneid is null or TS.OurZoneID = @OurZoneID)
	--			   --AND  (@customerid is null or TS.CustomerID = @CustomerID) AND TS.CustomerID NOT IN (SELECT rasc.CID
	--			   --                                                               FROM @RepresentedAsSwitchCarriers rasc)
	--			   --AND  (@SupplierID IS NULL OR  TS.SupplierID = @SupplierID)	 
 --    --AND (@ServicesFlag IS NULL OR SZ.ServicesFlag IN (SELECT M.ServiceFlag FROM ServiceFlagMask M WHERE M.Mask = @ServicesFlag))
	
		  
    GROUP BY CONVERT(VARCHAR(10), A.DayStart,121), R.Rate
    OPTION (Recompile)
    
    DECLARE @TotalAttempts bigint
    SELECT  @TotalAttempts = MAX(Attempts) FROM @Results
    DECLARE @AttemptScaleName varchar(10)
    DECLARE @AttemptScale numeric(13,5)
	EXEC bp_GetScale @TotalAttempts, @AttemptScale output, @AttemptScaleName output
	
    DECLARE @MaxDuration numeric(13,5) 
    SELECT  @MaxDuration = Max(DurationsInMinutes) FROM @Results
    DECLARE @DurationScaleName varchar(10)
    DECLARE @DurationScale numeric(13,5)
	EXEC bp_GetScale @MaxDuration, @DurationScale output, @DurationScaleName output

	Declare @MaxRate numeric(13,5)
	SELECT  @MaxRate = Max(Rate) FROM @Results
    DECLARE @RateScaleName varchar(10)
    DECLARE @RateScale numeric(13,5)
	EXEC bp_GetScale @MaxRate, @RateScale output, @RateScaleName output
    
    UPDATE @Results
		SET
			Attempts = (Attempts / @AttemptScale),
			AttemptScaleName = @AttemptScaleName,
			AttemptScale = @AttemptScale,
			DurationsInMinutes = (DurationsInMinutes /@DurationScale),
			DurationScaleName = @DurationScaleName,
			DurationScale = @DurationScale,
			Rate = (Rate /@RateScale),
			RateScaleName = @RateScaleName,
			RateScale = @RateScale
    
    SELECT * FROM @Results ORDER BY [Day]
    
END