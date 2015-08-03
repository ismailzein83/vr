CREATE FUNCTION [dbo].[fn_RT_Full_GetSubZones]
(
	@Code VARCHAR(20),
	@ZoneID INT,
	@ExcludedCodes VARCHAR(MAX)
)
 
RETURNS nvarchar(MAX)

AS
BEGIN


  DECLARE @str VARCHAR(MAX) 
   SELECT @str = COALESCE(@str + ',', '') + CAST(rcp.ZoneID as VARCHAR(20)) FROM  RoutePool rcp 
	 
	 WHERE rcp.Code LIKE (@Code + '%') AND rcp.ZoneID <> @ZoneID AND rcp.Code NOT IN (SELECT * FROM dbo.ParseArray(@ExcludedCodes,',') pa)
	
	
RETURN @str
END