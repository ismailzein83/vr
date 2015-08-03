
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fn_RT_Full_GetBlockRules]
(	
	-- Add the parameters for the function here
@Level INT,
@ZoneId INT,
@CodeGroup VARCHAR(250)
)
RETURNS @BlockRules TABLE 
(
	CustomerID VARCHAR(5),
	SupplierID VARCHAR(5),
	Code VARCHAR(20),
	ZoneID INT,
	UpdateDate SMALLDATETIME,
	IncludeSubCodes CHAR(1),
	ExcludedCodes VARCHAR(250)
	)
AS

BEGIN
	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	
	

	----- General Route Pool Rule
	IF(@Level = 1)
	BEGIN
		INSERT INTO @BlockRules
		SELECT 	
								rb.CustomerID,
								rb.SupplierID,
								rb.Code,
								rb.ZoneID,
								rb.UpdateDate,
								rb.IncludeSubCodes,
								rb.ExcludedCodes
								
								
							FROM   RouteBlock rb WITH (NOLOCK) 
							WHERE 
								rb.IsEffective = 'Y' 
								AND rb.CustomerID = 'SYS'
								AND rb.SupplierID IS NULL 
								AND rb.ZoneID IS NULL 
								AND rb.Code IS NOT NULL 								
								
	END
	
	ELSE
		------- General Route Option Pool
		IF(@Level = 2)
		BEGIN
			INSERT INTO @BlockRules
					SELECT 	
								rb.CustomerID,
								rb.SupplierID,
								rb.Code,
								rb.ZoneID,
								rb.UpdateDate,
								rb.IncludeSubCodes,
								rb.ExcludedCodes
								
								
							FROM   RouteBlock rb WITH (NOLOCK) 
							WHERE 
								rb.IsEffective = 'Y' 
								AND rb.CustomerID IS NULL 
								AND rb.SupplierID IS NOT NULL 
								AND rb.ZoneID IS NULL 
								AND rb.Code IS NOT NULL 
		END
	ELSE 
		----- General Zone Rate Rule
		IF(@Level = 3)
		BEGIN
			
				;WITH SupplierZoneBlock	AS (					
				--INSERT INTO @BlockRules
					SELECT 	
							ca.CarrierAccountID CustomerID,
							rb.SupplierID,
							rb.Code,
							rb.ZoneID,
							rb.UpdateDate,
							rb.IncludeSubCodes,
							rb.ExcludedCodes
								
					FROM   RouteBlock rb WITH (NOLOCK)
					JOIN CarrierAccount ca ON ca.IsDeleted = 'N'
					WHERE 
							rb.IsEffective = 'Y' 
							AND rb.ZoneID IS NOT NULL 
							And RB.CustomerID is null 
							AND rb.Code IS NULL
							AND ca.ActivationStatus <> @Account_Inactive
							And ca.RoutingStatus <> @Account_BlockedInbound 
							AND ca.RoutingStatus <> @Account_Blocked 
					)
					,
					SaleZoneBlock AS (					
					SELECT 	
							ro.CustomerID,
							'SYS' SupplierID,
							ro.Code,
							ro.OurZoneID ZoneID,
							ro.Updated UpdateDate,
							ro.IncludeSubCodes,
							ro.ExcludedCodes 
					FROM	RouteOverride ro 
					WHERE	[RouteOptions]='BLK' AND BlockedSuppliers IS NULL 
							AND ro.OurZoneID >0  AND ro.Code = '*ALL*' AND ro.IsEffective='Y' 
					)
					,
					-- Code Groupe rules translated into Zone- rule, but not exist in the ZaleZoneRule
					SaleZoneBlockForCodeGroup  AS (
					SELECT 
							ro.CustomerID,
							'SYS' SupplierID,
							ro.Code,
							z.ZoneID,
							ro.Updated,
							ro.IncludeSubCodes,
							ro.ExcludedCodes
							
					FROM	RouteOverride ro	
							INNER JOIN Zone z ON ro.Code = z.CodeGroup 
							LEFT  JOIN SaleZoneBlock bz ON ro.CustomerID = bz.CustomerID AND bz.ZoneID=z.ZoneID 		
					
					WHERE	ro.[RouteOptions]='BLK' AND ro.BlockedSuppliers IS NULL 
							AND ro.OurZoneID = -1   AND ro.Code <> '*ALL*'  AND ro.IsEffective='Y'
							AND ro.IncludeSubCodes = 'Y'  
							--AND EXISTS (SELECT cg.Code FROM CodeGroup cg WHERE cg.Code = ro.Code) -- No need for this condition, already considered in the join with Zone
							AND z.SupplierID = 'SYS' AND z.IsEffective = 'Y'
							AND bz.CustomerID IS NULL AND bz.ZoneID IS NULL					
					)
					
				INSERT INTO @BlockRules
				SELECT * FROM SupplierZoneBlock
				UNION ALL SELECT * FROM SaleZoneBlock
				UNION ALL SELECT * FROM SaleZoneBlockForCodeGroup
		END
	RETURN
END