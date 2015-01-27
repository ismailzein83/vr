CREATE PROCEDURE [dbo].[bp_CompressAndClean_Enhanced]
 (   @Batch BIGINT=1000
 	,@TrafficCompressDays INT = 7 -- 7 Days
 	,@TrafficStatsRemainDays INT = 31 -- 1 month
 	,@BillingRemainDays INT = 31 -- 1 month
 	,@CDRRemainDays INT = 93 -- 3 months
 	,@IsDebug char(1)='N'
 )
WITH 
RECOMPILE,
EXECUTE AS CALLER
AS
BEGIN
	
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @MainIDs TABLE(ID bigint NOT NULL PRIMARY key)
	Declare @MinId bigint
	Declare @MaxId BIGINT
	DECLARE @WorkingDayDesc VARCHAR(10)
    DECLARE @Till DATETIME 

	DECLARE @Now DATETIME 
	SET @Now = dbo.DateOf(GETDATE())
	
	DECLARE @Dummy int	

	SET NOCOUNT ON	

	DECLARE @Message VARCHAR(4000)
	DECLARE @MsgID VARCHAR(100)
	DECLARE @MsgSub VARCHAR(100)
	SET @MsgID = 'Compress And Clean'
	
	-----------------------------
        -- Compress Traffic Stats
	-----------------------------
	SET @MsgSub = @MsgID + ': Traffic Stats'
	SET @Message = 'compress Traffic Stats'
	IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
	
	DECLARE @From DATETIME 
	SET @From =DATEADD(DAY,-@TrafficCompressDays,@Now)
	
	EXEC bp_CompressTrafficStats
		@StartingDateTime = @From,
		@EndingDateTime = @Now		
		
    -----------------------------
		-- Delete Traffic Stats
	-----------------------------
	-- Setting the traffic stats period
    	
    SELECT @WorkingDay = MIN(TS.FirstCDRAttempt) FROM TrafficStats TS WITH(NOLOCK)
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@TrafficStatsRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		SET @Message = 'Working On: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
				
		SET @Message = 'Cleaning Traffic Stats for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		BEGIN
			SELECT @DeletedCount = 1
			WHILE @DeletedCount > 0
			BEGIN
				BEGIN TRANSACTION
			        DELETE TrafficStats FROM TrafficStats WITH(NOLOCK) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd
					SET @DeletedCount = @@ROWCOUNT
				COMMIT TRANSACTION	
			END
			IF @IsDebug = 'Y' PRINT 'Deleted Traffic Stats ' + convert(varchar(25), getdate(), 121)
		END

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END
    
     -----------------------------
		-- Delete Traffic Stats
	-----------------------------
	-- Setting the CDR period
    	
    SELECT @WorkingDay = MIN(C.AttemptDateTime) FROM CDR C WITH(NOLOCK,INDEX(IX_CDR_AttemptDateTime))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@CDRRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		SET @MsgSub = @MsgID + ': CDR'
		SET @Message = 'Cleaning CDR ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		SET @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION
					BEGIN
						DELETE CDR FROM CDR WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) WHERE AttemptDateTime BETWEEN @WorkingDay AND @WorkingDayEnd
					END
				SET @DeletedCount = @@ROWCOUNT
				
			COMMIT TRANSACTION
		END
		IF @IsDebug = 'Y' PRINT 'Deleted CDRs ' + convert(varchar(25), getdate(), 121)

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END

    -----------------------------
	   -- Delete Billing_CDR_Invalid
	-----------------------------
	-- Setting the Billing_CDR_Invalid period
    	
    SELECT @WorkingDay = MIN(BI.Attempt) FROM Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@CDRRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
	    SET @MsgSub = @MsgID + ': Billing CDR Invalid'
		SET @Message = 'Cleaning Billing CDR Invalid ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		SET @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION
					BEGIN
						DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd
					END
				SET @DeletedCount = @@ROWCOUNT
				
			COMMIT TRANSACTION
		END
		IF @IsDebug = 'Y' PRINT 'Deleted Invalid CDRs ' + convert(varchar(25), getdate(), 121)

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END


    -----------------------------
	   -- Delete Billing_CDR_Main
	-----------------------------
	-- Setting the Billing_CDR_Main period
    	
    SELECT @WorkingDay = MIN(BM.Attempt) FROM Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@CDRRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
	   SET @MsgSub = @MsgID + ': Main, Cost, Sale - Start'
		SET @Message = 'Deleting Billing CDR Main ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		-- Clear Batch Row Count Number (to insert all Mains)
		SET @DeletedCount = 1
		
		-- While there are still Main CDRs
		WHILE @DeletedCount > 0
		BEGIN
			
			SET ROWCOUNT @Batch

			DELETE FROM @MainIDs
			
			INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
			
			SET ROWCOUNT 0
			
			SELECT @DeletedCount = COUNT(*) FROM @MainIDs
		
			-- If there is something to delete
			IF @DeletedCount > 0
			BEGIN				
				BEGIN TRANSACTION
					-----------------------------
					-- Delete Billing Costs 
					-----------------------------
					DELETE Billing_CDR_Cost FROM Billing_CDR_Cost WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)

					-----------------------------
					-- Delete Billing Sales
					-----------------------------
					DELETE Billing_CDR_Sale FROM Billing_CDR_Sale WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)

					-----------------------------
					-- Delete Main Billing CDRs
					-----------------------------
					DELETE Billing_CDR_Main FROM Billing_CDR_Main WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)
			
				COMMIT TRANSACTION		
			END	-- If @Deleted > 0	
		
		END -- While
		
		SET @MsgSub = @MsgID + ': Main, Cost, Sale - End'
		SET @Message = 'Deleted Billing CDR Main with sales and costs for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END


END