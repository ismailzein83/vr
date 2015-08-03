-- =================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-30
-- Updated: 2010-04-26 (Fixed Problem with Route Override On Our Zone when partially matching Supplier Zone Is Blocked)
-- Updated: 2010-05-17 (fixed blocked and onactive customers, Reverted to old way of managing overrides on our zones)
-- Updated: 2012-09-26 (fixed conflicts between overrides done by Zone, Code with subcodes and single codes)
-- Description:	Updates Routes / Route Options from Route Overrides
-- =================================================================
CREATE   PROCEDURE dbo.bp_UpdateRoutingFromOverrides_WithPercentage_New19102012 
	@CustomerID VARCHAR(10) = NULL,
	@Code VARCHAR(20) = NULL,
	@IncludeSubCodes CHAR(1) = NULL,
	@ExcludedCodes VARCHAR(200) = NULL,
	@OurZoneID INT = NULL,
	@RouteOptions VARCHAR(100) = NULL,
	@BlockedSuppliers VARCHAR(100) = NULL,
	@RouteBatch BIGINT = 1000000
AS
BEGIN
	--SET NOCOUNT ON;

	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	CREATE TABLE #tmpRO (CustomerID nvarchar(5), Code nvarchar(15), IncludeSubCodes CHAR(1),ExcludedCodes VARCHAR(100),OurZoneID int, SupplierID nvarchar(5), Percentage INT, Position INT, SupplierZoneID INT NULL)
	CREATE TABLE #tmpRO_ZonesToCodes (CustomerID nvarchar(5), Code nvarchar(15), IncludeSubCodes CHAR(1),ExcludedCodes VARCHAR(100),OurZoneID int, SupplierID nvarchar(5), Percentage INT, Position INT, SupplierZoneID INT NULL)
	CREATE INDEX IDX_Customer ON #tmpRO (CustomerID)
	CREATE INDEX IDX_Code ON #tmpRO (Code)
	CREATE INDEX IDX_OurZoneID ON #tmpRO (OurZoneID)
	CREATE INDEX IDX_SupplierZoneID ON #tmpRO (SupplierZoneID)
	CREATE INDEX IDX_SupplierID ON #tmpRO (SupplierID)
	
	CREATE INDEX IDX_Customer_ ON #tmpRO_ZonesToCodes (CustomerID)
	CREATE INDEX IDX_Code_ ON #tmpRO_ZonesToCodes (Code)
	CREATE INDEX IDX_OurZoneID_ ON #tmpRO_ZonesToCodes (OurZoneID)
	CREATE INDEX IDX_SupplierZoneID_ ON #tmpRO_ZonesToCodes (SupplierZoneID)
	CREATE INDEX IDX_SupplierID_ ON #tmpRO_ZonesToCodes (SupplierID)
	
	DECLARE @ValidCustomers TABLE(CustomerID NVARCHAR(5) PRIMARY KEY)
	INSERT INTO @ValidCustomers(CustomerID) 
		SELECT ca.CarrierAccountID FROM CarrierAccount ca 
	        WHERE ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound)
	        AND ca.ActivationStatus NOT IN (@Account_Inactive)
	        AND ca.IsDeleted = 'N' 

	DECLARE @MaxSuppliersPerRoute INT 
	SET @MaxSuppliersPerRoute = 8

	-- Process All Routing Overrides 

			INSERT INTO #tmpRO (CustomerID, Code, IncludeSubCodes, ExcludedCodes, OurZoneID, Percentage,SupplierID, Position)
			SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) SupplierID,
					1 as Position 
					
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)				  
			UNION
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) SupplierID,
					2 as Position 
					
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)  
			UNION	
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) SupplierID,
					3 as Position 
					
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)
			UNION
			SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
				(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) SupplierID,
					4 as Position 
					
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)  
			UNION
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) SupplierID,
					5 as Position 
					
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)  
			UNION	
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) SupplierID,
					6 as Position 
					
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)
			ORDER BY ro.CustomerID, ro.Code, Position

------------- Begin expand zone base overrides as a codes ------------- 		
           INSERT INTO #tmpRO_ZonesToCodes 
           SELECT tr.CustomerID, cm.Code, tr.IncludeSubCodes, tr.ExcludedCodes, tr.OurZoneID,
                tr.SupplierID, tr.Percentage, tr.Position, tr.SupplierZoneID
           FROM #tmpRO tr, CodeMatch cm WITH(NOLOCK)
           WHERE cm.SupplierZoneID = tr.OurZoneID
           AND cm.SupplierID ='SYS' AND tr.Code = '*ALL*'; 
------------- End expand zone base overrides as a codes ------------- 		


------------- Begin expand code with subcodes base overrides as a codes ------------- 		
		With CodesWithSub As(
			SELECT tr.CustomerID, cm.Code, IncludeSubCodes, tr.ExcludedCodes, tr.OurZoneID,
				tr.SupplierID, tr.Percentage, tr.Position, tr.SupplierZoneID,tr.Code GroupCode
			FROM #tmpRO tr, CodeMatch cm WITH(NOLOCK)
			WHERE cm.SupplierID ='SYS' 
			AND   cm.Code LIKE tr.Code +'%' COLLATE Latin1_General_BIN
			AND 1= ( CASE WHEN  PATINDEX('%,%',tr.ExcludedCodes) > 0  AND 
			   cm.Code NOT IN 
			   (SELECT * FROM dbo.ParseArray(tr.ExcludedCodes,','))
			   THEN 1
			WHEN  PATINDEX('%,%',tr.ExcludedCodes) = 0 AND  
			 cm.Code NOT LIKE tr.ExcludedCodes COLLATE Latin1_General_BIN THEN 1

			   ELSE 0 END )					 
			AND tr.IncludeSubCodes='Y'

			)
			,
		GroupCodes As (
						Select CustomerID, Code,MAX(Groupcode) Groupcode 
						from CodesWithSub 
						group by CustomerID, Code
						)

		,FinalCodesWithSub As( 
						Select CodesWithSub.* from CodesWithSub,GroupCodes
						Where CodesWithSub.CustomerID=GroupCodes.CustomerID
						And CodesWithSub.Code=GroupCodes.Code
						And CodesWithSub.Groupcode=GroupCodes.Groupcode
						)
           INSERT INTO #tmpRO_ZonesToCodes 
           SELECT FCS.CustomerID, FCS.Code, FCS.IncludeSubCodes, FCS.ExcludedCodes, FCS.OurZoneID,
                FCS.SupplierID, FCS.Percentage, FCS.Position, FCS.SupplierZoneID
           FROM FinalCodesWithSub  FCS WITH(NOLOCK)

------------- End expand code with subcodes base overrides as a codes ------------- 		

------------- Remove Zone & code with subcodes base overrides from #tmpRO ------------- 		
           DELETE FROM #tmpRO WHERE OurZoneID > 0 
           DELETE FROM #tmpRO WHERE IncludeSubCodes='Y' 
           
------------- Inserting expanded Codes #tmpRO ------------- 		

           INSERT INTO #tmpRO SELECT trztc.CustomerID, trztc.Code,
                                     trztc.IncludeSubCodes, trztc.ExcludedCodes,
                                     trztc.OurZoneID, trztc.SupplierID,
                                     trztc.Percentage, trztc.Position,
                                     trztc.SupplierZoneID
                                FROM #tmpRO_ZonesToCodes trztc 
           
------------- Removing Duplicates from #tmpRO ------------- 		
          ;WITH Duplicates AS
						(
						   SELECT CustomerID, Code, IncludeSubCodes, ExcludedCodes,
						          OurZoneID, SupplierID, Percentage, Position,
						          SupplierZoneID,
						ROW_NUMBER() OVER (PARTITION BY CustomerID,code,SupplierID ORDER BY Code DESC) as cnt
						FROM
						#tmpRO
						)
						DELETE FROM
						Duplicates
						WHERE
						cnt > 1;
          
-------------  Set the Matching Supplier Zones from Codes in RO based on direct one to one code match
			UPDATE #tmpRO
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO tr, CodeMatch cm 
			WHERE tr.Code = cm.Code COLLATE Latin1_General_BIN AND tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			-- Set the Matching Supplier Zones based on supplier our zone match
			UPDATE #tmpRO
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO tr, CodeMatch cm,ZoneMatch zm 
			WHERE   tr.SupplierZoneID IS NULL      
				  AND tr.OurZoneID = zm.OurZoneID 
			      AND  zm.SupplierZoneID = cm.SupplierZoneID 
			      AND  tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			-- Insert Route Overrides as Codes instead of Zones to match with supplier zones
			--INSERT INTO #tmpRO (CustomerID, Code, OurZoneID, SupplierID, SupplierZoneID, Percentage, Position)
			--SELECT ro.CustomerID, oc.Code, ro.OurZoneID, ro.SupplierID, sc.SupplierZoneID, ro.Percentage, ro.Position 
			--	FROM #tmpRO ro WITH(NOLOCK, INDEX(IDX_Code), INDEX(IDX_SupplierZoneID), INDEX(IDX_SupplierID), INDEX(IDX_OurZoneID)), 
			--		CodeMatch oc WITH(NOLOCK, INDEX(IDX_CodeMatch_Zone)),  
			--		ZoneMatch zm WITH(NOLOCK, INDEX(IX_ZoneMatch_SupplierZoneID)),
			--		CodeMatch sc WITH(NOLOCK, INDEX(IDX_CodeMatch_Code), INDEX(IDX_CodeMatch_Supplier)),
			--		ZoneRate zr WITH(NOLOCK)
			--	WHERE 
			--		--ro.SupplierZoneID IS NULL
			--		 ro.Code = '*ALL*' 
			--		AND oc.SupplierZoneID = ro.SupplierZoneID 
			--		AND zm.OurZoneID = ro.OurZoneID
			--		AND sc.Code = oc.Code
			--		AND ro.SupplierID = sc.SupplierID COLLATE Latin1_General_CI_AI
			--		AND zm.SupplierZoneID = sc.SupplierZoneID
------------------ Begin Clean Zone and Code override Conflict ------------------
            
			Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			SELECT r.code
			  FROM [Route] r, #tmpRO rov, CarrierAccount ca  
			WHERE 
				rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
				AND   r.Code LIKE rov.Code +'%'  COLLATE Latin1_General_BIN
					AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
	                   R.Code NOT IN 
	                   (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
	                   THEN 1
	              WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
 			         R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN  THEN 1

	                   ELSE 0 END )	 
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.IncludeSubCodes ='Y' AND rov.CustomerID = #tmpRO.CustomerID
				) and #tmpRO.OurZoneID>0



			Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			SELECT r.code
			  FROM [Route] r, #tmpRO rov, CarrierAccount ca 
			WHERE 
				rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
				AND rov.Code = r.Code COLLATE Latin1_General_BIN
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.IncludeSubCodes ='N' and rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO.CustomerID
				)and #tmpRO.OurZoneID>0 


			Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			SELECT r.code
			  FROM [Route] r, #tmpRO rov, CarrierAccount ca 
			WHERE 
				rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
				AND rov.Code = r.Code COLLATE Latin1_General_BIN
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.IncludeSubCodes ='N' and rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO.CustomerID
				)and #tmpRO.IncludeSubCodes='Y'

------------------ End Clean Zone and Code override Conflict ------------------

            
      		DELETE FROM #tmpRO WHERE SupplierZoneID IS NULL

			DECLARE @MinRouteID bigint
			DECLARE @MaxRouteID bigint
			DECLARE @CurrentRouteID bigint
			DECLARE @ProcessedRoutes bigint 
			DECLARE @TotalRoutes bigint
			DECLARE @NextRouteID bigint
			DECLARE @Message VARCHAR(4000)
				
			SELECT @MinRouteID = Min(RouteID), @MaxRouteID = Max(RouteID), @TotalRoutes = Count(*) FROM [Route]

			SET @CurrentRouteID = @MinRouteID
			
			SET @ProcessedRoutes = 0
			
			WHILE @CurrentRouteID <= @MaxRouteID
			BEGIN
				
				SET @NextRouteID = @CurrentRouteID + @RouteBatch - 1


				/*************************************
				* Delete Overrides / Options 
				*************************************/
				BEGIN TRANSACTION 
			
				DELETE [RouteOption] 
					FROM [Route] r, RouteOverride rov, [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.RouteOptions IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND   r.Code LIKE rov.Code +'%' COLLATE Latin1_General_BIN
							AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
			                   R.Code NOT IN 
			                   (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
			                   THEN 1
			              WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
         			         R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN THEN 1
		
			                   ELSE 0 END )				
						AND [RouteOption].RouteID = r.RouteID
						AND rov.IncludeSubCodes = 'Y'
						
				
				DELETE [RouteOption] 
					FROM [Route] r, RouteOverride rov, [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.RouteOptions IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND   r.Code  = rov.Code	
						AND [RouteOption].RouteID = r.RouteID
						AND rov.IncludeSubCodes = 'N'

				DELETE [RouteOption] 
					FROM [Route] r, RouteOverride rov, [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.RouteOptions IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.OurZoneID = r.OurZoneID 
						AND [RouteOption].RouteID = r.RouteID
				
				COMMIT
				
				/*************************************
				* Insert Overrides / Options 
				*************************************/

				BEGIN TRANSACTION 
				
				-- Insert Override Options (Code based)
				INSERT INTO [RouteOption] (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Percentage, Priority, [State], NumberOfTries, Updated)
					SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag,rov.Percentage, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
					  FROM [Route] r, #tmpRO rov, CarrierAccount ca  
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
						AND rov.Code = r.Code COLLATE Latin1_General_BIN
						AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
						AND ca.ActivationStatus <> @Account_Inactive
						AND ca.RoutingStatus <> @Account_Blocked
						AND ca.RoutingStatus <> @Account_BlockedOutbound
						AND ca.IsDeleted = 'N'
							
				
				COMMIT



				BEGIN TRANSACTION 
				
				/*************************************
				* Process Blocks 
				*************************************/
				-- include subcodes comes first
				UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
					FROM [Route] r, RouteOverride rov, [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.BlockedSuppliers IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND   r.Code LIKE rov.Code +'%' COLLATE Latin1_General_BIN
							AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
			                   R.Code NOT IN 
			                   (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
			                   THEN 1
			              WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
         			         R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN THEN 1
		
			                   ELSE 0 END )					 
						AND [RouteOption].RouteID = r.RouteID
						AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)
						AND rov.IncludeSubCodes='Y'
						
				UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
					FROM [Route] r, RouteOverride rov, [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.BlockedSuppliers IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.Code = r.Code					 
						AND [RouteOption].RouteID = r.RouteID
						AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)
						AND rov.IncludeSubCodes='N'
						
				UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
					FROM [Route] r, RouteOverride rov, [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.BlockedSuppliers IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.OurZoneID = r.OurZoneID 
						AND [RouteOption].RouteID = r.RouteID
						AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)

				COMMIT


				SET @ProcessedRoutes = @ProcessedRoutes + @RouteBatch
				IF @ProcessedRoutes > @TotalRoutes SET @ProcessedRoutes = @TotalRoutes
				SET @CurrentRouteID = @CurrentRouteID + @RouteBatch
				
				SET @Message = cast(@ProcessedRoutes AS varchar) + ' / ' + cast(@TotalRoutes AS varchar)
				EXEC bp_SetSystemMessage 'BuildRoutes: Route Overrides Processing', @Message
				
			END		

	-- A particular Route Override
	
	DROP TABLE #tmpRO
	DROP TABLE #tmpRO_ZonesToCodes
END