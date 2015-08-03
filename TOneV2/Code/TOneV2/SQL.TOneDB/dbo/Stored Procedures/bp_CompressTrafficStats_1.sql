
CREATE PROCEDURE [dbo].[bp_CompressTrafficStats]
	@StartingDateTime datetime,
	@EndingDateTime datetime
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @Message VARCHAR(4000)
	DECLARE @MsgID VARCHAR(100)
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @WorkingDayDesc VARCHAR(10)

	SET @WorkingDay = dbo.DateOf(@StartingDateTime)
	SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
	SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)	

	SET @MsgID = 'TrafficStatsCompression:Start'
	SET @Message = convert(varchar(25), GETDATE(), 121)	
	EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message

	CREATE TABLE #TrafficStatsCompressed
	(
		[FirstCDRAttempt] [datetime] NOT NULL,
		[LastCDRAttempt] [datetime] NOT NULL,
		[SwitchId] [tinyint] NOT NULL,
		[CustomerID] [varchar](10) NULL,
		[OurZoneID] [int] NULL,
		[OriginatingZoneID] [int] NULL,
		[SupplierID] [varchar](10) NULL,
		[SupplierZoneID] [int] NULL,
		[Attempts] [int] NULL,
		[NumberOfCalls] [int] NULL,
		[DeliveredAttempts] [int] NULL,
		[SuccessfulAttempts] [int] NULL,
		[DurationsInSeconds] [numeric](13, 5) NULL,
		[PDDInSeconds] [numeric](13, 5) NULL,
		[MaxDurationInSeconds] [numeric](13, 5) NULL,
		[Port_IN]	varchar(21)	NULL,
        [Port_OUT]	varchar(21)	NULL,
        [UtilizationInSeconds] NUMERIC(13, 5) NULL,
		[DeliveredNumberOfCalls] NUMERIC(13, 5) NULL,
		[PGAD] Numeric(19,5) NULL,
		[CeiledDuration] bigint NULL,
		[ReleaseSourceAParty] int NULL
	) 
	
	WHILE @WorkingDay >= @StartingDateTime AND @WorkingDay <= @EndingDateTime
	BEGIN
		
		SET @MsgID = 'TrafficStatsCompression:' + @WorkingDayDesc + ':Start'
		SET @Message = convert(varchar(25), GETDATE(), 121)	
		EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
			
		-- Clean Temp
		TRUNCATE TABLE #TrafficStatsCompressed
		
		BEGIN TRANSACTION TrafficDay 
		
		-- Create Stats for this working day
		INSERT INTO #TrafficStatsCompressed
		(
			FirstCDRAttempt,
			LastCDRAttempt,
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Attempts,
			NumberOfCalls,			
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			MaxDurationInSeconds,
			Port_IN,
			Port_OUT,
			UtilizationInSeconds,
			DeliveredNumberOfCalls,
			PGAD ,
			CeiledDuration,
			ReleaseSourceAParty
		)
		SELECT
			Min(FirstCDRAttempt),
			Max(LastCDRAttempt),
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Sum(Attempts),
			SUM(NumberOfCalls),
			Sum(DeliveredAttempts),
			Sum(SuccessfulAttempts),
			Sum(DurationsInSeconds),
			Avg(PDDInSeconds),
			Max(MaxDurationInSeconds),
			Port_IN,
			Port_OUT,
			SUM(UtilizationInSeconds),
			Sum(DeliveredNumberOfCalls),
			Avg(PGAD) ,
			Sum(CeiledDuration) ,
			ReleaseSourceAParty 
		FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd
		GROUP BY
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Port_IN,
			Port_OUT,
			ReleaseSourceAParty ,
			CONVERT (varchar(10), FirstCDRAttempt, 121)

		-- Remove non necessary stats
		SET @DeletedCount = 1
		SET ROWCOUNT 5000
		WHILE @DeletedCount > 0
		BEGIN
			DELETE TrafficStats FROM TrafficStats WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
				WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd
			SET @DeletedCount = @@ROWCOUNT				
		END
		
		SET ROWCOUNT 0
		
		-- Insert grouped
		INSERT INTO TrafficStats
			(
			FirstCDRAttempt,
			LastCDRAttempt,
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Attempts,
			NumberOfCalls,
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			MaxDurationInSeconds,
			Port_IN,
			Port_OUT,
			UtilizationInSeconds,
			DeliveredNumberOfCalls,
			PGAD ,
			CeiledDuration,
			ReleaseSourceAParty
			)
			SELECT 
				FirstCDRAttempt,
				LastCDRAttempt,
				SwitchId,
				CustomerID,
				OurZoneID,
				OriginatingZoneID,
				SupplierID,
				SupplierZoneID,
				Attempts,
				NumberOfCalls,
				DeliveredAttempts,
				SuccessfulAttempts,
				DurationsInSeconds,
				PDDInSeconds,
				MaxDurationInSeconds,
				Port_IN,
				Port_OUT,
				UtilizationInSeconds,
				DeliveredNumberOfCalls,
				PGAD ,
				CeiledDuration,
				ReleaseSourceAParty
			FROM 
				#TrafficStatsCompressed

		COMMIT TRANSACTION TrafficDay

		SET @MsgID = 'TrafficStatsCompression:' + @WorkingDayDesc + ':End'
		SET @Message = convert(varchar(25), GETDATE(), 121)	
		EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
		
		SET @WorkingDay = DATEADD(DAY, 1, @WorkingDay)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)

	END

	SET @MsgID = 'TrafficStatsCompression:End'
	SET @Message = convert(varchar(25), GETDATE(), 121)	
	EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
		
	DROP TABLE #TrafficStatsCompressed
END