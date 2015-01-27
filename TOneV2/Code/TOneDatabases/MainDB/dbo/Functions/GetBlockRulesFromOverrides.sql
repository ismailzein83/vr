
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetBlockRulesFromOverrides]
(	
	-- Add the parameters for the function here
@Level VARCHAR(20),
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
	
	----- CodeOnly
	IF(@Level = 'Code')
	BEGIN
		INSERT INTO @BlockRules
		SELECT 	
								ro.CustomerID,
								ro.BlockedSuppliers,
								ro.Code,
								ro.OurZoneID,
								ro.Updated,
								ro.IncludeSubCodes,
								ro.ExcludedCodes
								
								
							FROM RouteOverride ro
							
								WHERE ro.Code <> '*all*'
									  AND ro.IsEffective = 'Y'
									  AND (ro.RouteOptions = 'BLK')
									  AND ro.BlockedSuppliers IS NULL
									  AND ro.Code NOT IN (SELECT cg.Code FROM CodeGroup cg)								
								
	END
	

	ELSE 
		-----Zones
		IF(@Level = 'Zones')
		BEGIN
				INSERT INTO @BlockRules
			SELECT 	
								ro.CustomerID,
								ro.BlockedSuppliers,
								ro.Code,
								ro.OurZoneID,
								ro.Updated,
								ro.IncludeSubCodes,
								ro.ExcludedCodes
								
								
							FROM RouteOverride ro
								WHERE ro.OurZoneID <> -1
									  AND ro.IsEffective = 'Y'
									  AND (ro.RouteOptions = 'BLK' OR ro.BlockedSuppliers IS NOT NULL)	
		END
		
	ELSE 
		-----Code Groups
		IF(@Level = 'CodeGroup')
		BEGIN
				INSERT INTO @BlockRules
			SELECT 	
								ro.CustomerID,
								ro.BlockedSuppliers,
								ro.Code,
								ro.OurZoneID,
								ro.Updated,
								ro.IncludeSubCodes,
								ro.ExcludedCodes
								
								
							FROM RouteOverride ro
								WHERE ro.Code <> '*all*'
									  AND ro.IsEffective = 'Y'
									  AND (ro.RouteOptions = 'BLK' OR ro.BlockedSuppliers IS NOT NULL)
									  AND ro.Code IN (SELECT cg.Code FROM CodeGroup cg)	
		END	
		
	RETURN
END