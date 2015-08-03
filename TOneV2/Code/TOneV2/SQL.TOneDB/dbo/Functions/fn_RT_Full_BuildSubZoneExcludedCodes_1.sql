CREATE FUNCTION [dbo].[fn_RT_Full_BuildSubZoneExcludedCodes]
(
	@ZoneID INT,
	@Code VARCHAR(20),
	@ExcludedCodes VARCHAR(MAX),
	@ExcludedZoneCodes VARCHAR(MAX)
)
 
RETURNS nvarchar(MAX)

AS
BEGIN




SELECT @ExcludedZoneCodes = COALESCE(@ExcludedZoneCodes + ',', '') + rcp.Code FROM  RouteCodesPool rcp 
	 
	 WHERE rcp.Code LIKE (@Code + '%') AND rcp.SupplierZoneID = @ZoneID AND rcp.Code NOT IN (SELECT * FROM dbo.ParseArray(@ExcludedCodes,',') pa)
	
	
RETURN @ExcludedZoneCodes
END