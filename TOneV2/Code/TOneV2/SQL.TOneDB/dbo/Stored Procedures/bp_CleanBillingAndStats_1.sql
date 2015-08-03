CREATE PROCEDURE [dbo].[bp_CleanBillingAndStats]
(@From datetime, @Till datetime, @SwitchID tinyint=NULL, @CustomerID varchar(10)=NULL, @SupplierID varchar(10)=NULL, @Batch bigint=50000, @IsDebug char(1)='N', @IncludeTrafficStats char(1)='Y')
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

	SET @WorkingDay = dbo.DateOf(@From)
	SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)	
	SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
	
	DECLARE @Dummy int	

	SET NOCOUNT ON	

	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		DECLARE @Message VARCHAR(4000)
		DECLARE @MsgID VARCHAR(100)
		DECLARE @MsgSub VARCHAR(100)
		SET @MsgID = 'Clean Billing And Stats'
		
		SET @Message = 'Working On: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
				
		-----------------------------
		-- Delete Invalid Billing Info
		-----------------------------
		SET @MsgSub = @MsgID + ': Billing CDR Invalid'
		SET @Message = 'Cleaning Billing CDR Invalid ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		SET @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION Cleaner
				
				IF @IncludeTrafficStats = 'Y'
					BEGIN
						-- No Customer, No Supplier
						IF @CustomerID IS NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd
						-- No Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt, IX_Billing_CDR_Invalid_Customer)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID
						-- Customer, Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt, IX_Billing_CDR_Invalid_Customer,IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID
						-- No Customer
						ELSE
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt, IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID
					END
				ELSE
					BEGIN
						-- No Customer, No Supplier
						IF @CustomerID IS NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND DurationInSeconds > 0
						-- No Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt,IX_Billing_CDR_Invalid_Customer)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND DurationInSeconds > 0
						-- Customer, Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt,IX_Billing_CDR_Invalid_Customer,IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID AND DurationInSeconds > 0
						-- No Customer
						ELSE
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt,IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID AND DurationInSeconds > 0
					END
				SET @DeletedCount = @@ROWCOUNT
				
			COMMIT TRANSACTION Cleaner
		END
		IF @IsDebug = 'Y' PRINT 'Deleted Invalid CDRs ' + convert(varchar(25), getdate(), 121)
		
		-----------------------------
		-- Get Main Billing CDR ids
		-----------------------------
		SET @MsgSub = @MsgID + ': Main, Cost, Sale - Start'
		SET @Message = 'Deleting Billing CDR Main ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		-- Clear Batch Row Count Number (to insert all Mains)
		SET @DeletedCount = 1

		WHILE @DeletedCount > 0
		BEGIN
			
			DELETE FROM @MainIDs
			
			SET ROWCOUNT @Batch
			
			IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Customer)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID 
			-- Customer, Supplier
			ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Customer, IX_Billing_CDR_Main_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID 
			-- No Customer
			ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL	
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID 
			ELSE 
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
			
			SELECT @DeletedCount = @@ROWCOUNT
			
			-- If there is something to delete
			IF @DeletedCount > 0
			BEGIN				
				SET ROWCOUNT 0

				BEGIN TRANSACTION Cleaner
					IF @CustomerID IS NULL AND @SupplierID IS NULL
						Begin
							Delete Billing_CDR_Main FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
							Delete Billing_CDR_Sale FROM Billing_CDR_Sale WITH(NOLOCK, INDEX(IX_Billing_CDR_Sale_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
							Delete Billing_CDR_Cost FROM Billing_CDR_Cost WITH(NOLOCK, INDEX(IX_Billing_CDR_Cost_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
						End
					Else
						Begin
							-----------------------------
							-- Delete Billing Costs 
							-----------------------------
							DELETE Billing_CDR_Cost FROM Billing_CDR_Cost BC WITH(NOLOCK) WHERE 
							Exists(SELECT ID FROM @MainIDs M Where M.ID=BC.ID)
							
							-----------------------------
							-- Delete Billing Sales
							-----------------------------
							DELETE Billing_CDR_Sale FROM Billing_CDR_Sale BS WITH(NOLOCK) WHERE 
							Exists(SELECT ID FROM @MainIDs M Where M.ID=BS.ID)

							-----------------------------
							-- Delete Main Billing CDRs
							-----------------------------
							DELETE Billing_CDR_Main FROM Billing_CDR_Main BM WITH(NOLOCK) WHERE 
							Exists(SELECT ID FROM @MainIDs M Where M.ID=BM.ID)
						End			
				COMMIT TRANSACTION Cleaner	 	
			END	-- If @Deleted > 0	
		
		END 
		SET @MsgSub = @MsgID + ': Main, Cost, Sale - End'
		SET @Message = 'Deleted Billing CDR Main with sales and costs for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		-----------------------------
		-- Move to next Day
		-----------------------------
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'

	END

END