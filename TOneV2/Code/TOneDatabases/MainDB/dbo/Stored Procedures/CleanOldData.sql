CREATE PROCEDURE dbo.CleanOldData(
	@Keep_TrafficStats_Days INT = 60,
	@Keep_BillingMain_Days INT = 60,
	@Keep_BillingInvalid_Days INT = 30,
	@Keep_CDR_Days INT = 90,
	@Keep_EndedSpecialRequests_Days INT = 7,
	@Keep_EndedRouteBlocks_Days INT = 7 
)
AS
BEGIN

	-- Check Validity of Days to keep
	IF @Keep_TrafficStats_Days < 30 SET @Keep_TrafficStats_Days = 30
	IF @Keep_BillingMain_Days < 30 SET @Keep_BillingMain_Days = 30
	IF @Keep_BillingInvalid_Days < 7 SET @Keep_BillingInvalid_Days = 7
	IF @Keep_CDR_Days < 60 SET @Keep_CDR_Days = 60
	IF @Keep_EndedSpecialRequests_Days < 7 SET @Keep_EndedSpecialRequests_Days = 7
	IF @Keep_EndedRouteBlocks_Days < 7 SET @Keep_EndedRouteBlocks_Days = 7

	SET NOCOUNT ON
	SET ROWCOUNT 1000
	
	DECLARE @Counter BIGINT
	DECLARE @Msg VARCHAR(MAX)
	
	DECLARE @Today DATETIME
	SET @Today = dbo.Dateof(GETDATE())
	
	DECLARE @DELETED INT

	-- Traffic Stats
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-TrafficStats-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM TrafficStats WHERE FirstCDRAttempt < DATEADD(day, -@Keep_TrafficStats_Days, @Today)
		SET @DELETED = @@ROWCOUNT
		SET @Counter = @Counter + @DELETED  		
	END
	SET @Msg = 'Days: '+ @Keep_TrafficStats_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-TrafficStats-End', @message = @Msg

	-- Billing Invalid
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Invalid-Start', @message = @Msg
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM Billing_CDR_Invalid WHERE Attempt < DATEADD(day, -@Keep_BillingInvalid_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_BillingInvalid_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Invalid-End', @message = @Msg

	-- Billing Main, Cost and Sale
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Main-Start', @message = NULL
	DECLARE @MainIDs TABLE (ID BIGINT PRIMARY KEY) 
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		INSERT INTO @MainIDs (ID) SELECT ID FROM Billing_CDR_Main WHERE Attempt < DATEADD(day, -@Keep_BillingMain_Days, @Today)
		SET @DELETED = @@ROWCOUNT
		DELETE FROM Billing_CDR_Cost WHERE ID IN (SELECT ID FROM @MainIDs)
		DELETE FROM Billing_CDR_Sale WHERE ID IN (SELECT ID FROM @MainIDs)
		DELETE FROM Billing_CDR_Main WHERE ID IN (SELECT ID FROM @MainIDs)
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_BillingMain_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Main-End', @message = @Msg

	-- CDR
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-CDR-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM CDR WHERE AttemptDateTime < DATEADD(day, -@Keep_CDR_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_CDR_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-CDR-End', @message = @Msg

	-- Special Requests
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-SpecialRequests-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM SpecialRequest WHERE EndEffectiveDate < DATEADD(day, -@Keep_EndedSpecialRequests_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_EndedSpecialRequests_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-SpecialRequests-End', @message = @Msg

	-- Route Blocks
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-RouteBlocks-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM RouteBlock WHERE EndEffectiveDate < DATEADD(day, -@Keep_EndedRouteBlocks_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END	
	SET @Msg = 'Days: '+ @Keep_EndedRouteBlocks_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-RouteBlocks-End', @message = @Msg
			
END