CREATE PROCEDURE [dbo].[bp_CleanTrafficStats_Enhanced]
(@From datetime, @Till datetime, @SwitchID tinyint=NULL, @CustomerID varchar(10)=NULL, @SupplierID varchar(10)=NULL, @Batch bigint=1000, @IsDebug char(1)='N')
WITH 
RECOMPILE,
EXECUTE AS CALLER
AS
BEGIN
	
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @WorkingDayDesc VARCHAR(10)

	DECLARE @Message VARCHAR(4000)
	DECLARE @MsgID VARCHAR(100)
	SET @MsgID = 'Clean Traffic Stats'

	SET @WorkingDay = dbo.DateOf(@From)
	SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)	
	SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
	
	DECLARE @Dummy int	

	SET NOCOUNT ON	

	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		SET @Message = 'Cleaning Traffic Stats for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message

		-----------------------------
		-- Delete Traffic Stats
		-----------------------------
		SELECT @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION Cleaner
				-- No Customer, No Supplier
				IF @CustomerID IS NULL AND @SupplierID IS NULL
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd 
				-- No Supplier
				ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID 
				-- Customer, Supplier
				ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID
				-- No Customer
				ELSE
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID
				SET @DeletedCount = @@ROWCOUNT
			COMMIT TRANSACTION Cleaner	
		END
		IF @IsDebug = 'Y' PRINT 'Deleted Traffic Stats ' + convert(varchar(25), getdate(), 121)
		
		-----------------------------
		-- Move to next Day
		-----------------------------
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'

		SET @Message = 'Cleaned Traffic Stats for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message

	END

END