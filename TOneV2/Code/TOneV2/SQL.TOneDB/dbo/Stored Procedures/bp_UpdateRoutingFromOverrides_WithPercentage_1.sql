-- =================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-30
-- Updated: 2010-04-26 (Fixed Problem with Route Override On Our Zone when partially matching Supplier Zone Is Blocked)
-- Updated: 2010-05-17 (fixed blocked and onactive customers, Reverted to old way of managing overrides on our zones)
-- Updated: 2012-09-26 (fixed conflicts between overrides done by Zone, Code with subcodes and single codes)
-- Description:	Updates Routes / Route Options from Route Overrides
-- =================================================================
CREATE   PROCEDURE [dbo].[bp_UpdateRoutingFromOverrides_WithPercentage] 
	@CustomerID VARCHAR(10) = NULL,
	@Code VARCHAR(20) = '',
	@IncludeSubCodes CHAR(1) = NULL,
	@ExcludedCodes VARCHAR(200) = NULL,
	@OurZoneID INT = NULL,
	@RouteOptions VARCHAR(100) = NULL,
	@BlockedSuppliers VARCHAR(100) = NULL,
	@RouteBatch BIGINT = 1000000
AS
BEGIN
	--SET NOCOUNT ON;
	Declare @TargetCode varchar(20)
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

	CREATE TABLE #tmpRO (CustomerID nvarchar(5), Code nvarchar(20), IncludeSubCodes CHAR(1),ExcludedCodes VARCHAR(100),OurZoneID int, SupplierID nvarchar(5),BlockedSuppliers varchar(1024),Percentage INT, Position INT, SupplierZoneID INT NULL)
	CREATE TABLE #tmpRO_ZonesToCodes (CustomerID nvarchar(5), Code nvarchar(20), IncludeSubCodes CHAR(1),ExcludedCodes VARCHAR(100),OurZoneID int, SupplierID nvarchar(5),BlockedSuppliers varchar(1024), Percentage INT, Position INT, SupplierZoneID INT NULL)
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

	IF @CustomerID IS NULL
			INSERT INTO @ValidCustomers(CustomerID) 
				SELECT ca.CarrierAccountID FROM CarrierAccount ca  with(nolock) 
					WHERE ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound)
					AND ca.ActivationStatus NOT IN (@Account_Inactive)
					AND ca.IsDeleted = 'N' 
	ELSE
		Begin
			INSERT INTO @ValidCustomers(CustomerID) 
				SELECT ca.CarrierAccountID FROM CarrierAccount ca with(nolock)
					WHERE ca.CarrierAccountID= @CustomerID and ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound)
					AND ca.ActivationStatus NOT IN (@Account_Inactive)
					AND ca.IsDeleted = 'N' 
			if @OurZoneID <>-1
				Select @TargetCode = isnull(CodeGroup+'%','%') from zone where ZoneID=@OurZoneID
			else 
				begin 
					if @IncludeSubCodes ='Y'
						Set @TargetCode=@Code+'%'
					else
						Set @TargetCode=@Code
				End
		End
	DECLARE @MaxSuppliersPerRoute INT 
	SET @MaxSuppliersPerRoute = 8

	select 'Beginprocess',GETDATE()
	-- Process All Routing Overrides 
	IF @CustomerID IS NULL
		BEGIN

			INSERT INTO #tmpRO with (tablock) (CustomerID, Code, IncludeSubCodes, ExcludedCodes, OurZoneID, Percentage,SupplierID,BlockedSuppliers, Position) 
			SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) SupplierID,ro.BlockedSuppliers,
					1 as Position 
					
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y' 				  
			UNION
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) SupplierID,ro.BlockedSuppliers,
					2 as Position 
					
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y' 				  
			UNION	
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) SupplierID,ro.BlockedSuppliers,
					3 as Position 
					
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y'				  
			UNION
			SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
				(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) SupplierID,ro.BlockedSuppliers,
					4 as Position 
					
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y' 				  
			UNION
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) SupplierID,ro.BlockedSuppliers,
					5 as Position 
					
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y' 
			UNION	
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
					(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) Percentage,
					(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) SupplierID,ro.BlockedSuppliers,
					6 as Position 
					
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y'
			ORDER BY ro.CustomerID, ro.Code, Position
		End
	Else
		Begin
			if @OurZoneID =-1
				Begin
					INSERT INTO #tmpRO with (tablock) (CustomerID, Code, IncludeSubCodes, ExcludedCodes, OurZoneID, Percentage,SupplierID,BlockedSuppliers, Position) 
					SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) SupplierID,ro.BlockedSuppliers,
							1 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And Code like @TargetCode And IsEffective='Y' 				  
					UNION
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) SupplierID,ro.BlockedSuppliers,
							2 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And Code like @TargetCode And IsEffective='Y' 				  
					UNION	
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) SupplierID,ro.BlockedSuppliers,
							3 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And Code like @TargetCode And IsEffective='Y'				  
					UNION
					SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
						(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) SupplierID,ro.BlockedSuppliers,
							4 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And Code like @TargetCode And IsEffective='Y' 				  
					UNION
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) SupplierID,ro.BlockedSuppliers,
							5 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And Code like @TargetCode And IsEffective='Y' 
					UNION	
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) SupplierID,ro.BlockedSuppliers,
							6 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And Code like @TargetCode And IsEffective='Y'
					ORDER BY ro.CustomerID, ro.Code, Position

				End
			Else
				Begin
					INSERT INTO #tmpRO with (tablock) (CustomerID, Code, IncludeSubCodes, ExcludedCodes, OurZoneID, Percentage,SupplierID,BlockedSuppliers, Position) 
					SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) SupplierID,ro.BlockedSuppliers,
							1 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And ((Code like @TargetCode) or ro.OurZoneID=@OurZoneID) And IsEffective='Y' 				  
					UNION
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) SupplierID,ro.BlockedSuppliers,
							2 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And ((Code like @TargetCode) or ro.OurZoneID=@OurZoneID) And IsEffective='Y' 				  
					UNION	
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) SupplierID,ro.BlockedSuppliers,
							3 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And ((Code like @TargetCode) or ro.OurZoneID=@OurZoneID) And IsEffective='Y'				  
					UNION
					SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
						(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) SupplierID,ro.BlockedSuppliers,
							4 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And ((Code like @TargetCode) or ro.OurZoneID=@OurZoneID) And IsEffective='Y' 				  
					UNION
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) SupplierID,ro.BlockedSuppliers,
							5 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And ((Code like @TargetCode) or ro.OurZoneID=@OurZoneID) And IsEffective='Y' 
					UNION	
						SELECT 
							ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,
							(SELECT o.Percentage FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) Percentage,
							(SELECT o.[value] FROM dbo.ParseOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) SupplierID,ro.BlockedSuppliers,
							6 as Position 
							
						FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) IS NOT NULL
							AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And ((Code like @TargetCode) or ro.OurZoneID=@OurZoneID) And IsEffective='Y'
					ORDER BY ro.CustomerID, ro.Code, Position

				End			
		End
		--IF @CustomerID IS not NULL and @OurZoneID <>-1
		--	Set @TargetCode=''
				
	select 'Begin Zone Expand',GETDATE()
------------- Begin expand zone base overrides as a codes ------------- 		
           INSERT INTO #tmpRO_ZonesToCodes with (tablock)
           SELECT tr.CustomerID, cm.Code, tr.IncludeSubCodes, tr.ExcludedCodes, tr.OurZoneID,
                tr.SupplierID,tr.BlockedSuppliers, tr.Percentage, tr.Position, tr.SupplierZoneID
           FROM #tmpRO tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK)
           WHERE cm.SupplierZoneID = tr.OurZoneID
           AND cm.SupplierID ='SYS' AND tr.Code = '*ALL*' 
------------- End expand zone base overrides as a codes ------------- 		


------------- Begin expand code with subcodes base overrides as a codes ------------- 		
	select 'Begin Code Expand',GETDATE()

		;With CodesWithSub As(
			SELECT tr.CustomerID, cm.Code, IncludeSubCodes, tr.ExcludedCodes, tr.OurZoneID,
				tr.SupplierID,tr.BlockedSuppliers, tr.Percentage, tr.Position, tr.SupplierZoneID,tr.Code GroupCode
			FROM #tmpRO tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK)
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
						from CodesWithSub WITH(NOLOCK) 
						group by CustomerID, Code
						)

		,FinalCodesWithSub As( 
						Select CodesWithSub.* from CodesWithSub WITH(NOLOCK),GroupCodes WITH(NOLOCK)
						Where CodesWithSub.CustomerID=GroupCodes.CustomerID
						And CodesWithSub.Code=GroupCodes.Code
						And CodesWithSub.Groupcode=GroupCodes.Groupcode
						)
           INSERT INTO #tmpRO_ZonesToCodes with (tablock)
           SELECT FCS.CustomerID, FCS.Code, FCS.IncludeSubCodes, FCS.ExcludedCodes, FCS.OurZoneID,
                FCS.SupplierID,FCS.BlockedSuppliers, FCS.Percentage, FCS.Position, FCS.SupplierZoneID
           FROM FinalCodesWithSub  FCS WITH(NOLOCK)

------------- End expand code with subcodes base overrides as a codes ------------- 		

------------- Remove Zone & code with subcodes base overrides from #tmpRO ------------- 		
           DELETE FROM #tmpRO WHERE OurZoneID > 0 
           DELETE FROM #tmpRO WHERE IncludeSubCodes='Y' 
           
------------- Inserting expanded Codes #tmpRO ------------- 		

           INSERT INTO #tmpRO with (tablock) SELECT trztc.CustomerID, trztc.Code,
                                     trztc.IncludeSubCodes, trztc.ExcludedCodes,
                                     trztc.OurZoneID, trztc.SupplierID,trztc.BlockedSuppliers,
                                     trztc.Percentage, trztc.Position,
                                     trztc.SupplierZoneID
                                FROM #tmpRO_ZonesToCodes trztc WITH(NOLOCK) 
           
-------------  Set the Matching Supplier Zones from Codes in RO based on direct one to one code match
			UPDATE #tmpRO
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK) 
			WHERE tr.Code = cm.Code COLLATE Latin1_General_BIN AND tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			-- Set the Matching Supplier Zones based on supplier our zone match
			UPDATE #tmpRO
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK),ZoneMatch zm WITH(NOLOCK) 
			WHERE   tr.SupplierZoneID IS NULL      
				  AND tr.OurZoneID = zm.OurZoneID 
			      AND  zm.SupplierZoneID = cm.SupplierZoneID 
			      AND  tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			--select * from #tmpRO order by code
			select 'beginclean',GETDATE()

------------------ Begin Clean Zone and Code Override Conflict ------------------
			--Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			--SELECT r.code
			--  FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK)  
			--WHERE r.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And
			--	rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
			--	AND   r.Code LIKE rov.Code +'%'  COLLATE Latin1_General_BIN
			--		AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
	  --                 R.Code NOT IN 
	  --                 (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
	  --                 THEN 1
	  --            WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
 		--	         R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN  THEN 1

	  --                 ELSE 0 END )	 
			--	AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
			--	AND ca.ActivationStatus <> @Account_Inactive
			--	AND ca.RoutingStatus <> @Account_Blocked
			--	AND ca.RoutingStatus <> @Account_BlockedOutbound
			--	AND ca.IsDeleted = 'N'
			--	AND rov.IncludeSubCodes ='Y' AND rov.CustomerID = #tmpRO.CustomerID
			--	) and #tmpRO.OurZoneID>0



			--Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			--SELECT r.code
			--  FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK) 
			--WHERE r.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And
			--	rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
			--	AND rov.Code = r.Code COLLATE Latin1_General_BIN
			--	AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
			--	AND ca.ActivationStatus <> @Account_Inactive
			--	AND ca.RoutingStatus <> @Account_Blocked
			--	AND ca.RoutingStatus <> @Account_BlockedOutbound
			--	AND ca.IsDeleted = 'N'
			--	AND rov.IncludeSubCodes ='N' and rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO.CustomerID
			--	)and #tmpRO.OurZoneID>0 

			--Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			--SELECT r.code
			--  FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK) 
			--WHERE r.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And
			--	rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
			--	AND rov.Code = r.Code COLLATE Latin1_General_BIN
			--	AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
			--	AND ca.ActivationStatus <> @Account_Inactive
			--	AND ca.RoutingStatus <> @Account_Blocked
			--	AND ca.RoutingStatus <> @Account_BlockedOutbound
			--	AND ca.IsDeleted = 'N'
			--	AND rov.IncludeSubCodes ='N' and rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO.CustomerID
			--	)and #tmpRO.IncludeSubCodes='Y'

			Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			SELECT rov.code
			  FROM #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK) 
			WHERE rov.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) 
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO.CustomerID
				)and #tmpRO.OurZoneID>0 

			Delete #tmpRO where code  COLLATE Latin1_General_BIN in(
			SELECT rov.code
			  FROM #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK) 
			WHERE rov.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) 
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.IncludeSubCodes ='N' and rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO.CustomerID
				)and #tmpRO.IncludeSubCodes='Y'

------------------ End Clean Zone and Code override Conflict ------------------
			--select * from #tmpRO order by code
			select 'Actions',GETDATE()

            
      		--DELETE FROM #tmpRO WHERE SupplierZoneID IS NULL
			DECLARE @MinRouteID bigint
			DECLARE @MaxRouteID bigint
			DECLARE @CurrentRouteID bigint
			DECLARE @ProcessedRoutes bigint 
			DECLARE @TotalRoutes bigint
			DECLARE @NextRouteID bigint
			DECLARE @Message VARCHAR(4000)
				
			IF @CustomerID IS NULL
				Begin
					SELECT @MinRouteID = Min(RouteID), @MaxRouteID = Max(RouteID), @TotalRoutes = Count(*) FROM [Route] WITH(NOLOCK)
					--SELECT @MinRouteID , @MaxRouteID ,@MaxRouteID -@MinRouteID,@TargetCode
					
					SET @CurrentRouteID = @MinRouteID
					
					SET @ProcessedRoutes = 0
					
					WHILE @CurrentRouteID <= @MaxRouteID
					BEGIN
						
						SET @NextRouteID = @CurrentRouteID + @RouteBatch - 1

			select 'Begin Delete Actions',GETDATE()
						/*************************************
						* Delete Overrides / Options 
						*************************************/
						BEGIN TRANSACTION 
					
							DELETE [RouteOption] 
								FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
								WHERE 
										r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
									AND rov.RouteOptions IS NOT NULL 
									AND rov.CustomerID = r.CustomerID
									AND r.Code LIKE rov.Code +'%' COLLATE Latin1_General_BIN
										AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
										   R.Code NOT IN 
										   (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
										   THEN 1
									  WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
 										 R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN THEN 1
					
										   ELSE 0 END )				
									AND [RouteOption].RouteID = r.RouteID
									AND rov.IncludeSubCodes = 'Y' And rov.IsEffective='Y'
								
							DELETE [RouteOption] 
								FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
								WHERE 
										r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
									AND rov.RouteOptions IS NOT NULL 
									AND rov.CustomerID = r.CustomerID
									AND   r.Code  = rov.Code	
									AND [RouteOption].RouteID = r.RouteID
									AND rov.IncludeSubCodes = 'N' And rov.IsEffective='Y'

							DELETE [RouteOption] 
								FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
								WHERE 
										r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
									AND rov.RouteOptions IS NOT NULL 
									AND rov.CustomerID = r.CustomerID
									AND rov.OurZoneID = r.OurZoneID 
									AND [RouteOption].RouteID = r.RouteID And rov.IsEffective='Y'
					COMMIT

					select 'End Delete Actions',GETDATE()

						/*************************************
						* Insert Overrides / Options 
						*************************************/

						BEGIN TRANSACTION 
						
						-- Insert Override Options (Code based)
						INSERT INTO [RouteOption] (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Percentage, Priority, [State], NumberOfTries, Updated)
							SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag,rov.Percentage, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
							  FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK)  
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

					select 'End Insert Actions',GETDATE()

						BEGIN TRANSACTION 
						
						/*************************************
						* Process Blocks 
						*************************************/
						-- include subcodes comes first
							UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
								FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
								WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
									AND rov.BlockedSuppliers IS NOT NULL 
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
									And rov.IsEffective='Y'
									
							UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
								FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
								WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
									AND rov.BlockedSuppliers IS NOT NULL 
									AND rov.CustomerID = r.CustomerID
									AND rov.Code = r.Code					 
									AND [RouteOption].RouteID = r.RouteID
									AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)
									AND rov.IncludeSubCodes='N' 
									And rov.IsEffective='Y'
									
							UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
								FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
								WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
									AND rov.BlockedSuppliers IS NOT NULL 
									AND rov.CustomerID = r.CustomerID
									AND rov.OurZoneID = r.OurZoneID 
									AND [RouteOption].RouteID = r.RouteID
									AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)
									And rov.IsEffective='Y'
						COMMIT

					select 'End Block Actions',GETDATE()

							SET @ProcessedRoutes = @ProcessedRoutes + @RouteBatch
							IF @ProcessedRoutes > @TotalRoutes SET @ProcessedRoutes = @TotalRoutes
							SET @CurrentRouteID = @CurrentRouteID + @RouteBatch
							
							SET @Message = cast(@ProcessedRoutes AS varchar) + ' / ' + cast(@TotalRoutes AS varchar)
							EXEC bp_SetSystemMessage 'BuildRoutes: Route Overrides Processing', @Message
					End
				End
			ELSE
				Begin
						DECLARE @UpdateStamp DATETIME 
						SET @UpdateStamp = GETDATE()

					SELECT @MinRouteID = Min(RouteID), @MaxRouteID = Max(RouteID), @TotalRoutes = Count(*) FROM [Route]  WITH(NOLOCK) Where CustomerId=@CustomerId And (Code like @TargetCode) 
					--SELECT @MinRouteID , @MaxRouteID , @TotalRoutes ,@CustomerId ,@TargetCode

					--SELECT r.*,rov.* 
					--	FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK)
					--	WHERE 
					--			r.RouteID BETWEEN @MinRouteID AND @MaxRouteID
					--		AND rov.CustomerID = @CustomerID
					--		AND r.CustomerID = @CustomerID
					--		AND   r.Code=rov.Code COLLATE Latin1_General_BIN
							--AND [RouteOption].RouteID = r.RouteID
					

					BEGIN TRANSACTION 
					DELETE [RouteOption] 
						FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
						WHERE 
								r.RouteID BETWEEN @MinRouteID AND @MaxRouteID
							AND rov.CustomerID = @CustomerID
							AND r.CustomerID = @CustomerID
							AND   r.Code=rov.Code COLLATE Latin1_General_BIN
							AND [RouteOption].RouteID = r.RouteID

					--DELETE [RouteOption] 
					--	FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					--	WHERE 
					--			r.RouteID BETWEEN @MinRouteID AND @NextRouteID
					--		AND rov.RouteOptions IS NOT NULL 
					--		AND rov.CustomerID = @CustomerID
					--		AND r.CustomerID = @CustomerID
					--		AND   r.Code LIKE @TargetCode
					--		AND   r.Code LIKE rov.Code +'%' COLLATE Latin1_General_BIN
					--			AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
					--			   R.Code NOT IN 
					--			   (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
					--			   THEN 1
					--		  WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
					--			 R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN THEN 1
			
					--			   ELSE 0 END )				
					--		AND [RouteOption].RouteID = r.RouteID
					--		AND rov.IncludeSubCodes = 'Y' And rov.IsEffective='Y'
					----DELETE [RouteOption] 
					----	FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					----	WHERE 
					----			r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
					----		AND rov.RouteOptions IS NOT NULL 
					----		AND rov.CustomerID = @CustomerID
					----		AND r.CustomerID = @CustomerID
					----		AND   r.Code =@TargetCode
					----		AND   r.Code  = rov.Code	
					----		AND [RouteOption].RouteID = r.RouteID
					----		AND rov.IncludeSubCodes = 'N' And rov.IsEffective='Y'
					----if @OurZoneID<>-1
					----	DELETE [RouteOption] 
					----		FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					----		WHERE 
					----				r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
					----			AND rov.RouteOptions IS NOT NULL 
					----			AND rov.CustomerID = @CustomerID
					----			AND r.CustomerID = @CustomerID
					----			AND rov.OurZoneID = r.OurZoneID 
					----			AND r.OurZoneID =@OurZoneID
					----			AND [RouteOption].RouteID = r.RouteID And rov.IsEffective='Y'
					COMMIT

					BEGIN TRANSACTION 
					-- Insert Override Options (Code based)
						INSERT INTO [RouteOption] (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Percentage, Priority, [State], NumberOfTries, Updated)
							SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag,rov.Percentage, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
							  FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK)  
							WHERE 
									r.RouteID BETWEEN @MinRouteID AND @MaxRouteID
								AND rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
								AND rov.Code = r.Code COLLATE Latin1_General_BIN
								AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
								AND ca.ActivationStatus <> @Account_Inactive
								AND ca.RoutingStatus <> @Account_Blocked
								AND ca.RoutingStatus <> @Account_BlockedOutbound
								AND ca.IsDeleted = 'N'
					COMMIT

					BEGIN TRANSACTION 
					UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
						FROM [Route] r WITH(NOLOCK), #tmpRO rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
						WHERE r.RouteID BETWEEN @MinRouteID AND @MaxRouteID
							AND rov.CustomerID = @CustomerID
							AND r.CustomerID = @CustomerID
							AND rov.Code = r.Code COLLATE Latin1_General_BIN
							AND [RouteOption].RouteID = r.RouteID
							AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)

					--UPDATE [RouteOption] SET [State] = @State_Blocked, Updated = GETDATE()
					--	FROM [Route] r WITH(NOLOCK), RouteOverride rov WITH(NOLOCK), [RouteOption] WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					--	WHERE r.RouteID BETWEEN @MinRouteID AND @MaxRouteID
					--		AND rov.BlockedSuppliers IS NOT NULL 
					--		AND rov.CustomerID = @CustomerID
					--		AND r.CustomerID = @CustomerID
					--		AND   r.Code LIKE @TargetCode+'%'
					--		AND   r.Code LIKE rov.Code +'%' COLLATE Latin1_General_BIN
					--			AND 1= ( CASE WHEN  PATINDEX('%,%',rov.ExcludedCodes) > 0  AND 
					--			   R.Code NOT IN 
					--			   (SELECT * FROM dbo.ParseArray(rov.ExcludedCodes,','))
					--			   THEN 1
					--		  WHEN  PATINDEX('%,%',rov.ExcludedCodes) = 0 AND  
 				--				 R.Code NOT LIKE rov.ExcludedCodes COLLATE Latin1_General_BIN THEN 1
			
					--			   ELSE 0 END )					 
					--		AND [RouteOption].RouteID = r.RouteID
					--		AND [RouteOption].SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)
					--		AND rov.IncludeSubCodes='Y' 
					--		And rov.IsEffective='Y'
								
						COMMIT

				
				End
				select 'begin Signaling',GETDATE()
				EXEC bp_SignalRouteUpdatesFromOptions
					@UpdateStamp = @UpdateStamp,
					@UpdateType = 'RouteOverride'			
			select 'Endprocess',GETDATE()

END