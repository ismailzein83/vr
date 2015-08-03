

--
-- Update the Routes and Route Options from the defined Special Requests
--
CREATE  PROCEDURE [dbo].[bp_RT_UpdateRoutesFromSpecialRequests]
(
	@Check char(1) = 'Y',
	@CustomerId varchar(5) 
)
AS
BEGIN
	
	SET NOCOUNT ON

	IF @Check = 'Y'
	BEGIN
		DECLARE @IsRunning char(1)
		EXEC bp_IsRouteBuildRunning @IsRunning output
		print @IsRunning
		IF @IsRunning = 'Y' 
		BEGIN
			PRINT 'Build Routes is already Runnning'
			RETURN 
		END 
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = 'Updating From Special Requests'
		EXEC bp_ResetRoutes @UpdateType = 'SpecialRequests'
	END		

	DECLARE @UpdateStamp datetime
	SET @UpdateStamp = getdate()

	DECLARE @HighestPriority tinyint
	SET @HighestPriority = 0

	DECLARE @ForcedRoute tinyint
	SET @ForcedRoute = 1



-- case when included subcodes 
-- insert forced routes
	INSERT INTO [RouteOption]
	(
		RouteID,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,		
		NumberOfTries,
		[State],
		Updated,
		Percentage
	)
	SELECT 
		RouteID,
		sr.SupplierID,
		zr.ZoneID,
		zr.NormalRate,
		zr.ServicesFlag,
		sr.Priority,		
		sr.NumberOfTries,
		1,
		GETDATE(),
		sr.Percentage
		FROM 
			SpecialRequest sr, CarrierAccount ca, [Route] r, CodeMatch cm, ZoneRate zr  
		WHERE 
				sr.IsEffective = 'Y'
			AND sr.SpecialRequestType = @ForcedRoute
			AND sr.CustomerID = r.CustomerID 
			AND sr.SupplierID = ca.CarrierAccountID
			AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (1,3)
			AND r.Code LIKE sr.Code +'%' 
			AND 1= ( CASE WHEN  PATINDEX('%,%',sr.ExcludedCodes) > 0  AND 
			                   R.Code NOT IN 
			                   (SELECT * FROM dbo.ParseArray(sr.ExcludedCodes,','))
			                   THEN 1
			              WHEN  PATINDEX('%,%',sr.ExcludedCodes) = 0 AND  
         			         R.Code NOT LIKE sr.ExcludedCodes THEN 1
		
			                   ELSE 0 END )
			AND sr.CustomerID = r.CustomerID
			AND NOT EXISTS(SELECT * FROM [RouteOption] ro WHERE ro.RouteID = r.RouteID AND ro.SupplierID = sr.SupplierID)
			AND cm.Code = r.Code
			AND cm.SupplierID = sr.SupplierID
			AND zr.ZoneID = cm.SupplierZoneID
			AND sr.IncludeSubCodes ='Y'

	-- insert forced routes
	INSERT INTO [RouteOption]
	(
		RouteID,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		
		NumberOfTries,
		[State],
		Updated,
		Percentage
	)
	SELECT 
		RouteID,
		sr.SupplierID,
		zr.ZoneID,
		zr.NormalRate,
		zr.ServicesFlag,
		sr.Priority,
		sr.NumberOfTries,
		1,		
		GETDATE()
		,sr.Percentage
		FROM 
			SpecialRequest sr, CarrierAccount ca, [Route] r, CodeMatch cm, ZoneRate zr  
		WHERE 
				sr.IsEffective = 'Y'
			AND sr.SpecialRequestType = @ForcedRoute
			AND sr.CustomerID = r.CustomerID 
			AND sr.SupplierID = ca.CarrierAccountID
			AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (1,3)
			AND sr.Code = r.Code
			AND sr.CustomerID = r.CustomerID
			AND NOT EXISTS(SELECT * FROM [RouteOption] ro WHERE ro.RouteID = r.RouteID AND ro.SupplierID = sr.SupplierID)
			AND cm.Code = r.Code
			AND cm.SupplierID = sr.SupplierID
			AND zr.ZoneID = cm.SupplierZoneID
			AND sr.IncludeSubCodes='N'


	
	-- Special Requests for Customer, Code/Zone, Supplier
	UPDATE [RouteOption] 
		SET 
			Priority = S.Priority 
			, Percentage = S.Percentage 
			, Updated = GETDATE()
			, NumberOfTries = S.NumberOfTries 
	FROM 
		[RouteOption], [Route] R, SpecialRequest S
	WHERE
		S.IsEffective='Y'
		AND [RouteOption].RouteID = R.RouteID
		AND S.SupplierID = [RouteOption].SupplierID
		AND S.CustomerID = R.CustomerID
		--AND s.SpecialRequestType = @HighestPriority ----removed to reflect priority change
		AND  r.Code LIKE s.code +'%'
			AND 1= ( CASE WHEN  PATINDEX('%,%',S.ExcludedCodes) > 0  AND 
			                   R.Code NOT IN 
			                   (SELECT * FROM dbo.ParseArray(S.ExcludedCodes,','))
			                   THEN 1
			              WHEN  PATINDEX('%,%',S.ExcludedCodes) = 0 AND  
         			         R.Code NOT LIKE S.ExcludedCodes THEN 1
		
			                   ELSE 0 END )
		AND s.IncludeSubCodes ='Y'

	
	-- Special Requests for Customer, Code/Zone, Supplier
	UPDATE [RouteOption] 
		SET 
			Priority = S.Priority 
			, Percentage = S.Percentage 
			, Updated = GETDATE()
			, NumberOfTries = S.NumberOfTries 
	FROM 
		[RouteOption], [Route] R, SpecialRequest S
	WHERE
		S.IsEffective='Y'
		AND [RouteOption].RouteID = R.RouteID
		AND S.SupplierID = [RouteOption].SupplierID
		AND S.CustomerID = R.CustomerID
		--AND s.SpecialRequestType = @HighestPriority ----removed to reflect priority change
		AND S.Code = r.Code
		AND s.IncludeSubCodes ='N'
	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='SpecialRequests'

	
	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END

END