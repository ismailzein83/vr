-- =====================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-23
-- Update date: 2009-06-24 (Zone ID < 1)
-- Description:	Get a list of Route Overrides Matching the given params
-- =====================================================================
CREATE FUNCTION [dbo].[GetRouteOverrides](@CustomerID VARCHAR(10), @OurZoneID INT, @Code VARCHAR(15), @OverrideType VARCHAR(20) = 'ALL')
RETURNS TABLE
AS
RETURN 
(
	SELECT * FROM RouteOverride ro
		WHERE 
			@OverrideType IN ('OVERRIDES', 'ALL') 
		AND (ro.CustomerID = @CustomerID OR ro.CustomerID = '*ALL*')
		AND (ro.Code = @Code OR ro.Code = '*ALL*')
		AND (ro.OurZoneID = @OurZoneID OR ro.OurZoneID < 1)
		AND ro.IsEffective = 'Y'
		AND ro.RouteOptions IS NOT NULL
	UNION ALL
	SELECT * FROM RouteOverride ro
		WHERE 
			@OverrideType IN ('BLOCKS', 'ALL') 
		AND (ro.CustomerID = @CustomerID OR ro.CustomerID = '*ALL*')
		AND (ro.Code = @Code OR ro.Code = '*ALL*')
		AND (ro.OurZoneID = @OurZoneID OR ro.OurZoneID < 1)
		AND ro.IsEffective = 'Y'
		AND ro.BlockedSuppliers IS NOT NULL	
)