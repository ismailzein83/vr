CREATE FUNCTION [dbo].[fn_RT_Full_GetChildSubCodes]
(
	@Code VARCHAR(20),
	@ExcludedCodes VARCHAR(MAX)
)
 
RETURNS nvarchar(MAX)

AS
BEGIN


  DECLARE @str VARCHAR(MAX) 
   SELECT @str = COALESCE(@str + ',', '') + rcp.Code FROM  RoutePool rcp 
	 
	 WHERE rcp.Code LIKE (@Code + '%') AND rcp.Code NOT IN (SELECT * FROM dbo.ParseArray(@ExcludedCodes,',') pa)
	
	
RETURN @str
END