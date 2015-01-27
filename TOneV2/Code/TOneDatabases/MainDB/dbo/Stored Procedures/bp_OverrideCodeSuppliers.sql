-- =================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-30
-- Updated: 2010-04-26 (Fixed Problem with Route Override On Our Zone when partially matching Supplier Zone Is Blocked)
-- Updated: 2010-05-17 (fixed blocked and onactive customers, Reverted to old way of managing overrides on our zones)
-- Updated: 2012-09-26 (fixed conflicts between overrides done by Zone, Code with subcodes and single codes)
-- Updated: 2012-11-06 (fixing conflicts between overrides done by Zone, Code with subcodes and single codes for single customer Application data entry)
-- Description:	Updates Routes / Route Options from Route Overrides
-- =================================================================
CREATE   PROCEDURE [dbo].[bp_OverrideCodeSuppliers] 
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

	CREATE TABLE #tmpRO_tmp (CustomerID nvarchar(5), Code nvarchar(15), IncludeSubCodes CHAR(1),ExcludedCodes VARCHAR(100),OurZoneID int, SupplierID nvarchar(5),BlockedSuppliers varchar(1024),Percentage INT, Position INT, SupplierZoneID INT NULL)
	CREATE TABLE #tmpRO_tmp_ZonesToCodes (CustomerID nvarchar(5), Code nvarchar(15), IncludeSubCodes CHAR(1),ExcludedCodes VARCHAR(100),OurZoneID int, SupplierID nvarchar(5),BlockedSuppliers varchar(1024), Percentage INT, Position INT, SupplierZoneID INT NULL)
	CREATE INDEX IDX_Customer ON #tmpRO_tmp (CustomerID)
	CREATE INDEX IDX_Code ON #tmpRO_tmp (Code)
	CREATE INDEX IDX_OurZoneID ON #tmpRO_tmp (OurZoneID)
	CREATE INDEX IDX_SupplierZoneID ON #tmpRO_tmp (SupplierZoneID)
	CREATE INDEX IDX_SupplierID ON #tmpRO_tmp (SupplierID)
	
	CREATE INDEX IDX_Customer_ ON #tmpRO_tmp_ZonesToCodes (CustomerID)
	CREATE INDEX IDX_Code_ ON #tmpRO_tmp_ZonesToCodes (Code)
	CREATE INDEX IDX_OurZoneID_ ON #tmpRO_tmp_ZonesToCodes (OurZoneID)
	CREATE INDEX IDX_SupplierZoneID_ ON #tmpRO_tmp_ZonesToCodes (SupplierZoneID)
	CREATE INDEX IDX_SupplierID_ ON #tmpRO_tmp_ZonesToCodes (SupplierID)
	
	DECLARE @ValidCustomers TABLE(CustomerID NVARCHAR(5) PRIMARY KEY)


			INSERT INTO @ValidCustomers(CustomerID) 
				SELECT ca.CarrierAccountID FROM CarrierAccount ca  with(nolock) 
					WHERE ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound)
					AND ca.ActivationStatus NOT IN (@Account_Inactive)
					AND ca.IsDeleted = 'N' 

	DECLARE @MaxSuppliersPerRoute INT 
	SET @MaxSuppliersPerRoute = 8

	--select 'Beginprocess',GETDATE()
	-- Process All Routing Overrides 

		BEGIN

			INSERT INTO #tmpRO_tmp with (tablock) (CustomerID, Code, IncludeSubCodes, ExcludedCodes, OurZoneID, Percentage,SupplierID,BlockedSuppliers, Position) 
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
			UNION
				SELECT 
					ro.CustomerID, ro.Code,ro.IncludeSubCodes,ro.ExcludedCodes, ro.OurZoneID,0,NULL,ro.BlockedSuppliers,0
									
				FROM RouteOverride ro WITH(NOLOCK) WHERE ro.BlockedSuppliers IS not NULL  
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) And IsEffective='Y' 	
			ORDER BY ro.CustomerID, ro.Code, Position
		End

		--IF @CustomerID IS not NULL and @OurZoneID <>-1
		--	Set @TargetCode=''
				

------------- Begin expand zone base overrides as a codes ------------- 		
           INSERT INTO #tmpRO_tmp_ZonesToCodes with (tablock)
           SELECT tr.CustomerID, cm.Code, tr.IncludeSubCodes, tr.ExcludedCodes, tr.OurZoneID,
                tr.SupplierID,tr.BlockedSuppliers, tr.Percentage, tr.Position, tr.SupplierZoneID
           FROM #tmpRO_tmp tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK)
           WHERE cm.SupplierZoneID = tr.OurZoneID
           AND cm.SupplierID ='SYS' AND tr.Code = '*ALL*'; 
------------- End expand zone base overrides as a codes ------------- 		


------------- Begin expand code with subcodes base overrides as a codes ------------- 		
		With CodesWithSub As(
			SELECT tr.CustomerID, cm.Code, IncludeSubCodes, tr.ExcludedCodes, tr.OurZoneID,
				tr.SupplierID,tr.BlockedSuppliers, tr.Percentage, tr.Position, tr.SupplierZoneID,tr.Code GroupCode
			FROM #tmpRO_tmp tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK)
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
           INSERT INTO #tmpRO_tmp_ZonesToCodes with (tablock)
           SELECT FCS.CustomerID, FCS.Code, FCS.IncludeSubCodes, FCS.ExcludedCodes, FCS.OurZoneID,
                FCS.SupplierID,FCS.BlockedSuppliers, FCS.Percentage, FCS.Position, FCS.SupplierZoneID
           FROM FinalCodesWithSub  FCS WITH(NOLOCK)

------------- End expand code with subcodes base overrides as a codes ------------- 		

------------- Remove Zone & code with subcodes base overrides from #tmpRO_tmp ------------- 		
           DELETE FROM #tmpRO_tmp WHERE OurZoneID > 0 
           DELETE FROM #tmpRO_tmp WHERE IncludeSubCodes='Y' 
           
------------- Inserting expanded Codes #tmpRO_tmp ------------- 		

           INSERT INTO #tmpRO_tmp with (tablock) SELECT trztc.CustomerID, trztc.Code,
                                     trztc.IncludeSubCodes, trztc.ExcludedCodes,
                                     trztc.OurZoneID, trztc.SupplierID,trztc.BlockedSuppliers,
                                     trztc.Percentage, trztc.Position,
                                     trztc.SupplierZoneID
                                FROM #tmpRO_tmp_ZonesToCodes trztc WITH(NOLOCK) 
           
-------------  Set the Matching Supplier Zones from Codes in RO based on direct one to one code match
			UPDATE #tmpRO_tmp
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO_tmp tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK) 
			WHERE tr.Code = cm.Code COLLATE Latin1_General_BIN AND tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			-- Set the Matching Supplier Zones based on supplier our zone match
			UPDATE #tmpRO_tmp
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO_tmp tr WITH(NOLOCK), CodeMatch cm WITH(NOLOCK),ZoneMatch zm WITH(NOLOCK) 
			WHERE   tr.SupplierZoneID IS NULL      
				  AND tr.OurZoneID = zm.OurZoneID 
			      AND  zm.SupplierZoneID = cm.SupplierZoneID 
			      AND  tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			Delete #tmpRO_tmp where code  COLLATE Latin1_General_BIN in(
			SELECT rov.code
			  FROM #tmpRO_tmp rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK) 
			WHERE rov.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) 
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO_tmp.CustomerID
				)and #tmpRO_tmp.OurZoneID>0 

			Delete #tmpRO_tmp where code  COLLATE Latin1_General_BIN in(
			SELECT rov.code
			  FROM #tmpRO_tmp rov WITH(NOLOCK), CarrierAccount ca WITH(NOLOCK) 
			WHERE rov.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) 
				AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound
				AND ca.IsDeleted = 'N'
				AND rov.IncludeSubCodes ='N' and rov.OurZoneID =-1 AND rov.CustomerID = #tmpRO_tmp.CustomerID
				)and #tmpRO_tmp.IncludeSubCodes='Y'
			
			SELECT CustomerID,Code,SupplierID,percentage,position
			FROM #tmpRO_tmp
			WHERE supplierid IS NOT null
			ORDER BY CustomerID,Code,position,percentage
END