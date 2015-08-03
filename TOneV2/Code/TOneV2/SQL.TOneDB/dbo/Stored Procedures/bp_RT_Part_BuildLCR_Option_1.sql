

CREATE PROCEDURE [dbo].[bp_RT_Part_BuildLCR_Option]
	 @CheckRouteBlocks BIT = 1
	,@MaxSuppliersPerRoute INT = 6
	,@ApplySaleMarketPrice BIT = 0
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @State_Enabled tinyint
	SET @State_Enabled = 1
	
	DECLARE @Message varchar(500) 

	DECLARE @RouteOptionTableCreationSQL nvarchar(max) 
	DECLARE @RouteOptionIndexesCreationSQL nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL1 nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL2 nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL3 nvarchar(max)


	SET @RouteOptionIndexesCreationSQL1 = '
		CREATE NONCLUSTERED INDEX [IDX_RouteOptions_RouteID] ON [dbo].[#RouteOptions]([RouteID] ASC) WITH (ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON,SORT_IN_TEMPDB = ON) 
	';
	SET @RouteOptionIndexesCreationSQL2 = '
		CREATE NONCLUSTERED INDEX [IDX_RouteOptions_SupplierZoneID] ON [dbo].[#RouteOptions]([SupplierZoneID] ASC) WITH (ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON,SORT_IN_TEMPDB = ON) ON [PRIMARY]
	';
	SET @RouteOptionIndexesCreationSQL3 = '
		CREATE NONCLUSTERED INDEX [IDX_RouteOptions_Updated] ON [dbo].[#RouteOptions]([Updated] DESC) WITH (ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON,SORT_IN_TEMPDB = ON) ON [PRIMARY]
	';



IF(@ApplySaleMarketPrice = 0)
BEGIN
	
--- Forced Special Request are added
;with TheOptions AS

	(
		SELECT
			Rt.RouteID,
			CS.SupplierID,
			CS.SupplierZoneID,
			CS.ActiveRate AS SupplierActiveRate,
			rt.OurActiveRate,
			CS.SupplierServicesFlag,
			ISNULL(ronl.Priority,0) [Priority],
			1 AS NumberOfTries,
			ISNULL(ronl.[State],1) [State],
			ISNULL(ronl.[Percentage],0) [Percentage],
			(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.ActiveRate ASC,ronl.SupplierServicesFlag DESC)) as RowNumber,
			rt.CustomerID,
			rt.Code,
			Rt.OurServicesFlag,
			ISNULL(ronl.ActionType,0) ActionType
			
		FROM #Routes Rt 
			INNER join #RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
			LEFT JOIN #RouteOption_NonLCR ronl ON 
			ronl.Code COLLATE DATABASE_DEFAULT = rt.Code AND ronl.CustomerID COLLATE DATABASE_DEFAULT = rt.CustomerID AND ronl.SupplierID COLLATE DATABASE_DEFAULT = CS.SupplierID 
			--LEFT JOIN #RouteOption_Block rob ON rob.Code COLLATE DATABASE_DEFAULT = rt.Code AND rob.CustomerID COLLATE DATABASE_DEFAULT = rt.CustomerID AND rob.SupplierID COLLATE DATABASE_DEFAULT = CS.SupplierID
			LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID =RPC.CustomerID COLLATE DATABASE_DEFAULT
			LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID=RPS.SupplierID COLLATE DATABASE_DEFAULT and rpc.id=rps.id
			--LEFT JOIN #RouteOption_Special sp ON sp.ActionType = 4 AND  sp.Code COLLATE DATABASE_DEFAULT = rt.Code AND sp.CustomerID COLLATE DATABASE_DEFAULT = rt.CustomerID AND sp.SupplierID COLLATE DATABASE_DEFAULT = CS.SupplierID	 
			WHERE 
				
				RT.ProfileID <> CS.ProfileID -- Prevent Looping
				AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
				AND CS.ActiveRate <= Rt.OurActiveRate
				AND RT.IsBlockAffected = 0
				AND RT.IsOverrideAffected = 0
				AND (ronl.ActionType is null or ronl.ActionType = 3)
				AND cs.IsBlock = 0
					),
	
	NonLCROptions AS (
	SELECT 
			TR.RouteID,
			TR.CustomerID,
			TR.Code, 
			CS.SupplierID,
			CS.SupplierZoneID,
			cs.SupplierActiveRate,
			CS.SupplierServicesFlag,
			CS.[Priority] ,
			CS.NumberOfTries,
			CS.[State],
			CS.[Percentage]
	
	FROM #RouteOption_NonLCR cs WITH(NOLOCK) INNER join #Routes TR WITH(NOLOCK)
	ON tr.CustomerID COLLATE DATABASE_DEFAULT = cs.CustomerID AND tr.code COLLATE DATABASE_DEFAULT = cs.code
	 WHERE 
	      (((cs.actiontype IN(5,4)) or (cs.actiontype=1 and ((CS.SupplierServicesFlag & tr.OurServicesFlag) = tr.OurServicesFlag)) or (CS.SupplierActiveRate <= tr.OurActiveRate and cs.actiontype = 2)  ) AND tr.IsBlockAffected = 0) 
	
	),
	
	AllOptions AS (
	SELECT * FROM NonLCROptions 
	UNION ALL
	(
		SELECT 
		RouteID,
		CustomerID,
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		Percentage
	FROM TheOptions
	WHERE RowNumber <= @MaxSuppliersPerRoute OR ActionType = 4
	)	
	)

	INSERT INTO [#RouteOptions]  with(TABLOCK) 
	(
		RouteID,
		CustomerID,
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		Updated,
		[Percentage]
		
	)
	SELECT 
		RouteID,
		CustomerID,
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		getdate(),
		Percentage
	FROM AllOptions
END

ELSE
	BEGIN

					SELECT 'SaleMarket',
						Rt.RouteID,
						CS.SupplierID,
						CS.SupplierZoneID,
						CS.ActiveRate AS SupplierActiveRate,
						rt.OurActiveRate,
						CS.SupplierServicesFlag,
						rt.CustomerID,
						rt.Code,
						Rt.OurServicesFlag
						
					FROM #Routes Rt 
						INNER join #RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
						--LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID COLLATE DATABASE_DEFAULT =RPC.CustomerID
						--LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID COLLATE DATABASE_DEFAULT =RPS.SupplierID and rpc.id=rps.id	 
						inner JOIN SaleZoneMarketPrice szmp ON szmp.SaleZoneID = rt.OurZoneID  AND CS.SupplierServicesFlag  = szmp.ServicesFlag AND Rt.OurServicesFlag = szmp.ServicesFlag
						WHERE 
  
								(szmp.FromRate > cs.ActiveRate  or szmp.ToRate < cs.ActiveRate)
	
--- Forced Special Request are added
;with 
NonMarketPriceOptions AS

				(
					SELECT
						Rt.RouteID,
						CS.SupplierID,
						CS.SupplierZoneID,
						CS.ActiveRate AS SupplierActiveRate,
						rt.OurActiveRate,
						CS.SupplierServicesFlag,
						rt.CustomerID,
						rt.Code,
						Rt.OurServicesFlag
						
					FROM #Routes Rt 
						INNER join #RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
						--LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID COLLATE DATABASE_DEFAULT =RPC.CustomerID
						--LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID COLLATE DATABASE_DEFAULT =RPS.SupplierID and rpc.id=rps.id	 
						inner JOIN SaleZoneMarketPrice szmp ON szmp.SaleZoneID = rt.OurZoneID  AND CS.SupplierServicesFlag  = szmp.ServicesFlag AND Rt.OurServicesFlag = szmp.ServicesFlag
						WHERE 
  
								(szmp.FromRate > cs.ActiveRate  or szmp.ToRate < cs.ActiveRate)
								),
TheOptions AS

	(
		SELECT
			Rt.RouteID,
			CS.SupplierID,
			CS.SupplierZoneID,
			CS.ActiveRate AS SupplierActiveRate,
			rt.OurActiveRate,
			CS.SupplierServicesFlag,
			ISNULL(ronl.Priority,0) [Priority],
			1 AS NumberOfTries,
			ISNULL(ronl.[State],1) [State],
			ISNULL(ronl.[Percentage],0) [Percentage],
			(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.ActiveRate ASC,ronl.SupplierServicesFlag DESC)) as RowNumber,
			rt.CustomerID,
			rt.Code,
			Rt.OurServicesFlag,
			ISNULL(ronl.ActionType,0) ActionType
			
		FROM #Routes Rt 
			INNER join #RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
			LEFT JOIN #RouteOption_NonLCR ronl ON 
			ronl.Code COLLATE DATABASE_DEFAULT = rt.Code AND ronl.CustomerID COLLATE DATABASE_DEFAULT = rt.CustomerID AND ronl.SupplierID COLLATE DATABASE_DEFAULT = CS.SupplierID 
			LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID COLLATE DATABASE_DEFAULT =RPC.CustomerID
			LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID COLLATE DATABASE_DEFAULT =RPS.SupplierID and rpc.id=rps.id	 
			LEFT JOIN NonMarketPriceOptions msz ON msz.SupplierID COLLATE DATABASE_DEFAULT = cs.SupplierID AND Rt.Code = msz.Code COLLATE DATABASE_DEFAULT
			WHERE 
				
				RT.ProfileID <> CS.ProfileID -- Prevent Looping
				AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
				AND CS.ActiveRate <= Rt.OurActiveRate
				AND RT.IsBlockAffected = 0
				AND RT.IsOverrideAffected = 0
				AND (ronl.ActionType is null or ronl.ActionType = 3)
				AND cs.IsBlock = 0
				AND msz.SupplierID IS null AND  msz.SupplierZoneID IS NULL AND  msz.SupplierServicesFlag IS NULL
					),
	
	NonLCROptions AS (
	SELECT 
			TR.RouteID,
			TR.CustomerID,
			TR.Code, 
			CS.SupplierID,
			CS.SupplierZoneID,
			cs.SupplierActiveRate,
			CS.SupplierServicesFlag,
			CS.[Priority] ,
			CS.NumberOfTries,
			CS.[State],
			CS.[Percentage]
	
	FROM #RouteOption_NonLCR cs WITH(NOLOCK) INNER join #Routes TR WITH(NOLOCK)
	ON tr.CustomerID COLLATE DATABASE_DEFAULT = cs.CustomerID AND tr.code COLLATE DATABASE_DEFAULT = cs.code
	 WHERE 
      (((cs.actiontype IN(5,4)) or (cs.actiontype=1 and ((CS.SupplierServicesFlag & tr.OurServicesFlag) = tr.OurServicesFlag)) or (CS.SupplierActiveRate <= tr.OurActiveRate and cs.actiontype = 2)  ) AND tr.IsBlockAffected = 0) 
	
	),
	
	AllOptions AS (
	SELECT * FROM NonLCROptions 
	UNION ALL
	(
		SELECT 
		RouteID,
		CustomerID,
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		Percentage
	FROM TheOptions
	WHERE RowNumber <= @MaxSuppliersPerRoute OR ActionType = 4
	)	
	)

	INSERT INTO [#RouteOptions]  with(TABLOCK) 
	(
		RouteID,
		CustomerID,
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		Updated,
		[Percentage]
		
	)
	SELECT 
		RouteID,
		CustomerID,
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		getdate(),
		Percentage
	FROM AllOptions
	END
	
	
	IF(@CheckRouteBlocks = 1)
	BEGIN
--	;with BlockedOptions AS

--	(
--		SELECT
--			Rt.RouteID,
--			CS.SupplierID,
--			CS.SupplierZoneID,
--			CS.ActiveRate AS SupplierActiveRate,
--			rt.OurActiveRate,
--			CS.SupplierServicesFlag,
--			ISNULL(ronl.Priority,0) [Priority],
--			1 AS NumberOfTries,
--			0 [State],
--			ISNULL(ronl.[Percentage],0) [Percentage],
--			(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.ActiveRate ASC,ronl.SupplierServicesFlag DESC)) as RowNumber,
--			rt.CustomerID,
--			rt.Code,
--			Rt.OurServicesFlag,
--			ISNULL(ronl.ActionType,0) ActionType
			
--		FROM
--				#Routes Rt 
--				INNER join #RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
--			LEFT JOIN #RouteOption_Special ronl ON 
--			ronl.Code COLLATE DATABASE_DEFAULT = rt.Code AND ronl.CustomerID COLLATE DATABASE_DEFAULT = rt.CustomerID AND ronl.SupplierID COLLATE DATABASE_DEFAULT = CS.SupplierID 
--			WHERE 
				
--				RT.ProfileID <> CS.ProfileID -- Prevent Looping
--				AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
--				AND CS.ActiveRate <= Rt.OurActiveRate
--				AND (ronl.ActionType = 3 OR ronl.ActionType IS NULL )
--				AND RT.IsBlockAffected = 0
--				AND Rt.IsOverrideAffected = 0
--				AND cs.IsBlock = 1--AND (@CheckRouteBlocks='Y' OR (@CheckRouteBlocks= 'N' AND cs.IsBlock=0))
--				--AND CS.SupplierID = 'c006' AND cs.code LIKE '961%'
				
--	)
--INSERT INTO #RouteOptions with(TABLOCK) 
--		(
--			RouteID,
--			CustomerID,
--			Code,
--			SupplierID,
--			SupplierZoneID,
--			SupplierActiveRate,
--			SupplierServicesFlag,
--			Priority,
--			NumberOfTries,
--			[State],
--			Updated,
--			Percentage
--		)
--		SELECT
--		RouteID,
--		CustomerID,
--		Code,
--		SupplierID,
--		SupplierZoneID,
--		SupplierActiveRate,
--		SupplierServicesFlag,
--		Priority,
--		NumberOfTries,
--		[State],
--		getdate(),
--		Percentage
--	FROM BlockedOptions


;with BlockedOptions AS

	(
		SELECT DISTINCT
		rt.CustomerID,
			rt.Code,
			Rt.RouteID,
			--CS.SupplierID,
			--CS.SupplierZoneID,
			--CS.ActiveRate AS SupplierActiveRate,
			--rt.OurActiveRate,
			--CS.SupplierServicesFlag,
			8 AS priority,
			1 AS NumberOfTries,
			0 [State],
			0 AS percentage,
			--(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.ActiveRate ASC,ronl.SupplierServicesFlag DESC)) as RowNumber,
			
			--Rt.OurServicesFlag,
			ActionType
			
		FROM
				#Routes Rt 
			INNER JOIN #RouteOption_NonLCR ronl ON 
			ronl.Code = rt.Code AND ronl.CustomerID = rt.CustomerID
			WHERE 
				ronl.ActionType = 1 AND ronl.SupplierID = 'BLK'
				
	)
INSERT INTO #RouteOptions with(TABLOCK) 
		(
			RouteID,
			CustomerID,
			Code,
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
		CustomerID,
		Code,
		'BLK',
		NULL,
		-1,
		1,
		8,
		NumberOfTries,
		[State],
		getdate(),
		Percentage
	FROM BlockedOptions
				
	END
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: RouteOptions Inserted', @Message
	
	 
	 	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: Start building INDEX for RouteOption', @Message



	EXEC (@RouteOptionIndexesCreationSQL1)
	
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: Start building INDEX ROUTEID for RouteOption', @Message
	
	EXEC (@RouteOptionIndexesCreationSQL2)
	
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: Start building INDEX ZONEID for RouteOption', @Message
	
		
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: INDEX Built for RouteOptions', @Message
	 
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: RouteOptions of Special Request Inserted', @Message
	
--Testing
SELECT 'LCR Option', * FROM #RouteOptions ro	
END