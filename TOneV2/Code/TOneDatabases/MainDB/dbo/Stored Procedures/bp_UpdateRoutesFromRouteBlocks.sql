--
-- Update the Routes and Route Options from the defined route blocks
--
CREATE PROCEDURE [dbo].[bp_UpdateRoutesFromRouteBlocks]
	@Check char(1) = 'Y'
AS

	SET NOCOUNT ON

	IF @Check = 'Y' 
	BEGIN	
		DECLARE @IsRunning char(1)
		EXEC bp_IsRouteBuildRunning @IsRunning output
		IF @IsRunning = 'Y' 
		BEGIN
			PRINT 'Build Routes is already Runnning'
			RETURN 
		END 
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = 'Updating From Route Blocks'
		EXEC bp_ResetRoutes @UpdateType = 'RouteBlocks'
	END

	DECLARE @UpdateStamp datetime
	SET @UpdateStamp = getdate()
	
	
	/* Supplier Zone Blocks: All Customers */
	-- Block Route Options that have their supplier zones blocked
	UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B WITH(NOLOCK), [RouteOption] WITH(NOLOCK)
			WHERE 
					B.IsEffective='Y' 
				AND B.CustomerID IS NULL 
				AND B.ZoneID IS NOT NULL 
				AND B.ZoneID = [RouteOption].SupplierZoneID

	/* Supplier Zone Blocks: Specific Customers */
	-- Block Route Options for Customer specified Supplier Zone Block
	UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B WITH(NOLOCK), [RouteOption] WITH(NOLOCK), [Route] R WITH(NOLOCK)
		WHERE
			B.IsEffective='Y'
			AND B.CustomerID IS NOT NULL 
			AND B.ZoneID IS NOT NULL
			AND B.SupplierID IS NOT NULL 
			AND B.ZoneID = [RouteOption].SupplierZoneID 
			AND B.SupplierID = [RouteOption].SupplierID
			AND B.CustomerID = R.CustomerID
			AND R.RouteID = [RouteOption].RouteID 

     
  
     	UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B (NOLOCK), [RouteOption] (NOLOCK), [Route] R (NOLOCK)
		WHERE
			B.IsEffective='Y'
			AND B.Code IS NOT NULL 
			AND B.CustomerID = R.CustomerID
			AND R.RouteID = [RouteOption].RouteID 
			AND B.SupplierID = [RouteOption].SupplierID 
			AND R.Code LIKE  B.Code + '%' 
			AND 1= ( CASE WHEN  PATINDEX('%,%',B.ExcludedCodes) > 0  AND 
			                   R.Code NOT IN 
			                   (SELECT * FROM dbo.ParseArray(B.ExcludedCodes,','))
			                   THEN 1
			              WHEN  PATINDEX('%,%',B.ExcludedCodes) = 0 AND  
         			         R.Code NOT LIKE B.ExcludedCodes THEN 1
		
			                   ELSE 0 END 
			       )
			AND B.IncludeSubCodes = 'Y'
    
     
	-- Block Route Options matching other criteria On code only
	    UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B (NOLOCK), [RouteOption] (NOLOCK), [Route] R (NOLOCK)
		WHERE
			B.IsEffective='Y'
			AND B.Code IS NOT NULL -- OR B.ZoneID IS NOT NULL)
			AND B.CustomerID = R.CustomerID
			AND R.RouteID = [RouteOption].RouteID 
			AND B.SupplierID = [RouteOption].SupplierID 
			AND B.Code = R.Code -- OR B.ZoneID = R.OurZoneID)
			AND B.IncludeSubCodes = 'N'

	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='RouteBlocks'

	-- Block Route Options for all Blocked Routes
	UPDATE RouteOption
		SET
		[State] = 0, Updated = @UpdateStamp
		WHERE RouteID IN (SELECT r.RouteID FROM [Route] r WITH(NOLOCK) WHERE r.[State] = 0)

	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END