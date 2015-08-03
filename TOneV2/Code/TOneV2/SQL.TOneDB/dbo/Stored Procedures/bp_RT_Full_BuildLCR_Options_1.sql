
CREATE PROCEDURE [dbo].[bp_RT_Full_BuildLCR_Options]
	 @RoutingTableFileGroup nvarchar(255) = 'PRIMARY'
	,@RoutingIndexesFileGroup nvarchar(255) = 'PRIMARY'
	,@SORT_IN_TEMPDB nvarchar(10) = 'ON'
	,@IncludeBlockedZones CHAR(1) = 'N' 
	,@MaxSuppliersPerRoute INT = 6
	,@CheckRouteBlocks CHAR(1) = 'N'
	,@UpdateStamp datetime OUTPUT
	,@ApplySaleMarketPrice CHAR(1) = 'N' 
WITH RECOMPILE
AS
BEGIN

	SET NOCOUNT ON


	DECLARE @State_Enabled tinyint
	SET @State_Enabled = 1
	SET @UpdateStamp = getdate() 
	DECLARE @Message varchar(500) 
	declare @startroute int
declare @endroute int
declare @count int

	DECLARE @RouteOptionTableCreationSQL nvarchar(max) 
	DECLARE @RouteOptionIndexesCreationSQL nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL1 nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL2 nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL3 nvarchar(max)


	

	Set @RouteOptionTableCreationSQL = '
 		-- Temp Route Option
		CREATE TABLE [dbo].[TempRouteOptions](
			[RouteID] [int] NOT NULL,
			[SupplierID] [varchar](5) NOT NULL,
			[SupplierZoneID] [int] NULL,
			[SupplierActiveRate] [real] NULL,
			[SupplierServicesFlag] [smallint] NULL,
			[Priority] [tinyint] NOT NULL,
			[NumberOfTries] [tinyint] NULL,
			[State] [tinyint] NOT NULL DEFAULT ((0)),
			[Updated] [datetime] NULL,
			[Percentage] [tinyint] NULL
		) ON [' + @RoutingTableFileGroup + ']
	';
	
		-- Temp Route Option
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRouteOptions]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRouteOptions]
	END

	EXEC (@RouteOptionTableCreationSQL);

SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Start RouteOptions Insertion', @Message
	set @UpdateStamp = getdate()
IF(@ApplySaleMarketPrice = 'N')
Begin

			
			
			SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: No Market Price applied', @Message

			
			; with RouteOption_NonLCRs as  ( 
			select * from RouteOption_NonLCR
			where actiontype is null or actiontype=3)
			
			
			
			
				INSERT INTO [TempRouteOptions]  with(TABLOCK) 
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
					[Percentage]
					
				)
			
				
				select routeid,supplierid,supplierzoneid,supplieractiverate,SupplierServicesFlag,Priority,1 AS NumberOfTries,
				[State],DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp),[Percentage]
				 from 
			(
			SELECT
						Rt.RouteID,
						CS.SupplierID,
						CS.SupplierZoneID,
						CS.ActiveRate AS SupplierActiveRate,
						CS.SupplierServicesFlag,
						ISNULL(ronl.Priority,0) [Priority],
						ISNULL(ronl.[State],1) [State],
						((ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.ActiveRate ASC,ronl.SupplierServicesFlag DESC))) as RN,
						ISNULL(ronl.[Percentage],0) [Percentage],
						ronl.ActionType ACT
					
						
					FROM TempRoutes Rt with(nolock,index=PK_TempRouteID) 
						INNER join RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
						LEFT JOIN RouteOption_NonLCRs ronl with(nolock , index=IX_RouteOptionNonLCR_multikey)  ON 
						ronl.Code = rt.Code AND ronl.CustomerID = rt.CustomerID AND ronl.SupplierID =CS.SupplierID 
						LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID=RPC.CustomerID
						LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID=RPS.SupplierID and rpc.id=rps.id
	
						WHERE 
							
							RT.ProfileID <> CS.ProfileID -- Prevent Looping
							AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
							AND CS.ActiveRate <= Rt.OurActiveRate
							AND RT.IsBlockAffected = 0
							AND RT.IsOverrideAffected = 0
							AND ( ronl.ActionType is null or ronl.ActionType = 3)
							AND cs.IsBlock = 0
									 ) q 
						  where q.rn <@MaxSuppliersPerRoute or q.ACT=4

				INSERT INTO [TempRouteOptions]  with(TABLOCK) 
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
					[Percentage]	
		     		)
					
					SELECT  
					tr.RouteID,
					cs.SupplierID,
					cs.SupplierZoneID,
					cs.SupplierActiveRate,
					cs.SupplierServicesFlag,
					cs.Priority,
					cs.NumberOfTries,
					cs.[State],
					DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp),
					cs.Percentage 
						FROM RouteOption_NonLCR cs with(nolock , index=IX_RouteOptionNonLCR_multikey) INNER join TempRoutes TR  with(nolock,index=PK_TempRouteID) 
				  ON  cs.CustomerID=tr.CustomerID  AND  cs.code=tr.code
				 WHERE 
			      (((cs.actiontype IN(5,4))  
			      or (CS.SupplierActiveRate <= tr.OurActiveRate and cs.actiontype = 2)  ) 
			      AND tr.IsBlockAffected = 0) 		
			      
			;with toupdate as (	select t.routeid , n.supplierid from RouteOption_NonLCR n with(nolock, index=IX_RouteOptionNonLCR_multikey) join 
			TempRoutes t with(nolock,index=PK_TempRouteID) on 
			t.customerid=n.customerid and t.code=n.code
			where n.actiontype=1
			)
			
			UPDATE A
			SET A.state = 0
			FROM [TempRouteOptions] A with(TABLOCK) 
			INNER JOIN toupdate B ON 
			A.Routeid=B.RouteId and A.Supplierid=B.Supplierid

End
ELSE
	
					BEGIN
					
			---- Forced Special Request are added
				SET @Message = CONVERT(varchar, getdate(), 121)
	          EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Preparing Sale Market Price', @Message
			INSERT INTO NonMarketPriceOptions 
			SELECT
						Rt.RouteID,
						CS.SupplierID
						FROM TempRoutes Rt 
						INNER join RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
						inner JOIN SaleZoneMarketPrice szmp ON szmp.SaleZoneID = rt.OurZoneID  AND CS.SupplierServicesFlag  = szmp.ServicesFlag AND Rt.OurServicesFlag = szmp.ServicesFlag
						LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID=RPC.CustomerID
						LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID=RPS.SupplierID and rpc.id=rps.id	
						WHERE 
							 (szmp.FromRate > cs.ActiveRate or szmp.ToRate < cs.ActiveRate) 
			

			
			; with RouteOption_NonLCRMP as  ( 
			select * from RouteOption_NonLCR
			where actiontype is null or actiontype=3)
			
				INSERT INTO [TempRouteOptions]  with(TABLOCK) 
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
					[Percentage]
					
				)
				select routeid,supplierid,supplierzoneid,supplieractiverate,SupplierServicesFlag,Priority,1 AS NumberOfTries,
				[State],DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp),[Percentage]
				 from 
			(
			SELECT
						Rt.RouteID,
						CS.SupplierID,
						CS.SupplierZoneID,
						CS.ActiveRate AS SupplierActiveRate,
						CS.SupplierServicesFlag,
						ISNULL(ronl.Priority,0) [Priority],
						ISNULL(ronl.[State],1) [State],
						((ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.ActiveRate ASC,ronl.SupplierServicesFlag DESC))) as RN,
						ISNULL(ronl.[Percentage],0) [Percentage],
						ronl.ActionType ACT
					
						
					FROM TempRoutes Rt with(nolock,index=PK_TempRouteID) 
						INNER join RouteOptionsPool CS	on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT
						LEFT JOIN RouteOption_NonLCRMP ronl with(nolock , index=IX_RouteOptionNonLCR_multikey)  ON 
						ronl.Code = rt.Code AND ronl.CustomerID = rt.CustomerID AND ronl.SupplierID =CS.SupplierID 
						LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID=RPC.CustomerID
						LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID=RPS.SupplierID and rpc.id=rps.id
	
						WHERE 
							
							RT.ProfileID <> CS.ProfileID -- Prevent Looping
							AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
							AND CS.ActiveRate <= Rt.OurActiveRate
							AND RT.IsBlockAffected = 0
							AND RT.IsOverrideAffected = 0
							AND ( ronl.ActionType is null or ronl.ActionType = 3)
							AND cs.IsBlock = 0
						 ) q 
						  where q.rn <@MaxSuppliersPerRoute or q.ACT=4
								
								
								
					
				
			; with RouteOption_NonLCRMPs as  ( 
			select * from RouteOption_NonLCR
			where actiontype in (5,4,2,1))
				
				


				INSERT INTO [TempRouteOptions]  with(TABLOCK) 
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
					[Percentage]
					
				)
					SELECT  
					tr.RouteID,
					cs.SupplierID,
					cs.SupplierZoneID,
					cs.SupplierActiveRate,
					cs.SupplierServicesFlag,
					cs.Priority,
					cs.NumberOfTries,
					cs.[State],
					DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp),
					cs.Percentage 
						FROM RouteOption_NonLCRMPS cs with(nolock , index=IX_RouteOptionNonLCR_multikey) INNER join TempRoutes TR  with(nolock,index=PK_TempRouteID) 
				  ON tr.CustomerID = cs.CustomerID AND tr.code = cs.code
				 WHERE 
				      (((cs.actiontype IN(5,4)) or (cs.actiontype=1 and ((CS.SupplierServicesFlag & tr.OurServicesFlag) = tr.OurServicesFlag)) or (CS.SupplierActiveRate <= tr.OurActiveRate and cs.actiontype = 2)  ) AND tr.IsBlockAffected = 0) 

				
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Appling Market Price', @Message
	
DELETE ro FROM [TempRouteOptions] ro
INNER JOIN NonMarketPriceOptions mp ON ro.RouteID = mp.RouteID where ro.SupplierID = mp.SupplierID
	END
		SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: RouteOptions Inserted', @Message
	set @UpdateStamp =getdate()

	IF(@CheckRouteBlocks = 'Y')
	BEGIN
	
		;with BlockedOptions AS

	(
		SELECT DISTINCT
		rt.CustomerID,
			rt.Code,
			Rt.RouteID,
			8 AS priority,
			1 AS NumberOfTries,
			0 [State],
			0 AS percentage,
			ActionType
			
		FROM
				TempRoutes Rt 
			INNER JOIN RouteOption_NonLCR ronl ON 
			ronl.Code = rt.Code AND ronl.CustomerID = rt.CustomerID --AND ronl.SupplierID =CS.SupplierID 
			WHERE 
				ronl.ActionType = 1 AND ronl.SupplierID = 'BLK'
				
	)
INSERT INTO TempRouteOptions with(TABLOCK) 
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
		'BLK',
		NULL,
		-1,
		1,
		8,
		NumberOfTries,
		[State],
		DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp),
		Percentage
	FROM BlockedOptions
	
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: RouteOptions Blocked Inserted', @Message
	set @UpdateStamp = getdate()	
	END

END